using System.Threading.Tasks;
using AutoMapper;
using FourTwenty.Core.Data.Interfaces;
using FourTwenty.IoT.Connect.Entities;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;

namespace GrowIoT.Managers
{
    public class GrowboxManager : IGrowboxManager
    {
        #region fields

        private readonly IAsyncRepository<GrowBox, int> _growBoxRepo;
        private readonly IAsyncRepository<GrowBoxModule, int> _modulesRepo;
        private readonly IAsyncRepository<ModuleRule, int> _rulesRepo;

        private readonly IMapper _mapper;

        #endregion

        public GrowboxManager(IAsyncRepository<GrowBox, int> growBoxRepo, IAsyncRepository<GrowBoxModule, int> modulesRepo, IAsyncRepository<ModuleRule, int> rulesRepo, IMapper mapper)
        {
            _growBoxRepo = growBoxRepo;
            _modulesRepo = modulesRepo;
            _rulesRepo = rulesRepo;
            _mapper = mapper;
        }


        public async Task<GrowBoxViewModel> GetBox() => _mapper.Map<GrowBoxViewModel>(await _growBoxRepo.GetByIdAsync(Infrastructure.Constants.BoxId));
        public Task SaveBox(GrowBoxViewModel box) => _growBoxRepo.UpdateAsync(_mapper.Map<GrowBox>(box));

    }
}
