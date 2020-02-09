using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FourTwenty.IoT.Connect.Entities;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;
using Infrastructure.Interfaces;
using Infrastructure.Specifications;

namespace GrowIoT.Managers
{
    public class GrowboxManager : IGrowboxManager
    {
        #region fields

        private readonly ITrackedEfRepository<GrowBox, int> _growBoxRepo;
        private readonly ITrackedEfRepository<GrowBoxModule, int> _modulesRepo;
        private readonly ITrackedEfRepository<ModuleRule, int> _rulesRepo;

        private readonly IMapper _mapper;

        #endregion

        public int[] AvailableGpio => new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };

        public GrowboxManager(ITrackedEfRepository<GrowBox, int> growBoxRepo, ITrackedEfRepository<GrowBoxModule, int> modulesRepo, ITrackedEfRepository<ModuleRule, int> rulesRepo, IMapper mapper)
        {
            _growBoxRepo = growBoxRepo;
            _modulesRepo = modulesRepo;
            _rulesRepo = rulesRepo;
            _mapper = mapper;
        }

        private GrowBox _box;
        public async Task<GrowBoxViewModel> GetBox()
        {
            _box = await _growBoxRepo.GetByIdAsync(Infrastructure.Constants.BoxId);
            return _mapper.Map(_box, new GrowBoxViewModel());
        }

        public async Task SaveBox(GrowBoxViewModel box)
        {
            _mapper.Map(box, _box);
            await _growBoxRepo.Save();
        }


        private GrowBoxModule _module;
        public async Task<ModuleVm> GetModule(int id)
        {
            _module = await _modulesRepo.GetSingleBySpecAsync(
                new ModuleWithRulesSpecification().And(new ModuleByIdSpecification(id)));
            return _mapper.Map(_module, new ModuleVm());
        }

        public async Task SaveModule(ModuleVm module)
        {
            _mapper.Map(module, _module);
            if (module.Id > 0)
            {
                await _modulesRepo.Save();
            }
            else
            {
                await _modulesRepo.AddAsync(_module);
            }
        }

        public Task DeleteModule(ModuleVm module) => _modulesRepo.DeleteAsync(module.Id);
        public Task DeleteRule(ModuleRule rule) => _rulesRepo.DeleteAsync(rule);

        public async Task<IReadOnlyList<ModuleVm>> GetModules() => _mapper.Map<IReadOnlyList<ModuleVm>>(await _modulesRepo.ListAllAsync());
    }
}
