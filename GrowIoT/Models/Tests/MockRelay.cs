using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;

namespace GrowIoT.Models.Tests
{
    public class MockRelay : IoTComponent, IRelay
    {
        public IDictionary<int, RelayState> States { get; }

        public event EventHandler<RelayEventArgs> StateChanged;

        public MockRelay(IReadOnlyCollection<int> pins) : base(pins, null)
        {
            States = new Dictionary<int, RelayState>(pins.Select(x => new KeyValuePair<int, RelayState>(x, RelayState.Closed)));
        }

        public MockRelay(IReadOnlyCollection<IRule> rules, IReadOnlyCollection<int> pins) : base(rules, pins, null)
        {
            States = new Dictionary<int, RelayState>(pins.Select(x => new KeyValuePair<int, RelayState>(x, RelayState.Closed)));
        }

        public override void SetValue(PinValue value, int pin)
        {
            if (!Pins.Contains(pin))
                return;

            States[pin] = value == PinValue.Low ? RelayState.Opened : RelayState.Closed;
            StateChanged?.Invoke(this, new RelayEventArgs(new RelayData(pin, States[pin])));
        }
    }
}
