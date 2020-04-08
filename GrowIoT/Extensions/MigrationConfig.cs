using FourTwenty.Core.Data.Extensions;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GrowIoT.Extensions
{
    public static class MigrationExtension
    {
        public static IHost SeedIdentity(this IHost webHost)
        {
            using (var serviceScope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                if (userManager != null)
                    AppDbContextSeed.SeedAsync(userManager).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            return webHost;
        }
        public static IHost ApplyMigrations(this IHost webHost)
        {
            using (var serviceScope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                Log.Logger.Information("DB init");
                serviceScope.ServiceProvider.GetService<IGrowDataContext>().InitDb().ConfigureAwait(false).GetAwaiter().GetResult();
                Log.Logger.Information("DB init finished\nStarting IOT initialization");
                //if (!serviceScope.ServiceProvider.GetService<GrowDbContext>().AllMigrationsApplied())
                //{
                //    serviceScope.ServiceProvider.GetService<GrowDbContext>().Database.Migrate();
                //}
            }
            return webHost;
        }
    }
}
