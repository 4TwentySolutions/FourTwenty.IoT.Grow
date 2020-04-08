using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BlazorStrap;
using GrowIoT.Interfaces;
using GrowIoT.Logging;
using GrowIoT.Models.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace GrowIoT.Pages.Diagnostic
{
    public partial class ApplicationLogs
    {

        [Inject] protected IJSRuntime JSRuntime { get; private set; }
        [Inject] protected ILogger<ApplicationLogs> Logger { get; private set; }
        protected List<DiagnosticDailyLog> LogsGroups { get; set; }
        protected MarkupString RealTimeLogs => (MarkupString)LogBuilder.ToString();

        private StringBuilder LogBuilder { get; } = new StringBuilder();
        protected bool IsStreaming { get; set; }

        private Timer _timer;

        public ApplicationLogs()
        {
            LoggerProvider.RealTimeSink.LogReceived -= RealTimeSinkOnLogReceived;
        }


        protected override Task OnInitializedAsync()
        {
            var logFiles = Directory.GetFiles(Constants.Constants.LogsDirectory)?.ToList();
            if (logFiles != null && logFiles.Any())
            {
                LogsGroups = logFiles.Select(d => new
                {
                    date = Path.GetFileName(d).Replace("logs", "").Replace(".txt", ""),
                    path = d
                }).GroupBy(d => d.date)
                    .Select(d => new DiagnosticDailyLog()
                    {
                        Date = DateTime.ParseExact(d.Key, "yyyyMMdd", CultureInfo.InvariantCulture),
                        Logs = d.Select(c => new DiagnosticLog() { Path = c.path }).FirstOrDefault()
                    }).ToList();

            }
            return base.OnInitializedAsync();
        }

        private void StartLogStream()
        {
            IsStreaming = true;
            if (_timer == null)
                _timer = new Timer(2500) { AutoReset = true, Enabled = true };
            _timer.Elapsed += DisplayTimerElapsed;
            _timer.Start();
            LoggerProvider.RealTimeSink.LogReceived += RealTimeSinkOnLogReceived;
        }

        private void DisplayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
            JSRuntime.InvokeVoidAsync("scrollLogsToBottom");
        }

        private void StopStreamLog()
        {
            IsStreaming = false;
            if (_timer != null)
            {
                _timer.Elapsed -= DisplayTimerElapsed;
                _timer.Stop();
            }
            LoggerProvider.RealTimeSink.LogReceived -= RealTimeSinkOnLogReceived;
        }

        private void ClearStreamLog()
        {
            LogBuilder.Clear();
            StateHasChanged();
        }

        private void AccordionClick(MouseEventArgs e, DiagnosticDailyLog log)
        {
            log.IsOpen = !log.IsOpen;
            StateHasChanged();
        }


        private void RealTimeSinkOnLogReceived(object? sender, LogEventArgs e)
        {
            LogBuilder.Append($"{e.LogEvent.RenderMessage()}<br/>");
        }


        private void LoadFile(DiagnosticDailyLog dailylog)
        {
            dailylog.Logs.Content = ReadFile(dailylog.Logs.Path);
            StateHasChanged();
        }

        private string ReadFile(string sourceFilePath)
        {
            using FileStream sourceStream = File.Open(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(sourceStream);
            return reader.ReadToEnd().Replace("\n", "<br/>");
        }


    }
}
