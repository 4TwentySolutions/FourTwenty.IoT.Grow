using System.Collections.Generic;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;

namespace GrowIoT.Models.Tests
{
    public class MockModule : IoTComponent
    {
        public MockModule(IReadOnlyCollection<int> pins) : base(pins, null)
        {
        }

        public MockModule(IReadOnlyCollection<IRule> rules, IReadOnlyCollection<int> pins) : base(rules, pins, null)
        {
        }
    }
}
