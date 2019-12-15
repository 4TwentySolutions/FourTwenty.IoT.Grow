using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using GrowIoT.Modules;

namespace GrowIoT.Interfaces
{
    public interface IJobsService
    {
        Task Init();
        Task StartJobs(IList<IoTBaseModule> modules);
        Task StopJobs();
    }
}
