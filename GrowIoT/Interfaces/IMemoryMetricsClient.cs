using GrowIoT.Models.Diagnostics;

namespace GrowIoT.Interfaces
{
    public interface IMemoryMetricsClient
    {
        MemoryMetrics GetMetrics();
    }
}
