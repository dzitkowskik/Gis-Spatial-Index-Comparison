namespace SpatialIndexesComparison.Services
{
    using System;
    using System.Diagnostics;

    using Npgsql;

    public class PostgreDbService
    {
        public int FirstSelect(string index, int numPoints, bool andOr, double distance, int size)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;Password=boss;Database=spatial_index_comparison;");
            conn.Open();

            Random rand = new Random();

            string commandText = @"SELECT * FROM public.random_points_" + index + "_" + size;
            if (numPoints > 0)
                commandText += " WHERE ";
            for (int i = 0; i < numPoints; i++)
            {
                var lon = rand.NextDouble() * 360 - 180;
                var lat = rand.NextDouble() * 180 - 90;
                if (i != 0) commandText += andOr ? " AND " : " OR ";
                commandText += "ST_DWithin(geom, ST_SetSRID(ST_MakePoint("+lon+", "+lat+"), 4326), "+distance+")";
            }
            commandText += ";";
	                                                     
            NpgsqlCommand command = new NpgsqlCommand(commandText, conn);
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                command.ExecuteReader();

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                return (int)ts.TotalMilliseconds;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}