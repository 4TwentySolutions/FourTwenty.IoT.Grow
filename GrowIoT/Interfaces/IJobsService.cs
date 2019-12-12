using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;

namespace GrowIoT.Interfaces
{
    public interface IJobsService
    {
        Task Init();
        Task StartJobs(ConfigDto config);
        Task StopJobs();
    }
}
