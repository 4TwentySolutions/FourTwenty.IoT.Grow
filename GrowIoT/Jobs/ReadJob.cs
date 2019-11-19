using System.Diagnostics;
using System.Threading.Tasks;
using GrowIoT.Models;
using GrowIoT.Modules;
using GrowIoT.Modules.Sensors;
using Quartz;

namespace GrowIoT.Jobs
{
    public class ReadJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var module = (BaseModule)dataMap["module"];
            var rule = (ModuleRule)dataMap["rule"];

            if (module != null && rule != null)
            {
                var readResult = await module.ReadData();
                if (readResult is ModuleResponse<DthData> dhtResult)
                {
                    if (dhtResult.IsSuccess)
                    {
                        Debug.Write("\n--- Dht Sensor Data ---\n");
                        Debug.WriteLine($"--- Temperature: {dhtResult.Data.Temperature} ---");
                        Debug.WriteLine($"--- Humidity: {dhtResult.Data.Humidity} ---");
                    }
                    else
                    {
                        Debug.Write("\n--- Dht Sensor Error ---\n");
                        Debug.WriteLine($"--- Error: {dhtResult.Message} ---");
                    }
                }
            }
        }
    }
}
