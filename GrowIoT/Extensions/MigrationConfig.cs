using FourTwenty.Core.Data.Extensions;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                if (!serviceScope.ServiceProvider.GetService<GrowDbContext>().AllMigrationsApplied())
                {
                    serviceScope.ServiceProvider.GetService<GrowDbContext>().Database.Migrate();
                }
            }
            return webHost;
        }
    }
}
