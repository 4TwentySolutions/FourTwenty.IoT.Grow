using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using FourTwenty.IoT.Connect.Constants;

namespace GrowIoT.Models.ChartData
{
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
}
