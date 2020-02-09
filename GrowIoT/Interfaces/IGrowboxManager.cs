using System.Collections.Generic;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Entities;
using GrowIoT.ViewModels;

namespace GrowIoT.Interfaces
{
    public interface IGrowboxManager
    {
        #region properties
        int[] AvailableGpio { get; }
        #endregion
        #region management

        Task<GrowBoxViewModel> GetBox();
        Task SaveBox(GrowBoxViewModel box);

        Task<IReadOnlyList<ModuleVm>> GetModules();
        Task<ModuleVm> GetModule(int id);
        Task SaveModule(ModuleVm module);
        Task DeleteModule(ModuleVm module);
        Task DeleteRule(ModuleRule rule); 
        #endregion
    }
}
