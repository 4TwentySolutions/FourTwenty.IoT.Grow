using System;
using FourTwenty.IoT.Connect.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components.Sensors;

namespace GrowIoT.ViewModels
{
    public class ModuleVm
    {
        public Guid Id{get;set;}
        [Required]
        public string Name { get; set; }
        public ModuleType Type { get; set; }
        public int[] Pins { get; set; }
        public int GrowBoxId { get; set; }
        public GrowBoxViewModel GrowBox { get; set; }
        public ICollection<ModuleRuleVm> Rules { get; set; }

        public string CurrentValue { get; set; }


        private ISensor _sensor;
        public ISensor Sensor
        {
            get => _sensor;
            set
            {
                _sensor = value;
                if (_sensor != null)
                {
                    _sensor.DataReceived +=SensorOnDataReceived;
                }
            }
        }

        private void SensorOnDataReceived(object? sender, SensorEventArgs e)
        {
            switch (Type)
            {
                case ModuleType.HumidityAndTemperature:
                {
                    var moduleData = e.Data as DhtData;
                    CurrentValue = $"Temperature - {moduleData?.Temperature.Celsius} \n Humidity - {moduleData?.Humidity}";
                    break;
                }
                case ModuleType.Humidity:
                    break;
                case ModuleType.Temperature:
                    break;
                case ModuleType.Relay:
                    break;
                case ModuleType.TwoRelay:
                    break;
                case ModuleType.Fan:
                    break;
                case ModuleType.Light:
                    break;
                case ModuleType.WaterPump:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
