using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowIoT.Interfaces
{
    public interface IHistoryService : IInitializableService
    {
        List<ModuleHistoryItem> GetModuleHistory(int moduleId);
    }
}
