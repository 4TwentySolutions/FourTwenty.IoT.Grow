using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace GrowIoT.Logging
{
    public static class LoggerProvider
    {
        public static LoggerConfiguration GetLoggerConfiguration(string environment)
        {

            string outputTemplate = "[{Timestamp:dd.MM.yyyy HH:mm:ss.fff zzz} {Level:u3} for {Platform} in {Environment}({SourceContext})] {Message:lj} {Exception}{NewLine}";
            var logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "logs.txt");
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", environment)
                .MinimumLevel.Verbose()
                .WriteTo.File(logFilePath, LogEventLevel.Verbose, rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 5000000, outputTemplate: outputTemplate);
        }
    }
}
