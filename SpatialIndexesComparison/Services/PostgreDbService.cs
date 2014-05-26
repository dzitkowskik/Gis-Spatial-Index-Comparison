namespace SpatialIndexesComparison.Services
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using Npgsql;
    using SpatialIndexesComparison.Enums;

    public class PostgreDbService : IDisposable
    {
        private readonly NpgsqlConnection _conn;

        public PostgreDbService(int timeout = 10)
        {
            var builder = new NpgsqlConnectionStringBuilder
                          {
                              Host = "localhost", 
                              Port = 5432, 
                              Password = "boss", 
                              Database = "spatial_index_comparison", 
                              UserName = "postgres", 
                              CommandTimeout = timeout, 
                              ConnectionLifeTime = 3
                          };
            _conn = new NpgsqlConnection(builder);
            _conn.Open();
        }

        public double ExecuteSqlCommand(string commandText)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                using (var command = new NpgsqlCommand(commandText, _conn))
                    command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                return -1.0;
            }
            catch (NpgsqlException ex)
            {
                return -1.0d;
            }
            finally
            {
                stopWatch.Stop();                
            }

            var ts = stopWatch.Elapsed;

            return ts.TotalMilliseconds;
        }

        public void CreateTable(DataSizeEnum dataSize, DataEnum data)
        {
            string tableName = data.ToString();
            tableName += (dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)dataSize);

            string createTableCommandText =
@"DO 
$BODY$
DECLARE
	i INT := 0;
BEGIN
	IF NOT EXISTS 
	(
		SELECT 1
		FROM   pg_class c
		JOIN   pg_namespace n ON n.oid = c.relnamespace
		WHERE  c.relname = '"+tableName+@"'
		AND    n.nspname = 'public'
	) 
	THEN
		CREATE TABLE " + tableName + @"
		(
			id serial NOT NULL,
			geom geometry,
			CONSTRAINT " + tableName + @"_pk PRIMARY KEY (id),
			CONSTRAINT enforce_geotype_geom CHECK (geometrytype(geom) = 'POINT'::text OR geom IS NULL),
			CONSTRAINT enforce_srid_geom CHECK (st_srid(geom) = 4326)
		) WITH (OIDS=FALSE);
		ALTER TABLE " + tableName + @" OWNER TO postgres;

" + (data != DataEnum.countries ? @"
		FOR i IN 1.." + (int)dataSize + @" 
		LOOP
			INSERT INTO " + tableName + @"(geom)
				VALUES (ST_SetSRID(ST_MakePoint((random()*360)-180, (random()*180-90)), 4326));
		END LOOP;" : string.Empty) + @"
	END IF;
END
$BODY$;";

            using (var command = new NpgsqlCommand(createTableCommandText, _conn))
                command.ExecuteNonQuery();
        }

        public void CreateIndex(IndexEnum index, DataSizeEnum dataSize, DataEnum data)
        {
            if (data == DataEnum.countries) return;

            string indexName = data.ToString();
            indexName += (dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)dataSize);
            indexName += "_" + index + @"_idx";

            string tableName = data.ToString();
            tableName += (dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)dataSize);

            string createIndexCommandText =
                @"DO $$
                BEGIN
                IF NOT EXISTS (
                    SELECT 1
                    FROM   pg_class c
                    JOIN   pg_namespace n ON n.oid = c.relnamespace
                    WHERE  c.relname = '" + indexName + @"'
                    AND    n.nspname = 'public'
                    ) THEN
                    CREATE INDEX " + indexName + @"
	                ON " + tableName + @"
	                USING " + index + @" (geom);
                END IF;
                END$$;";

            using (var command = new NpgsqlCommand(createIndexCommandText, _conn))
                command.ExecuteNonQuery();
        }

        public void RemoveIndex(IndexEnum index, DataSizeEnum dataSize, DataEnum data)
        {
            if (data == DataEnum.countries) return;

            string indexName = data.ToString();
            indexName += (dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)dataSize);
            indexName += "_" + index + @"_idx";

            string dropIndexCommandText = @"DROP INDEX IF EXISTS " + indexName;

            using (var command = new NpgsqlCommand(dropIndexCommandText, _conn))
                command.ExecuteNonQuery();
        }

        public void DisableIndex(IndexEnum index, DataSizeEnum dataSize, DataEnum data)
        {
            if (data == DataEnum.countries) return;

            string indexName = data.ToString();
            indexName += (dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)dataSize);
            indexName += "_" + index + @"_idx";

            string dropIndexCommandText = @"UPDATE pg_index SET indislive = false, indisvalid = false where indexrelid = '" + indexName + @"'::regclass;";
            int rowsChanged;
            using (var command = new NpgsqlCommand(dropIndexCommandText, _conn))
                rowsChanged = command.ExecuteNonQuery();
            if (rowsChanged != 1) throw new Exception("Nieudana kwerenda - " + dropIndexCommandText);
        }

        public void EnableIndex(IndexEnum index, DataSizeEnum dataSize, DataEnum data)
        {
            if (data == DataEnum.countries) return;

            string indexName = data.ToString();
            indexName += (dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)dataSize);
            indexName += "_" + index + @"_idx";

            string dropIndexCommandText = @"UPDATE pg_index SET indislive = true, indisvalid = true where indexrelid = '" + indexName + @"'::regclass;";
            int rowsChanged;
            using (var command = new NpgsqlCommand(dropIndexCommandText, _conn))
                rowsChanged = command.ExecuteNonQuery();
            if (rowsChanged != 1) throw new Exception("Nieudana kwerenda - " + dropIndexCommandText);
        }

        public void Dispose()
        {
            _conn.Close();
        }

        public void InitializeDb()
        {
            foreach (var value in Enum.GetValues(typeof(DataSizeEnum)))
            {
                var data = DataEnum.random_points;
                if ((DataSizeEnum)value == DataSizeEnum.None)
                    continue;
                this.CreateTable((DataSizeEnum)value, data);
                this.CreateIndex(IndexEnum.gist, (DataSizeEnum)value, data);
                this.CreateIndex(IndexEnum.btree, (DataSizeEnum)value, data);
            }
        }
    }
}