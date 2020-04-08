using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Syncfusion.Blazor.CircularGauge;

namespace GrowIoT.Pages.Diagnostic
{
    public partial class SystemMonitor
    {
        private Timer _timer;


        protected SfCircularGauge CpuGauge;
        protected double CpuUsageByCurrentProcess { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (_timer == null)
            {
                _timer = new Timer(2000) { AutoReset = true, Enabled = true };
                _timer.Elapsed += DisplayTimerElapsed;
                _timer.Start();
            }
            CpuUsageByCurrentProcess = await GetCpuUsageForProcess();
            await base.OnInitializedAsync();
        }


        private async void DisplayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            
            CpuUsageByCurrentProcess = await GetCpuUsageForProcess();
            await CpuGauge.SetPointerValue(0, 0, CpuUsageByCurrentProcess);
        }

        private async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }
    }
}
