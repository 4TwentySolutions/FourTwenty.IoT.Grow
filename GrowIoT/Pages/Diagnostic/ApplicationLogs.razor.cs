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
        protected string StringLogs { get; set; } = string.Empty;
        protected MarkupString RealTimeLogs => (MarkupString)StringLogs;
        protected bool IsStreaming { get; set; }

        private Timer Timer;

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
            if (Timer == null)
                Timer = new Timer(2500) { AutoReset = true, Enabled = true };
            Timer.Elapsed += DisplayTimerElapsed;
            Timer.Start();
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
            if (Timer != null)
            {
                Timer.Elapsed -= DisplayTimerElapsed;
                Timer.Stop();
            }
            LoggerProvider.RealTimeSink.LogReceived -= RealTimeSinkOnLogReceived;
        }

        private void ClearStreamLog()
        {
            StringLogs = string.Empty;
            StateHasChanged();
        }

        private void AccordionClick(MouseEventArgs e, DiagnosticDailyLog log)
        {
            log.IsOpen = !log.IsOpen;
            StateHasChanged();
        }

        private void DownloadLogs(DiagnosticDailyLog log)
        {

        }

        private void RealTimeSinkOnLogReceived(object? sender, LogEventArgs e)
        {
            StringLogs += $"{e.LogEvent.RenderMessage()}<br/>";
        }
        private string ReadFile(string sourceFilePath)
        {

            using FileStream sourceStream = File.Open(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(sourceStream);
            return reader.ReadToEnd().Replace("\n", "<br/>");
        }


    }
}
