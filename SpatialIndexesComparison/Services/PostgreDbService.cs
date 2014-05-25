namespace SpatialIndexesComparison.Services
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    using Npgsql;

    using SpatialIndexesComparison.Enums;

    public class PostgreDbService : IDisposable
    {
        private readonly NpgsqlConnection _conn;

        public PostgreDbService()
        {
            _conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;Password=boss;Database=spatial_index_comparison; CommandTimeout=300; ConnectionLifeTime=3;");
            _conn.Open();
        }

        public int ExecuteSqlCommand(string commandText)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (var command = new NpgsqlCommand(commandText, _conn))
                command.ExecuteNonQuery();

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;

            return (int)ts.TotalMilliseconds;
        }

        public void CreateIndex(IndexEnum index, DataSizeEnum dataSize)
        {
            string indexName = "random_points_" + (int)dataSize + "_" + index + @"_idx";

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
	                ON random_points_" + (int)dataSize + @"
	                USING " + index + @" (geom);
                END IF;
                END$$;";

            using (var command = new NpgsqlCommand(createIndexCommandText, _conn))
                command.ExecuteNonQuery();
        }

        public void RemoveIndex(IndexEnum index, DataSizeEnum dataSize)
        {
            string indexName = "random_points_" + (int)dataSize + "_" + index + @"_idx";
            string dropIndexCommandText = @"DROP INDEX IF EXISTS " + indexName;

            using (var command = new NpgsqlCommand(dropIndexCommandText, _conn))
                command.ExecuteNonQuery();
        }

        public void DisableIndex(IndexEnum index, DataSizeEnum dataSize)
        {
            string indexName = "random_points_" + (int)dataSize + "_" + index + @"_idx";
            string dropIndexCommandText = @"UPDATE pg_index SET indislive = false, indisvalid = false where indexrelid = '" + indexName + @"'::regclass;";
            int r = 0;
            using (var command = new NpgsqlCommand(dropIndexCommandText, _conn))
                r = command.ExecuteNonQuery();
            if(r != 1) throw new Exception("Nieudana kwerenda");
        }

        public void EnableIndex(IndexEnum index, DataSizeEnum dataSize)
        {
            string indexName = "random_points_" + (int)dataSize + "_" + index + @"_idx";
            string dropIndexCommandText = @"UPDATE pg_index SET indislive = true, indisvalid = true where indexrelid = '" + indexName + @"'::regclass;";
            int r = 0;
            using (var command = new NpgsqlCommand(dropIndexCommandText, _conn))
                r = command.ExecuteNonQuery();
            if (r != 1) throw new Exception("Nieudana kwerenda");
        }

        public void Dispose()
        {
            _conn.Close();
        }
    }
}