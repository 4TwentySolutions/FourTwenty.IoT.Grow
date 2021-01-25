

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Models;
using GrowIoT.Models.ChartData;

namespace GrowIoT.Extensions
{
    public static class ModuleExtensions
    {
	    #region Chart point creation

	    public static WeatherChartData CreateChartPoint(DhtData dhtData, DateTime date) => new WeatherChartData
	    {
		    Date = date,
		    Humidity = dhtData.Humidity,
		    Temperature = dhtData.Temperature,
		    HumidityColor = Color.LightBlue,
		    TemperatureColor = Color.IndianRed
	    };

	    public static RelayChartData CreateRelayChartPoint(RelayData relayData, DateTime date) => new RelayChartData
	    {
		    Date = date,
		    Pin = relayData.Pin,
		    State = relayData.State
	    };

	    public static RangeFinderChartData CreateRangeFinderChartPoint(RangeFinderData data, DateTime date) => new RangeFinderChartData()
	    {
		    Date = date,
		    Distance = data.Distance
	    };

	    #endregion

	    public static async Task Execute(this IReadOnlyCollection<IRule> rules)
	    {
		    foreach (var rule in rules)
		    {
			    await rule.Execute();
		    }
	    }

        //public static TwoRelayModule SetRelayValue(this TwoRelayModule currentModule, GpioPin gpioPin, GpioPinValue value)
        //{
        //    var relayPin = gpioPin;//currentModule.Pins.FirstOrDefault(x => x.PinNumber == gpioPin);
        //    if (relayPin != null)
        //    {
        //        Debug.WriteLine($"--- Relay {relayPin.PinNumber}: set to {value.ToString()}");
        //        relayPin.SetDriveMode(GpioPinDriveMode.Input);
        //        relayPin.Write(value);
        //        relayPin.SetDriveMode(GpioPinDriveMode.Output);
        //    }

        //    return currentModule;
        //}
    }
}
