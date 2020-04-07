using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowIoT.Models.Diagnostics;
using Microsoft.AspNetCore.Components.Web;

namespace GrowIoT.Pages.Diagnostic
{
    public partial class ApplicationLogs
    {

        protected List<DiagnosticDailyLog> LogsGroups { get; set; }


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
                        Logs = d.Select(c => new DiagnosticLog() { Path = c.path }).ToList()
                    }).ToList();

            }
            return base.OnInitializedAsync();
        }

        private string ReadFile(string sourceFilePath)
        {

            using FileStream sourceStream = File.Open(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(sourceStream);
            return reader.ReadToEnd().Replace("\n","<br/>");
        }
        void AccordionClick(MouseEventArgs e, DiagnosticDailyLog log)
        {
            log.IsOpen = !log.IsOpen;
            StateHasChanged();
        }
    }
}
