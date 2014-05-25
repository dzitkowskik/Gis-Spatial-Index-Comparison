namespace SpatialIndexesComparison.Services
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;

    using SpatialIndexesComparison.Extensions;

    public class QGisService : IDisposable
    {
        private readonly string _path;

        public QGisService()
        {
            _path = Path.GetTempFileName().Replace(".tmp",".py");
        }

        public double ExecuteScript(string script)
        {
            File.WriteAllText(_path, script);

            var start = new ProcessStartInfo
                        {
                            FileName = @"python",
                            CreateNoWindow = true,
                            Arguments = string.Format("{0}", _path),
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    return reader.ReadToEnd().GetDouble(0.0d);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}