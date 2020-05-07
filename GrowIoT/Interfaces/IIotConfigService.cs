using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Interfaces;
using GrowIoT.ViewModels;
using Quartz;

namespace GrowIoT.Interfaces
{
    public interface IIoTConfigService : IInitializableService
    {
        ConfigDto GetConfig();
        IList<IModule> GetModules();
        IModule GetModule(int id);
        Task<long> UpdateConfig(ConfigDto model);
    }
}
