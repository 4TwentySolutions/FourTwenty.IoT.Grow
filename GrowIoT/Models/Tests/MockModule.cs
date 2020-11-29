using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;
using FourTwenty.IoT.Server.Models;

namespace GrowIoT.Models.Tests
{
    public class MockModule : IoTComponent, ISensor
    {
        private static Random _random;
        private static Random Random => _random ??= new Random(123);

        public MockModule(IReadOnlyCollection<int> pins) : base(pins, null) { }
        public MockModule(IReadOnlyCollection<IRule> rules, IReadOnlyCollection<int> pins) : base(rules, pins, null) { }

        public event EventHandler<SensorEventArgs> DataReceived;

        public ValueTask<object> GetData()
        {
            var val = new DhtData(Random.Next(-10, 45), Random.Next(20, 80));
            DataReceived?.Invoke(this, new SensorEventArgs(val));
            return new ValueTask<object>(val);
        }
    }
}
