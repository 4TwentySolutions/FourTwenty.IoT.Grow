using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Server.Interfaces;
using GrowIoT.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz.Impl;

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

                var configService = serviceProvider.GetService<IIoTConfigService>();
                var jobsService = serviceProvider.GetService<IJobsService>();

                var growBoxManager = serviceProvider.GetService<IGrowboxManager>();
                var growBox = await growBoxManager.GetBoxWithRules();

                logger.LogInformation($"{nameof(InitIoT)} GrowBox name - {growBox.Title}");
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

#if DebugLocalWin
                logger.LogInformation($"{nameof(InitIoT)} Iot config init");
                configService.InitConfig(scheduler, growBox);
                logger.LogInformation($"{nameof(InitIoT)} Iot config init finished");
                await jobsService.StartJobs(configService.GetModules());
                logger.LogInformation($"{nameof(InitIoT)} Jobs started");
#else
                using GpioController controller = new GpioController(PinNumberingScheme.Logical);
                logger.LogInformation($"{nameof(InitIoT)} Iot config init");
                configService.InitConfig(scheduler, growBox, controller);
                logger.LogInformation($"{nameof(InitIoT)} Iot config init finished");
                await jobsService.StartJobs(configService.GetModules());
                logger.LogInformation($"{nameof(InitIoT)} Jobs started");
#endif
            }
            catch (Exception e)
            {
                logger.LogCritical(e, nameof(InitIoT));
            }
        }
    }
}
