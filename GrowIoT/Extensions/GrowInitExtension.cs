using System;
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
            logger.LogInformation("DB init");
            serviceProvider.GetService<IGrowDataContext>().InitDb().ConfigureAwait(false).GetAwaiter().GetResult();
            logger.LogInformation("DB init finished\nStarting IOT initialization");
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

                logger.LogInformation($"{nameof(InitIoT)} Iot config init");
                configService.InitConfig(scheduler, growBox);
                logger.LogInformation($"{nameof(InitIoT)} Iot config init finished");
                await jobsService.StartJobs(configService.GetModules());
                logger.LogInformation($"{nameof(InitIoT)} Jobs started");

                //using GpioController controller = new GpioController(PinNumberingScheme.Logical);
                //configService.InitConfig(scheduler, growBox, controller);
                //await jobsService.StartJobs(configService.GetModules());
            }
            catch (Exception e)
            {
                logger.LogCritical(e, nameof(InitIoT));
            }
        }
    }
}
