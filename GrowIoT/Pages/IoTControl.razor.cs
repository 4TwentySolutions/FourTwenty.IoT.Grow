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
using Blazored.Toast.Services;
using FourTwenty.IoT.Connect.Entities;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using System.Collections.ObjectModel;

namespace GrowIoT.Pages
{
    public partial class IoTControl : BaseGrowComponent
    {

        #region DI

        [Inject] protected ILogger<IoTControl> Logger { get; private set; }
        [Inject] protected IHistoryService HistoryService { get; private set; }

        #endregion

        //private Timer _timer;

        [Parameter]
        public int? Id { get; set; }

        public ModuleVm Module { get; set; }

        public ObservableCollection<ChartData> HistoryData { get; set; }


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
                var historyData = new List<ChartData>();

                foreach (var item in moduleHistory)
                {
                    var itemData = JsonConvert.DeserializeObject<DhtData>(item.Data);

                    var historyItem = new ChartData
                    {
                        X = item.Date.ToString("H:mm:ss"),
                        Y = itemData.Humidity.ToString(),
                        Y2 = itemData.Temperature.ToString(),
                        Color = "blue",
                        Color2 = "red",
                        Text = itemData.Humidity.ToString() + "%",
                        Text2 = itemData.Temperature.ToString() + "°C",
                    };

                    historyData.Add(historyItem);
                }

                HistoryData = new ObservableCollection<ChartData>(historyData);
            }

            await base.OnInitializedAsync();
        }

        private void ModuleOnStateChanged(object? sender, RelayEventArgs e)
        {

        }

        private async void ModuleOnDataReceived(object? sender, SensorEventArgs e)
        {
            var currentData = HistoryData.ToList();

            currentData.RemoveAt(0);
            currentData.Add(new ChartData
            {
                X = DateTime.Now.ToString("H:mm:ss"),
                Y = ((DhtData)e.Data).Humidity.ToString(),
                Y2 = ((DhtData)e.Data).Temperature.ToString(),
                Color = "blue",
                Color2 = "red",
                Text = ((DhtData)e.Data).Humidity.ToString() + "%",
                Text2 = ((DhtData)e.Data).Temperature.ToString() + "°C",
            });

            await InvokeAsync(() =>
            {
                try
                {
                    HistoryData = new ObservableCollection<ChartData>(currentData);
                    StateHasChanged();
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, nameof(ModuleOnDataReceived));
                }

            });
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
        public string Y2;
        public string Color2;
        public string Color;
        public string Text;
        public string Text2;
    }
}
