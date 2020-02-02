using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Models;
using GrowIoT.Interfaces;
using GrowIoT.Modules;
using GrowIoT.Modules.Sensors;
using Quartz;

namespace GrowIoT.Jobs
{
    public class ReadJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {

            var dataMap = context.JobDetail.JobDataMap;
            var module = (IoTBaseModule)dataMap["module"];
            var rule = (ModuleRuleDto)dataMap["rule"];
            var hub = (IHubService)dataMap[nameof(IHubService)];

            if (module == null || rule == null || hub == null)
                return;

            var readResult = await module.ReadData();
            if (readResult is ModuleResponse<DthData> dhtResult)
            {
                await hub.SendMessage(module.Name, dhtResult);
            }
        }
    }
}
