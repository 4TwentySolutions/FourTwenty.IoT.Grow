using Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;

namespace GrowIoT.Interfaces
{
    public interface IHistoryService : IInitializeService<IList<IModule>>
    {
        Task<ICollection<ModuleHistoryItem>> GetModuleHistory(int moduleId, int count = 50);
    }
}
