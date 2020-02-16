using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Interfaces;
using GrowIoT.ViewModels;

namespace GrowIoT.Interfaces
{
    public interface IIoTConfigService
    {
        Task<ConfigDto> LoadConfig();
        void InitConfig(GpioController controller = null, GrowBoxViewModel config = null);
        ConfigDto GetConfig();
        IList<IModule> GetModules();
        Task<long> UpdateConfig(ConfigDto model);
    }
}
