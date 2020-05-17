using System;
using System.Threading.Tasks;
using FourTwenty.IoT.Server.Interfaces;
using GrowIoT.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GrowIoT.Extensions
{
    public static class GrowInitExtensions
    {
        public static IApplicationBuilder AddIoT(this IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider, ILogger logger)
        {
            serviceProvider.InitIoT(logger).ConfigureAwait(false).GetAwaiter().GetResult();
            logger.LogInformation("IOT initialization finished");
            return applicationBuilder;
        }
        private static async Task InitIoT(this IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                var growBox = await serviceProvider.GetService<IGrowboxManager>().GetBoxWithRules();
                var configService = serviceProvider.GetService<IIoTConfigService>();
                var historyService = serviceProvider.GetService<IHistoryService>();
                await configService.Initialize(growBox);
                await historyService.Initialize(configService.GetModules());
                var jobsService = serviceProvider.GetService<IJobsService>();
                await jobsService.StartJobs(configService.GetModules());
                logger.LogInformation($"{nameof(InitIoT)} Jobs started");

            }
            catch (Exception e)
            {
                logger.LogCritical(e, nameof(InitIoT));
            }
        }
    }
}
