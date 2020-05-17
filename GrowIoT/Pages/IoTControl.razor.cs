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

        public ObservableCollection<ChartData> HistoryData { get; set; } = new ObservableCollection<ChartData>();

        public double Maximum => HistoryData != null ? HistoryData.Max(d => Math.Max(d.Y, d.Y2)) + 5 : 5;
        public double Minimum => HistoryData != null ? HistoryData.Min(d => Math.Min(d.Y, d.Y2)) - 5 : -5;

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
            Module.Subscribe();
            Module.VisualStateChanged += ModuleOnVisualStateChanged;

            var moduleHistory = (await HistoryService.GetModuleHistory(Module.Id)).TakeLast(50).ToList();
            var historyData = new List<ChartData>();

            if (Module.Type == ModuleType.HumidityAndTemperature)
            {
                foreach (var item in moduleHistory)
                {
                    var itemData = JsonConvert.DeserializeObject<DhtData>(item.Data);
                    historyData.Add(CreateChartPoint(itemData, item.Date));
                }
            }

            if (Module.Type == ModuleType.Relay)
            {
                ChartType = ChartSeriesType.Line;
                YAxisType = Syncfusion.Blazor.Charts.ValueType.Category;
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
                        X = item.Key
                    };

                    var it = item.FirstOrDefault(x => x.data.Pin == Module.Pins.FirstOrDefault());
                    if (it.data != null)
                    {
                        historyItem.Y = it.data.Pin;
                        historyItem.ColorY = it.data.State == RelayState.Opened ? Color.Green : Color.Red;
                        historyItem.TextY = it.data.State == RelayState.Opened ? "ON" : "OFF";
                    }


                    var it2 = item.FirstOrDefault(x => x.data.Pin == Module.Pins.LastOrDefault());
                    if (it2.data != null)
                    {
                        historyItem.Y2 = it2.data.Pin;
                        historyItem.ColorY2 = it2.data.State == RelayState.Opened ? Color.Green : Color.Red;
                        historyItem.TextY2 = it2.data.State == RelayState.Opened ? "ON" : "OFF";
                    }

                    historyData.Add(historyItem);
                }
            }

            HistoryData = new ObservableCollection<ChartData>(historyData);

            await base.OnInitializedAsync();
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
            if (!(data is RelayData relayData)) return;
            if (HistoryData.Count > 0)
                HistoryData.RemoveAt(0);
            HistoryData.Add(CreateRelayChartPoint(relayData, DateTime.Now));
            HistoryChart.RefreshLiveData();
        }

        private void ModuleOnDataReceived(object data)
        {

            if (!(data is DhtData dhtData)) return;
            if (HistoryData.Count > 0)
                HistoryData.RemoveAt(0);
            HistoryData.Add(CreateChartPoint(dhtData, DateTime.Now));
            HistoryChart.RefreshLiveData();
        }

        private ChartData CreateChartPoint(DhtData dhtData, DateTime date)
        {
            return new ChartData
            {
                X = date,
                Y = dhtData.Humidity,
                Y2 = dhtData.Temperature,
                ColorY = Color.LightBlue,
                ColorY2 = Color.IndianRed,
                TextY = dhtData.Humidity + "%",
                TextY2 = dhtData.Temperature + "°C",
                FillColorY = ColorTranslator.ToHtml(Color.LightBlue),
                FillColorY2 = ColorTranslator.ToHtml(Color.LightYellow)
            };
        }

        private ChartData CreateRelayChartPoint(RelayData relayData, DateTime date)
        {
            var historyItem = new ChartData
            {
                X = date,
                Y = Module.Pins.FirstOrDefault(),
                Y2 = Module.Pins.LastOrDefault()
            };

            if (relayData.Pin == Module.Pins.FirstOrDefault())
            {
                historyItem.ColorY = relayData.State == RelayState.Opened ? Color.Green : Color.Red;
                historyItem.TextY = relayData.State == RelayState.Opened ? "ON" : "OFF";
            }

            if (relayData.Pin == Module.Pins.LastOrDefault())
            {
                historyItem.ColorY2 = relayData.State == RelayState.Opened ? Color.Green : Color.Red;
                historyItem.TextY2 = relayData.State == RelayState.Opened ? "ON" : "OFF";
            }
            return historyItem;
        }

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


    public class ChartData
    {
        public DateTime X;
        public double Y;
        public double Y2;
        public Color ColorY;
        public Color ColorY2;
        public string FillColorY;
        public string FillColorY2;
        public string TextY;
        public string TextY2;
    }
}
