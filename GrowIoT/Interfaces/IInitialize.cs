using System.Threading.Tasks;

namespace GrowIoT.Interfaces
{
    public interface IInitializeService<in T>
    {
        bool IsInitialized { get; }
        ValueTask Initialize(T options);
    }
}
