namespace SpatialIndexesComparison.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    using SpatialIndexesComparison.Extensions;

    public class QGisService : IDisposable
    {
        private readonly string _path;

        private readonly int _timeout;

        private delegate string ReadToEndDelegate();

        public QGisService(int timeout = 60)
        {
            this._timeout = timeout;
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
                            RedirectStandardOutput = true,
                        };

            using (Process process = Process.Start(start))
            {
                
                using (StreamReader reader = process.StandardOutput)
                {
                    ReadToEndDelegate asyncCall = reader.ReadToEnd;
                    IAsyncResult asyncResult = asyncCall.BeginInvoke(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(_timeout));
                    if (asyncResult.IsCompleted)
                    {
                        var processResult = asyncCall.EndInvoke(asyncResult);
                        return processResult.GetDouble(-1.0d);
                    }
                    return -1.0d;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}