using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;

namespace GrowIoT.Models.Tests
{
    public class MockModule : IoTComponent, ISensor
    {
        public MockModule(IReadOnlyCollection<int> pins) : base(pins, null)
        {
        }

        public MockModule(IReadOnlyCollection<IRule> rules, IReadOnlyCollection<int> pins) : base(rules, pins, null)
        {
        }

        public event EventHandler<SensorEventArgs> DataReceived;

        public ValueTask<object> GetData()
        {
            DataReceived?.Invoke(this, new SensorEventArgs(-1));
            return new ValueTask<object>(-1);
        }
    }
}
