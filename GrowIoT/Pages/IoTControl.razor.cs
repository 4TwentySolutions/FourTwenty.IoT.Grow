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
using Infrastructure.Entities;

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

        public ChartSeriesType ChartType { get; set; } = ChartSeriesType.MultiColoredLine;


        protected SfChart HistoryChart;



        protected override async Task OnInitializedAsync()
        {
            if (!Id.HasValue)
                return;

            Module = await BoxManager.GetModule(Id.Value);
            Module.DataReceived += ModuleOnDataReceived;
            Module.StateChanged += ModuleOnStateChanged;

            var moduleHistory = HistoryService.GetModuleHistory(Module.Id).TakeLast(50).ToList();
            var historyData = new List<ChartData>();

            if (Module.Type == ModuleType.HumidityAndTemperature)
            {
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
                        Text = itemData.Humidity + "%",
                        Text2 = itemData.Temperature + "°C",
                    };

                    historyData.Add(historyItem);
                }
            }

            if (Module.Type == ModuleType.Relay)
            {
                var dataList = new List<(ModuleHistoryItem item, RelayData data)>();

                //TODO make it better

                foreach (var item in moduleHistory)
                {
                    var itemData = JsonConvert.DeserializeObject<RelayData>(item.Data);
                    dataList.Add((item, itemData));
                }

                foreach (var item in dataList.GroupBy(x => x.item.Date))
                {

                    var historyItem = new ChartData
                    {
                        X = item.Key.ToString("H:mm:ss")
                    };

                    var it = item.FirstOrDefault(x => x.data.Pin == Module.Pins.FirstOrDefault());
                    if (it.data != null)
                    {
                        historyItem.Y = it.data.Pin.ToString();
                        historyItem.Color = it.data.State == RelayState.Opened ? "green" : "red";
                        historyItem.Text = it.data.State == RelayState.Opened ? "ON" : "OFF";
                    }
                    

                    var it2 = item.FirstOrDefault(x => x.data.Pin == Module.Pins.LastOrDefault());
                    if (it2.data != null)
                    {
                        historyItem.Y2 = it2.data.Pin.ToString();
                        historyItem.Color2 = it2.data.State == RelayState.Opened ? "green" : "red";
                        historyItem.Text2 = it2.data.State == RelayState.Opened ? "ON" : "OFF";
                    }

                    historyData.Add(historyItem);
                }
            }

            HistoryData = new ObservableCollection<ChartData>(historyData);

            await base.OnInitializedAsync();
        }

        private async void ModuleOnStateChanged(object? sender, RelayEventArgs e)
        {
            var currentData = HistoryData?.ToList() ?? new List<ChartData>();

            currentData.RemoveAt(0);

            var historyItem = new ChartData
            {
                X = DateTime.Now.ToString("H:mm:ss")
            };

            if(e.Data.Pin == Module.Pins.FirstOrDefault())
            {
                historyItem.Y = e.Data.Pin.ToString();
                historyItem.Color = e.Data.State == RelayState.Opened ? "green" : "red";
                historyItem.Text = e.Data.State == RelayState.Opened ? "ON" : "OFF";
            }

            if (e.Data.Pin == Module.Pins.LastOrDefault())
            {
                historyItem.Y2 = e.Data.Pin.ToString();
                historyItem.Color2 = e.Data.State == RelayState.Opened ? "green" : "red";
                historyItem.Text2 = e.Data.State == RelayState.Opened ? "ON" : "OFF";
            }

            currentData.Add(historyItem);

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

        private async void ModuleOnDataReceived(object? sender, SensorEventArgs e)
        {
            var currentData = HistoryData?.ToList() ?? new List<ChartData>();

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
