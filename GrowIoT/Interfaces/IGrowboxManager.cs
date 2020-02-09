using System.Threading.Tasks;
using GrowIoT.ViewModels;

namespace GrowIoT.Interfaces
{
    public interface IGrowboxManager
    {
        Task<GrowBoxViewModel> GetBox();
        Task SaveBox(GrowBoxViewModel box);
    }
}
