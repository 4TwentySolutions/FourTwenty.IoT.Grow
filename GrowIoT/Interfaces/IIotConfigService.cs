using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Interfaces;
using GrowIoT.ViewModels;
using Quartz;

namespace GrowIoT.Interfaces
{
    public interface IIoTConfigService
    {
        //Task<ConfigDto> LoadConfig();
        void InitConfig(IScheduler scheduler, GrowBoxViewModel config, GpioController controller = null);
        ConfigDto GetConfig();
        IList<IModule> GetModules();
        Task<long> UpdateConfig(ConfigDto model);
    }
}
