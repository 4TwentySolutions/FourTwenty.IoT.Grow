using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace GrowIoT.Logging
{
    public class RealTimeSink : ILogEventSink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeSink"/> class.
        /// </summary>
        public RealTimeSink()
            : this(default, default, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeSink"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="restrictedToMinimumLevel">The restricted to minimum level.</param>
        public RealTimeSink(
            ITextFormatter formatProvider,
            IDictionary<string, string> properties,
            LogEventLevel restrictedToMinimumLevel)
        {
            FormatProvider = formatProvider;
            Properties = properties;
            RestrictedToMinimumLevel = restrictedToMinimumLevel;
        }

        public event EventHandler<LogEventArgs> LogReceived;
        /// <summary>
        /// Gets the format provider.
        /// </summary>
        protected ITextFormatter FormatProvider { get; }

        /// <summary>
        /// Gets the restricted to minimum level.
        /// </summary>
        protected LogEventLevel RestrictedToMinimumLevel { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        protected IDictionary<string, string> Properties { get; }
        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level < RestrictedToMinimumLevel)
                return;
            LogReceived?.Invoke(this, new LogEventArgs(logEvent));
        }

    }

    public class LogEventArgs : EventArgs
    {
        public LogEvent LogEvent { get; }

        public LogEventArgs(LogEvent log)
        {
            LogEvent = log;
        }
    }
}
