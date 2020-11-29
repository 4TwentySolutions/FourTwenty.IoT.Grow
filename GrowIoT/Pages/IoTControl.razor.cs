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
using FourTwenty.IoT.Server.Models;

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
        public ObservableCollection<RangeFinderChartData> RangeFinderHistoryData { get; set; } = new ObservableCollection<RangeFinderChartData>();

        public double Maximum { get; set; }
        public double Minimum { get; set; }

        public ChartSeriesType ChartType { get; set; } = ChartSeriesType.Line;

        public Syncfusion.Blazor.Charts.ValueType YAxisType { get; set; } = Syncfusion.Blazor.Charts.ValueType.Double;

        protected SfChart HistoryChart;

        protected override async Task OnInitializedAsync()
        {
            if (_isInitialized || !Id.HasValue)
                return;

            Module = await BoxManager.GetModule(Id.Value);

            if (Module.Type == ModuleType.HumidityAndTemperature)
            {
                var moduleHistory = (await HistoryService.GetModuleHistory(Module.Id, DateTime.Now.AddHours(-2), 50)).ToList();
                TemperatureHistoryData = GetHistoryData<WeatherChartData, DhtData>(moduleHistory, CreateChartPoint);
            }

            if (Module.Type == ModuleType.RangeFinder)
            {
                var moduleHistory = (await HistoryService.GetModuleHistory(Module.Id, DateTime.Now.AddHours(-2), 50)).ToList();
                RangeFinderHistoryData = GetHistoryData<RangeFinderChartData, RangeFinderData>(moduleHistory, CreateRangeFinderChartPoint);
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

            Maximum = RefreshMaximum();
            Minimum = RefreshMinimum();

            await base.OnInitializedAsync();

            Module.Subscribe();
            Module.VisualStateChanged += ModuleOnVisualStateChanged;
            _isInitialized = true;
        }

        private double RefreshMaximum()
        {
            if (Module == null) 
                return 0d;

            switch (Module.Type)
            {
                case ModuleType.Temperature:
                case ModuleType.HumidityAndTemperature:
                    return TemperatureHistoryData != null && TemperatureHistoryData.Any() ? TemperatureHistoryData.Max(d => Math.Max(d.Humidity, d.Temperature)) + 5 : 5;
                case ModuleType.Relay:
                    return Module.Pins.Max();
                case ModuleType.RangeFinder:
                    return RangeFinderHistoryData != null && RangeFinderHistoryData.Any() ? RangeFinderHistoryData.Max(d => d.Distance) + 5 : 20;
                default:
                    return 0d;
            }
        }


        private double RefreshMinimum()
        {
            if (Module == null) 
                return 0d;

            switch (Module.Type)
            {
                case ModuleType.Temperature:
                case ModuleType.HumidityAndTemperature:
                    return TemperatureHistoryData != null && TemperatureHistoryData.Any() ? TemperatureHistoryData.Min(d => Math.Min(d.Humidity, d.Temperature)) - 5 : -5;
                case ModuleType.Relay:
                    return 0d;
                case ModuleType.RangeFinder:
                    return RangeFinderHistoryData != null && RangeFinderHistoryData.Any() ? RangeFinderHistoryData.Min(d => d.Distance) : 0;
                default:
                   return 0d;
            }

        }



        private ObservableCollection<T> GetHistoryData<T, K>(List<ModuleHistoryItem> moduleHistory, Func<K, DateTime, T> createItemAction)
        {
            var historyData = new List<T>();
            foreach (var item in moduleHistory)
            {
                var itemData = JsonConvert.DeserializeObject<K>(item.Data);
                historyData.Add(createItemAction.Invoke(itemData, item.Date));
            }
            return new ObservableCollection<T>(historyData);
        }

        private void ModuleOnVisualStateChanged(object? sender, VisualStateEventArgs e)
        {
            if (!(sender is ModuleVm module) || module != Module || HistoryChart == null) return;

            switch (module.Type)
            {
                case ModuleType.Temperature:
                case ModuleType.HumidityAndTemperature:
                case ModuleType.RangeFinder:
                    ModuleOnDataReceived(module.CurrentRawValue);

                    
                    break;
                case ModuleType.Relay:
                    ModuleOnStateChanged(module.CurrentRawValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Maximum = RefreshMaximum();
            Minimum = RefreshMinimum();

            HistoryChart.RefreshLiveData();
            StateHasChanged();

        }

        private void ModuleOnStateChanged(object data)
        {

            switch (Module.Type)
            {
                case ModuleType.Relay:
                    if (!(data is RelayData relayData) || !Module.Pins.Contains(relayData.Pin)) 
                        return;

                    if (HistoryData[relayData.Pin].Count > 0)
                        HistoryData[relayData.Pin].RemoveAt(0);
                    HistoryData[relayData.Pin].Add(CreateRelayChartPoint(relayData, DateTime.Now));
                    break;
            }

        }

        private void ModuleOnDataReceived(object data)
        {
            switch (Module.Type)
            {
                case ModuleType.Temperature:
                case ModuleType.HumidityAndTemperature:
                    if (!(data is DhtData dhtData))
                        return;

                    if (TemperatureHistoryData.Count > 0)
                        TemperatureHistoryData.RemoveAt(0);
                    TemperatureHistoryData.Add(CreateChartPoint(dhtData, DateTime.Now));                    
                    break;

                case ModuleType.RangeFinder:
                        if (!(data is RangeFinderData rfData)) 
                        return;

                    if (RangeFinderHistoryData.Count > 0)
                        RangeFinderHistoryData.RemoveAt(0);
                    RangeFinderHistoryData.Add(CreateRangeFinderChartPoint(rfData, DateTime.Now));
                    break;
            }
        }

        #region Chart point creation

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

        private RangeFinderChartData CreateRangeFinderChartPoint(RangeFinderData data, DateTime date) => new RangeFinderChartData()
        {
            Date = date,
            Distance = data.Distance
        };

        #endregion

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

    #region Chart data models

    public class RangeFinderChartData
    {
        public DateTime Date { get; set; }
        public double Distance { get; set; }

        public string DistanceText => Distance.ToString();
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

    #endregion

}
