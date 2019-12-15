using System;
using System.Device.Gpio;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrowIoT.Hubs;
using GrowIoT.Interfaces;
using GrowIoT.Services;
using Microsoft.AspNetCore.Mvc;
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
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR();

            services.AddScoped<IIoTConfigService, IoTConfigService>();
            services.AddSingleton<IJobsService, JobsService>();

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
                endpoints.MapHub<CommandsHub>("/commandsHub");
                endpoints.MapFallbackToPage("/_Host");
            });

            try
            {
                var configService = (IIoTConfigService)serviceProvider.GetService(typeof(IIoTConfigService));

                var config = await configService.LoadConfig();
                using GpioController controller = new GpioController(PinNumberingScheme.Logical);
                configService.InitConfig(controller, config);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
