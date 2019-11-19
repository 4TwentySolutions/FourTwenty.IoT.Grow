using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowIoT.Enums;

namespace GrowIoT.Interfaces.Sensors
{
    public interface IDefaultSensor : IModule
    {
    }

    public interface ISensor<T> : IDefaultSensor
    {
        Task<T> GetData();
    }
}
