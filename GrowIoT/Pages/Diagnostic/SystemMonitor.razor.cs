using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using GrowIoT.Interfaces;
using GrowIoT.Models.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor.CircularGauge;

namespace GrowIoT.Pages.Diagnostic
{
    public partial class SystemMonitor : IDisposable
    {

        #region DI

        [Inject] protected ILogger<SystemMonitor> Logger { get; private set; }
        [Inject] protected IMemoryMetricsClient MemoryMetricsClient { get; private set; }
        #endregion


        private Timer _timer;


        protected SfCircularGauge CpuGauge;
        [Parameter]
        public double CpuUsageByCurrentProcess { get; set; }
        [Parameter]
        public MemoryMetrics Metrics { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_timer == null)
            {
                _timer = new Timer(2000) { AutoReset = true, Enabled = true };
                _timer.Elapsed += DisplayTimerElapsed;
                _timer.Start();
            }
            CpuUsageByCurrentProcess = await GetCpuUsageForProcess();
            Metrics = MemoryMetricsClient.GetMetrics();
            await CpuGauge.SetPointerValue(0, 0, CpuUsageByCurrentProcess);
            await base.OnAfterRenderAsync(firstRender);
        }


        private async void DisplayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var metrics = MemoryMetricsClient.GetMetrics();
                var newUsage = await GetCpuUsageForProcess();
                Metrics = metrics;
                CpuUsageByCurrentProcess = newUsage == 0 ? CpuUsageByCurrentProcess : newUsage;
                await InvokeAsync(async () =>
                {
                    StateHasChanged();
                    await CpuGauge.SetPointerValue(0, 0, CpuUsageByCurrentProcess);
                });

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DisplayTimerElapsed));
            }
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Dispose();
                        _timer = null;

                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
