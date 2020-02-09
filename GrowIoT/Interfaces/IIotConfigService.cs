using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Server.Components;
using GrowIoT.ViewModels;

namespace GrowIoT.Interfaces
{
    public interface IIoTConfigService
    {
        Task<ConfigDto> LoadConfig();
        void InitConfig(GpioController controller = null, GrowBoxViewModel config = null);
        ConfigDto GetConfig();
        IList<IoTComponent> GetModules();
        Task<long> UpdateConfig(ConfigDto model);
    }
}
