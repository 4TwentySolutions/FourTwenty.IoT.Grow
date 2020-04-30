using System;
using System.Device.Gpio;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.Toast;
using FourTwenty.Core.Data.Interfaces;
using FourTwenty.IoT.Server.Hubs;
using FourTwenty.IoT.Server.Interfaces;
using FourTwenty.IoT.Server.Services;
using Ganss.XSS;
using GrowIoT.Areas.Identity;
using GrowIoT.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrowIoT.Interfaces;
using GrowIoT.Managers;
using GrowIoT.Services;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Impl;
using Serilog;
using Syncfusion.Blazor;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.AspNetCore.Components.Server.Circuits;

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
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjM2MzIzQDMxMzgyZTMxMmUzMFlTVkFONXl6ZlRac1BLRlYxSnFSMFYyUzV6L2xaVUdwNXdqYjlURlMvcG89");

            services.AddSingleton<SqLiteProvider<GrowSqlConnectionAsync>>();
            services.AddSingleton<SqLiteProvider<HistorySqlConnectionAsync>>();

            services.AddDbContextPool<GrowDbContext>((provider, builder) => builder.UseSqlite(provider.GetService<SqLiteProvider<GrowSqlConnectionAsync>>().GetConnection()));
            services.AddDbContextPool<HistoryDbContext>((provider, builder) => builder.UseSqlite(provider.GetService<SqLiteProvider<HistorySqlConnectionAsync>>().GetConnection()));

            services.AddScoped<IGrowDataContext>(provider => provider.GetRequiredService<GrowDbContext>());
            services.AddScoped<IHistoryDataContext>(provider => provider.GetRequiredService<HistoryDbContext>());

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<GrowDbContext>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => opts.ResourcesPath = "Resources");

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR();
            services.AddAutoMapper(GetType());
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            services.AddSingleton<IIoTConfigService, IoTConfigService>();
            services.AddSingleton<IJobsService, JobsService>();
            services.AddSingleton<IHubService, HubService>();
            services.AddSingleton<IMemoryMetricsClient, MemoryMetricsClient>();
            services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(x =>
            {
                // Configure sanitizer rules as needed here.
                // For now, just use default rules + allow class attributes
                var sanitizer = new Ganss.XSS.HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                return sanitizer;
            });

            services.AddBlazoredToast();
            services.AddScoped<IGrowboxManager, GrowboxManager>();
            services.AddLocalization(opts => opts.ResourcesPath = "Resources");
            services.AddGrowHealthChecks();
            services.AddSyncfusionBlazor();
            services.AddSingleton<CircuitHandler>(new CircuitHandlerService());

            services.AddScoped<IHistoryService, HistoryService>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILogger<Startup> logger)
        {
            logger.LogInformation("Configure started");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            var localizationOptions = app.ApplicationServices
                .GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/ping");
            app.UseMvcWithDefaultRoute();

            app.AddIoT(serviceProvider, logger);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<RealTimeDataHub>("/commandsHub");
                endpoints.MapFallbackToPage("/_Host");
            });

        }
    }
}
