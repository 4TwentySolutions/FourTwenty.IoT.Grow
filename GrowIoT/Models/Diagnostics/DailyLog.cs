using System;
using System.Collections.Generic;

namespace GrowIoT.Models.Diagnostics
{
    public class DiagnosticDailyLog
    {
        public bool IsOpen { get; set; }
        public DateTime Date { get; set; }
        public List<DiagnosticLog> Logs { get; set; }
    }

    public class DiagnosticLog
    {
        public string Path { get; set; }
        public string FileName => System.IO.Path.GetFileName(Path);

        public string Content { get; set; }


    }
}
