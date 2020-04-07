using System.IO;
using Serilog;
using Serilog.Events;

namespace GrowIoT.Logging
{
    public static class LoggerProvider
    {
        private static RealTimeSink _realTimeSink;
        public static RealTimeSink RealTimeSink => _realTimeSink ??= new RealTimeSink();
        public static LoggerConfiguration GetLoggerConfiguration(string environment)
        {

            string outputTemplate = "[{Timestamp:dd.MM.yyyy HH:mm:ss.fff zzz} {Level:u3} for {Platform} in {Environment}({SourceContext})] {Message:lj} {Exception}{NewLine}";
            var logFilePath = Path.Combine(Constants.Constants.LogsDirectory, "logs.txt");
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", environment)
                .MinimumLevel.Verbose()
                .WriteTo.Sink(RealTimeSink)
                .WriteTo.File(logFilePath, LogEventLevel.Verbose, rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 50000000, outputTemplate: outputTemplate);
        }
    }
}
