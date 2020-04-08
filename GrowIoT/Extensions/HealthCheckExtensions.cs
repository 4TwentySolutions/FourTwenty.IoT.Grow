using System.Net.NetworkInformation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GrowIoT.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddGrowHealthChecks(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHealthChecks().AddCheck("ping", () =>
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
            return serviceCollection;
        }
    }
}
