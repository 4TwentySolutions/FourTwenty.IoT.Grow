using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components.Sensors;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Syncfusion.Blazor.Charts;
using GrowIoT.Common;
using Infrastructure.Entities;
using System.Collections.ObjectModel;
using System.Drawing;

namespace GrowIoT.Pages
{
    public partial class IoTControl : BaseGrowComponent, IDisposable
    {

        private bool _isInitialized;
        #region DI

        [Inject] protected ILogger<IoTControl> Logger { get; private set; }
        [Inject] protected IHistoryService HistoryService { get; private set; }

        #endregion
        [Parameter]
        public int? Id { get; set; }

        public ModuleVm Module { get; set; }

        public Dictionary<int, ObservableCollection<RelayChartData>> HistoryData { get; set; } = new Dictionary<int, ObservableCollection<RelayChartData>>();
        public ObservableCollection<WeatherChartData> TemperatureHistoryData { get; set; } = new ObservableCollection<WeatherChartData>();


        public double Maximum
        {
            get
            {
                if (Module == null) return 0d;
                switch (Module.Type)
                {
                    case ModuleType.Temperature:
                    case ModuleType.HumidityAndTemperature:
                        return TemperatureHistoryData != null && TemperatureHistoryData.Any() ? TemperatureHistoryData.Max(d => Math.Max(d.Humidity, d.Temperature)) + 5 : 5;
                    case ModuleType.Relay:
                        return Module.Pins.Max();
                    default:
                        return 0d;
                }
            }
        }
        public double Minimum
        {
            get
            {
                if (Module == null) return 0d;
                switch (Module.Type)
                {
                    case ModuleType.Temperature:
                    case ModuleType.HumidityAndTemperature:
                        return TemperatureHistoryData != null && TemperatureHistoryData.Any() ? TemperatureHistoryData.Min(d => Math.Min(d.Humidity, d.Temperature)) - 5 : -5;
                    case ModuleType.Relay:
                        return 0d;
                    default:
                        return 0d;
                }
            }
        }

        public ChartSeriesType ChartType { get; set; } = ChartSeriesType.Line;

        public Syncfusion.Blazor.Charts.ValueType YAxisType { get; set; } = Syncfusion.Blazor.Charts.ValueType.Double;

        protected SfChart HistoryChart;



        protected override async Task OnInitializedAsync()
        {
            if (_isInitialized)
                return;
            if (!Id.HasValue)
                return;

            Module = await BoxManager.GetModule(Id.Value);


            if (Module.Type == ModuleType.HumidityAndTemperature)
            {
                var moduleHistory = (await HistoryService.GetModuleHistory(Module.Id, DateTime.Now.AddHours(-2), 50)).ToList();
                var historyData = new List<WeatherChartData>();
                foreach (var item in moduleHistory)
                {
                    var itemData = JsonConvert.DeserializeObject<DhtData>(item.Data);
                    historyData.Add(CreateChartPoint(itemData, item.Date));
                }
                TemperatureHistoryData = new ObservableCollection<WeatherChartData>(historyData);

            }

            if (Module.Type == ModuleType.Relay)
            {
                var moduleHistory = (await HistoryService.GetModuleHistory(Module.Id, DateTime.Now.AddHours(-2), 50 * Module.Pins.Length)).ToList();
                foreach (var pin in Module.Pins)
                {
                    HistoryData.Add(pin, new ObservableCollection<RelayChartData>());
                }

                var historyData = new List<RelayChartData>();
                ChartType = ChartSeriesType.Line;
                YAxisType = Syncfusion.Blazor.Charts.ValueType.Category;
                var dataList = new List<(ModuleHistoryItem item, RelayData data)>();

                //TODO make it better

                foreach (var item in moduleHistory)
                {
                    var itemData = JsonConvert.DeserializeObject<RelayData>(item.Data);
                    dataList.Add((item, itemData));
                }

                foreach (var item in dataList)
                {
                    if (!Module.Pins.Contains(item.data.Pin)) continue;
                    HistoryData[item.data.Pin].Add(CreateRelayChartPoint(item.data, item.item.Date));
                }
            }


            await base.OnInitializedAsync();

            Module.Subscribe();
            Module.VisualStateChanged += ModuleOnVisualStateChanged;
            _isInitialized = true;
        }


        private void ModuleOnVisualStateChanged(object? sender, VisualStateEventArgs e)
        {
            if (!(sender is ModuleVm module) || module != Module || HistoryChart == null) return;

            switch (module.Type)
            {
                case ModuleType.Temperature:
                    ModuleOnDataReceived(module.CurrentRawValue);
                    break;
                case ModuleType.HumidityAndTemperature:
                    ModuleOnDataReceived(module.CurrentRawValue);
                    break;
                case ModuleType.Relay:
                    ModuleOnStateChanged(module.CurrentRawValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ModuleOnStateChanged(object data)
        {

            switch (Module.Type)
            {
                case ModuleType.Relay:
                    if (!(data is RelayData relayData) || !Module.Pins.Contains(relayData.Pin)) return;
                    if (HistoryData[relayData.Pin].Count > 0)
                        HistoryData[relayData.Pin].RemoveAt(0);
                    HistoryData[relayData.Pin].Add(CreateRelayChartPoint(relayData, DateTime.Now));
                    HistoryChart.RefreshLiveData();
                    break;
            }

        }

        private void ModuleOnDataReceived(object data)
        {
            switch (Module.Type)
            {
                case ModuleType.Temperature:
                case ModuleType.HumidityAndTemperature:
                    if (!(data is DhtData dhtData)) return;
                    if (TemperatureHistoryData.Count > 0)
                        TemperatureHistoryData.RemoveAt(0);
                    TemperatureHistoryData.Add(CreateChartPoint(dhtData, DateTime.Now));
                    HistoryChart.RefreshLiveData();
                    break;
            }
        }

        private WeatherChartData CreateChartPoint(DhtData dhtData, DateTime date) => new WeatherChartData
        {
            Date = date,
            Humidity = dhtData.Humidity,
            Temperature = dhtData.Temperature,
            HumidityColor = Color.LightBlue,
            TemperatureColor = Color.IndianRed
        };


        private RelayChartData CreateRelayChartPoint(RelayData relayData, DateTime date) => new RelayChartData
        {
            Date = date,
            Pin = relayData.Pin,
            State = relayData.State
        };



        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (Module != null)
                    {
                        Module.Unsubscribe();
                        Module.VisualStateChanged -= ModuleOnVisualStateChanged;
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }


    public class WeatherChartData
    {
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }

        public string TemperatureText => $"{Temperature} °C";
        public string HumidityText => $"{Humidity} %";

        public Color HumidityColor { get; set; }
        public Color TemperatureColor { get; set; }
    }

    public class RelayChartData
    {
        public DateTime Date { get; set; }
        public RelayState State { get; set; }
        public int Pin { get; set; }
        public Color StateColor
        {
            get
            {
                switch (State)
                {
                    case RelayState.Opened:
                        return Color.Green;
                    case RelayState.Closed:
                        return Color.Red;
                    default: return Color.Red;
                }
            }
        }
        public string Description
        {
            get
            {
                switch (State)
                {
                    case RelayState.Opened:
                        return "ON";
                    case RelayState.Closed:
                        return "OFF";
                    default: return "OFF";
                }
            }
        }
    }
}
