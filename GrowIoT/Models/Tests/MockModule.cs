using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;

namespace GrowIoT.Models.Tests
{
    public class MockModule : IoTComponent, ISensor
    {
        private readonly Random _random = new Random(123);
        public MockModule(IReadOnlyCollection<int> pins) : base(pins, null)
        {
        }

        public MockModule(IReadOnlyCollection<IRule> rules, IReadOnlyCollection<int> pins) : base(rules, pins, null)
        {
        }

        public event EventHandler<SensorEventArgs> DataReceived;

        public ValueTask<object> GetData()
        {
            var val = _random.Next();
            DataReceived?.Invoke(this, new SensorEventArgs(val));
            return new ValueTask<object>(val);
        }
    }
}
