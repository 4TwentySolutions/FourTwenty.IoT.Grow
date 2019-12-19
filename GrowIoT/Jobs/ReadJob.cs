using System.Diagnostics;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Models;
using GrowIoT.Hubs;
using GrowIoT.Interfaces;
using GrowIoT.Modules;
using GrowIoT.Modules.Sensors;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
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


            if (module != null && rule != null)
            {
                var readResult = await module.ReadData();
                if (readResult is ModuleDataResponse<DthData> dhtResult)
                {
                    if (hub != null)
                        await hub.SendMessage("ReceiveMessage", JsonConvert.SerializeObject(readResult));

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
