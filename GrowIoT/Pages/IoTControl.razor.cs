using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components.Sensors;
using GrowIoT.Interfaces;
using GrowIoT.Models.Diagnostics;
using GrowIoT.Services;
using GrowIoT.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.CircularGauge;

namespace GrowIoT.Pages
{
    public partial class IoTControl : IDisposable
    {

        #region DI

        [Inject] protected ILogger<IoTControl> Logger { get; private set; }
        [Inject] protected IHistoryService HistoryService { get; private set; }

        #endregion

        private Timer _timer;

        [Parameter]
        public int? Id { get; set; }

        public ModuleVm Module { get; set; }

        public List<ChartData> HistoryData { get; set; }


        protected SfChart HistoryChart;



        protected override async Task OnInitializedAsync()
        {
            if (!Id.HasValue)
                return;

            Module = await BoxManager.GetModule(Id.Value);
            Module.DataReceived += ModuleOnDataReceived;
            Module.StateChanged += ModuleOnStateChanged;

            var moduleHistory = HistoryService.GetModuleHistory(Module.Id).TakeLast(50).ToList();

            if (Module.Type == ModuleType.HumidityAndTemperature)
            {
                HistoryData = moduleHistory.Select(x => new ChartData
                {
                    X = x.Date.ToString("H:m:s"),
                    Y = JsonConvert.DeserializeObject<DhtData>(x.Data).ToString(),
                    Color = "blue"
                }).ToList();
            }

            await base.OnInitializedAsync();
        }

        private void ModuleOnStateChanged(object? sender, RelayEventArgs e)
        {

        }

        private void ModuleOnDataReceived(object? sender, SensorEventArgs e)
        {
            HistoryData.RemoveAt(0);
            HistoryData.Add(new ChartData
            {
                Color = "blue",
                X = DateTime.Now.ToString("H:m:s"),
                Y = ((DhtData)e.Data).ToString(),
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_timer == null)
            {
                _timer = new Timer(1000) { AutoReset = true, Enabled = true };
                _timer.Elapsed += DisplayTimerElapsed;
                _timer.Start();
            }
            //CpuUsageByCurrentProcess = await GetCpuUsageForProcess();
            //Metrics = MemoryMetricsClient.GetMetrics();
            //await CpuGauge.SetPointerValue(0, 0, CpuUsageByCurrentProcess);
            await base.OnAfterRenderAsync(firstRender);
        }

        private async void DisplayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //var metrics = MemoryMetricsClient.GetMetrics();
                //var newUsage = await GetCpuUsageForProcess();
                //HistoryData = metrics;
                //CpuUsageByCurrentProcess = newUsage == 0 ? CpuUsageByCurrentProcess : newUsage;
                //await InvokeAsync(async () =>
                //{
                //    StateHasChanged();
                //    await CpuGauge.SetPointerValue(0, 0, CpuUsageByCurrentProcess);
                //});

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(DisplayTimerElapsed));
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

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

    public class ChartData
    {
        public string X;
        public string Y;
        public string Color;
    }
}
