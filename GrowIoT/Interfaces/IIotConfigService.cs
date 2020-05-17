using System.Collections.Generic;
using System.Device.Gpio;
using FourTwenty.IoT.Connect.Interfaces;
using GrowIoT.ViewModels;

namespace GrowIoT.Interfaces
{
    public interface IIoTConfigService : IInitializeService<GrowBoxViewModel>
    {
        IList<IModule> GetModules();
        GpioController Gpio { get; }
        IModule GetModule(int id);
        
    }
}
