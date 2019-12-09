using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;

namespace GrowIoT.Interfaces
{
    public interface IIoTConfigService
    {
        void InitConfig(GpioController controller, ConfigDto config = null);
        Task<ConfigDto> GetConfig();
        Task<long> UpdateConfig(ConfigDto model);
    }
}
