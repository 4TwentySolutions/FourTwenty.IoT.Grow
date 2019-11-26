using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Connect.Models.Config;

namespace GrowIoT.Interfaces
{
    public interface IIotConfigService
    {
        void InitConfig(GpioController controller, ConfigModel config = null);

        Task<ConfigModel> GetConfig();
    }
}
