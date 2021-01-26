using System;
using FourTwenty.IoT.Connect.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FourTwenty.IoT.Connect.Entities;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Connect.Models;
using FourTwenty.IoT.Server.Components.Sensors;
using GrowIoT.Common;
using FourTwenty.IoT.Server.Models;

namespace GrowIoT.ViewModels
{
	public class ModuleVm : EntityViewModel<GrowBoxModule>, IDisposable
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public ModuleType Type { get; set; }
		public int[] Pins { get; set; }
		public int GrowBoxId { get; set; }
		public GrowBoxViewModel GrowBox { get; set; }
		public ICollection<ModuleRuleVm> Rules { get; set; }
		public string CurrentValueString { get; set; }
		public object CurrentRawValue { get; set; }
		public IModule IotModule { get; set; }
		public AdditionalData DisplayData { get; set; } = new AdditionalData();

		public ISensor Sensor => IotModule as ISensor;
		public IRelay Relay => IotModule as IRelay;

		#region States

		public WorkState State => IotModule?.RulesWorkState ?? WorkState.Stopped;

		public string StateIcon => State == WorkState.Running ? "far fa-pause-circle" :
			State == WorkState.Paused ? "far fa-play-circle" : State == WorkState.Stopped ? "far fa-play-circle" : string.Empty;

		public string BadgeIcon => State == WorkState.Running ? "badge-success" :
			State == WorkState.Paused ? "badge-warning" :
			State == WorkState.Stopped ? "badge-danger" : "badge-secondary";

		#endregion

		#region Events subscription

		public void Subscribe()
		{

			if (Sensor != null)
				Sensor.DataReceived += OnDataReceived;
			if (Relay != null)
				Relay.StateChanged += OnDataReceived;
		}

		public void Unsubscribe()
		{
			if (Sensor != null)
				Sensor.DataReceived -= OnDataReceived;
			if (Relay != null)
				Relay.StateChanged -= OnDataReceived;
		}


		public event EventHandler<VisualStateEventArgs> VisualStateChanged;
		
		private void OnDataReceived(object? sender, ModuleResponseEventArgs e)
		{
			CurrentRawValue = e.Data;
			switch (Type)
			{
				case ModuleType.HumidityAndTemperature:
					{
						if (e.Data is DhtData moduleData && !double.IsNaN(moduleData.Temperature))
						{
							CurrentValueString = $"<b><i class='fas fa-thermometer-half'></i>{moduleData.Temperature}&#8451;</b><br/><b><i class='fas fa-tint'></i>{moduleData.Humidity}%</b>";
						}

						break;
					}
				case ModuleType.RangeFinder:
					{
						if (e.Data is RangeFinderData moduleData && !double.IsNaN(moduleData.Distance))
						{
							CurrentValueString = $"<b><i class='fas fa-arrows-alt-v'></i>{moduleData.Distance}</b>";
						}

						break;
					}

				case ModuleType.WaterTank:
				{
					if (e.Data is WaterTankData moduleData && !double.IsNaN(moduleData.Value))
					{
						CurrentValueString = $"<b><i class='fas fa-water'></i>{moduleData.ValueLine}</b>";
					}

					break;
				}

				case ModuleType.Relay:
				{
					CurrentValueString = Relay.States.Aggregate("", (current, relayPin) =>
						current + $"Pin {relayPin.Key}: {(relayPin.Value == RelayState.Opened ? "<i class='fas fa-toggle-on'></i>" : "<i class='fas fa-toggle-off'></i>")} <br/>");

					break;
				}
			}

			VisualStateChanged?.Invoke(this, new VisualStateEventArgs());
		}
		#endregion

		#region IDisposable Support
		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue) return;
			if (disposing)
			{
				Unsubscribe();
			}


			_disposedValue = true;
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
