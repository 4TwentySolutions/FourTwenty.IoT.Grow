using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using GrowIoT.Enums;
using GrowIoT.Models;
using GrowIoT.Modules;

namespace GrowIoT.Interfaces
{
    public interface IModule
    {
        List<int> Pins { get; }
        ModuleType Type { get; }
        List<ModuleRule> Rules { get; set; }

        void Init(GpioController controller);
        Task<BaseModuleResponse> ReadData();
        void SetValue(PinValue value,int? gpioPin = null);
        void Toggle(int? gpioPin = null);
    }
}
