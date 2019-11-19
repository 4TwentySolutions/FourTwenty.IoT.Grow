﻿using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GrowIoT.Enums;
using GrowIoT.Interfaces;
using GrowIoT.Models;
using Newtonsoft.Json;

namespace GrowIoT.Modules
{
    public abstract class BaseModule : IModule
    {
        [JsonIgnore]
        public GpioController Controller;
        public string Name { get; set; }
        public List<int> Pins { get; set; }
        public ModuleType Type { get; protected set; }
        public List<ModuleRule> Rules { get; set; }

        protected BaseModule(List<ModuleRule> rules = null, string name = null)
        {
            Rules = rules;
            Name = name;
        }

        public virtual void Init(GpioController controller)
        {
            Debug.WriteLine($"--- Module {Name}: Initialized ---");
            Controller = controller;
        }

        public virtual Task<BaseModuleResponse> ReadData()
        {
            return Task.FromResult(new BaseModuleResponse()
            {
                IsSuccess = true,
                Message = "base module result"
            });
        }

        /// <summary>
        /// Toggle module gpio value
        /// </summary>
        /// <param name="gpioPin">pin number</param>
        public virtual void Toggle(int? gpioPin = null)
        {
            if (Pins != null && Pins.Count > 0)
            {
                var relayPin = gpioPin.HasValue ? Pins.FirstOrDefault(x => x == gpioPin) : Pins.FirstOrDefault();

                if (relayPin > 0)
                {
                    var val = Controller.Read(relayPin);
                    val = val == PinValue.High ? PinValue.Low : PinValue.High;
                    Debug.WriteLine($"--- Relay {relayPin}: set to {val.ToString()}");
                    Controller.SetPinMode(relayPin, PinMode.Input);
                    Controller.Write(relayPin, val);
                    Controller.SetPinMode(relayPin, PinMode.Output);
                }
            }
        }

        /// <summary>
        /// Set module gpio value (Low - on , High - off)
        /// </summary>
        /// <param name="value">Low - on , High - off</param>
        /// <param name="gpioPin">pin number</param>
        public virtual void SetValue(PinValue value, int? gpioPin = null)
        {
            if (Pins != null && Pins.Count > 0)
            {
                var relayPin = gpioPin.HasValue ? Pins.FirstOrDefault(x => x == gpioPin) : Pins.FirstOrDefault();

                if (relayPin > 0)
                {
                    if (Controller.GetPinMode(relayPin) == PinMode.Output)
                        Controller.SetPinMode(relayPin, PinMode.Input);

                    var curValue = Controller.Read(relayPin);
                    if (curValue == value)
                        return;

                    Debug.WriteLine($"--- Pin {relayPin}: set to {value.ToString()}");
                    Controller.Write(relayPin, value);
                }
            }

        }
    }
}
