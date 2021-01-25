using System;
using Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;

namespace GrowIoT.Interfaces
{
    public interface IHistoryService : IInitializeService<IList<IModule>>
    {
        Task<List<ModuleHistoryItem>> GetModuleHistory(int moduleId, DateTime dateTo, int count = 50);

        Task<List<ModuleHistoryItem>> GetModuleHistory(int moduleId, DateTime dateFrom, DateTime dateTo, int? count = 50);

        Task<List<ModuleHistoryItem>> GetModuleHistory(int moduleId, int? count = 50);
    }
}
