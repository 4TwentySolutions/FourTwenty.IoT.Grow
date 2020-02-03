using System;
using System.Device.Gpio;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using AutoMapper;
using FourTwenty.Core.Data.Interfaces;
using FourTwenty.IoT.Server.Hubs;
using FourTwenty.IoT.Server.Interfaces;
using FourTwenty.IoT.Server.Services;
using GrowIoT.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrowIoT.Interfaces;
using GrowIoT.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GrowIoT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISqLiteProvider, SqLiteProvider>();
            services.AddDbContextPool<GrowDbContext>((provider, builder) => builder.UseSqlite(provider.GetService<ISqLiteProvider>().GetConnection()));
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR();
            services.AddAutoMapper(GetType());

            services.AddSingleton<IIoTConfigService, IoTConfigService>();
            services.AddSingleton<IJobsService, JobsService>();
            services.AddSingleton<IHubService, HubService>();

            services.AddScoped(typeof(IRepository<,>), typeof(DataRepository<,>));
            services.AddScoped(typeof(IAsyncRepository<,>), typeof(DataRepository<,>));
            services.AddScoped(typeof(IRepository<>), typeof(DataRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(DataRepository<>));

            services.AddHealthChecks().AddCheck("ping", () =>
            {
                try
                {
                    using var ping = new Ping();
                    var reply = ping.Send("www.google.com");

                    if (reply == null)
                        return HealthCheckResult.Unhealthy();

                    return reply.Status != IPStatus.Success ? HealthCheckResult.Unhealthy() :
                        reply.RoundtripTime > 100 ? HealthCheckResult.Degraded() : HealthCheckResult.Healthy();
                }
                catch
                {
                    return HealthCheckResult.Unhealthy();
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHealthChecks("/ping");
            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<RealTimeDataHub>("/commandsHub");
                endpoints.MapFallbackToPage("/_Host");
            });
            await serviceProvider.GetService<GrowDbContext>().InitDb();
           // await InitIoT(serviceProvider);
        }

        private static async Task InitIoT(IServiceProvider serviceProvider)
        {
            try
            {

                var configService = serviceProvider.GetService<IIoTConfigService>();
                var jobsService = serviceProvider.GetService<IJobsService>();
                var config = await configService.LoadConfig();

#if DebugLocalWin

                configService.InitConfig(null, config);

                await jobsService.StartJobs(configService.GetModules());

#else
                using GpioController controller = new GpioController(PinNumberingScheme.Logical);
                configService.InitConfig(controller, config);
                await jobsService.Init();
                await jobsService.StartJobs(configService.GetModules());
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
