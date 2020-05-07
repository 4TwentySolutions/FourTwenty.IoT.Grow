using System;
using FourTwenty.IoT.Connect.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FourTwenty.IoT.Connect.Entities;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components.Sensors;

namespace GrowIoT.ViewModels
{
    public class ModuleVm : EntityViewModel<GrowBoxModule>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ModuleType Type { get; set; }
        public int[] Pins { get; set; }
        public int GrowBoxId { get; set; }
        public GrowBoxViewModel GrowBox { get; set; }
        public ICollection<ModuleRuleVm> Rules { get; set; }

        private string _currentValue;
        public string CurrentValue
        {
            get => _currentValue;
            set => _currentValue = value;
        }


        public event EventHandler<SensorEventArgs> DataReceived;
        public event EventHandler<RelayEventArgs> StateChanged;


        private ISensor _sensor;
        public ISensor Sensor
        {
            get => _sensor;
            set
            {
                _sensor = value;
                if (_sensor != null)
                {
                    _sensor.DataReceived += SensorOnDataReceived;
                }
            }
        }

        private IRelay _relay;
        public IRelay Relay
        {
            get => _relay;
            set
            {
                _relay = value;
                if (_relay != null)
                {
                    _relay.StateChanged += RelayOnStateChanged;
                }
            }
        }

        private void RelayOnStateChanged(object? sender, RelayEventArgs e)
        {
            switch (Type)
            {
                case ModuleType.Relay:
                    {
                        CurrentValue = Relay.States.Aggregate("", (current, relayPin) =>
                           // current + $"State {relayPin.Key}: {(relayPin.Value == RelayState.Opened ? "<b class='on-relay-state'>ON</b>" : "<b class='off-relay-state'>OFF</b>")} <br/>");
                           current + $"Pin {relayPin.Key}: {(relayPin.Value == RelayState.Opened ? "<i class='fas fa-toggle-on'></i>" : "<i class='fas fa-toggle-off'></i>")} <br/>");

                        break;
                    }
            }

            OnStateChanged(e);
        }


        private void SensorOnDataReceived(object? sender, SensorEventArgs e)
        {
            switch (Type)
            {
                case ModuleType.HumidityAndTemperature:
                    {
                        if (e.Data is DhtData moduleData && !double.IsNaN(moduleData.Temperature))
                        {
                            CurrentValue = $"<b><i class='fas fa-thermometer-half'></i>{moduleData.Temperature}&#8451;</b><br/><b><i class='fas fa-tint'></i>{moduleData.Humidity}%</b>";
                        }


                        break;
                    }
            }

            OnDataReceived(e);
        }

        protected virtual void OnDataReceived(SensorEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        protected virtual void OnStateChanged(RelayEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}
