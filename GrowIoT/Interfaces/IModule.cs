using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces.Modules;
using FourTwenty.IoT.Connect.Models;
using GrowIoT.Modules;

namespace GrowIoT.Interfaces
{
    public interface IIoTModule : IModule
    {
        void Init(GpioController controller);
        Task<BaseModuleResponse> ReadData();
        void SetValue(PinValue value,int? gpioPin = null);
        void Toggle(int? gpioPin = null);
    }
}
