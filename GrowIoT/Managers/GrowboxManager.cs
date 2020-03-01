using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FourTwenty.IoT.Connect.Entities;
using FourTwenty.IoT.Connect.Interfaces;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace GrowIoT.Managers
{
    public class GrowboxManager : IGrowboxManager
    {
        #region fields

        private readonly ITrackedEfRepository<GrowBox, int> _growBoxRepo;
        private readonly ITrackedEfRepository<GrowBoxModule, int> _modulesRepo;
        private readonly ITrackedEfRepository<ModuleRule, int> _rulesRepo;
        private readonly GrowDbContext _context;
        private readonly IIoTConfigService _configService;

        private readonly IMapper _mapper;

        #endregion

        public int[] AvailableGpio => new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };

        public GrowboxManager(
            ITrackedEfRepository<GrowBox, int> growBoxRepo,
            ITrackedEfRepository<GrowBoxModule, int> modulesRepo,
            ITrackedEfRepository<ModuleRule, int> rulesRepo,
            GrowDbContext context,
            IMapper mapper, 
            IIoTConfigService configService)
        {
            _growBoxRepo = growBoxRepo;
            _modulesRepo = modulesRepo;
            _rulesRepo = rulesRepo;
            _context = context;
            _mapper = mapper;
            _configService = configService;
        }

        private GrowBox _box;
        public async Task<GrowBoxViewModel> GetBox()
        {
            _box = await _growBoxRepo.GetByIdAsync(Infrastructure.Constants.BoxId);
            return _mapper.Map(_box, new GrowBoxViewModel());
        }

        public async Task<GrowBoxViewModel> GetBoxWithRules()
        {
            var box = await _context.Boxes.FirstOrDefaultAsync();
            _box = await _context.Boxes.Include(d => d.Modules)
                .ThenInclude(d => d.Rules).FirstOrDefaultAsync();
            return _mapper.Map(_box, new GrowBoxViewModel());
        }

        public async Task SaveBox(GrowBoxViewModel box)
        {
            _mapper.Map(box, _box);
            await _growBoxRepo.Save();
        }


        private GrowBoxModule _module = new GrowBoxModule();
        public async Task<ModuleVm> GetModule(int id)
        {
            _module = await _modulesRepo.GetSingleBySpecAsync(
                new ModuleWithRulesSpecification().And(new ModuleByIdSpecification(id)));
            return _mapper.Map(_module, new ModuleVm());
        }

        public async Task SaveModule(ModuleVm module)
        {
            _mapper.Map(module, _module);
            if (module.Id != default)
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

        public async Task<IReadOnlyList<ModuleVm>> GetModules()
        {
            var mapped = _mapper.Map<IReadOnlyList<ModuleVm>>(await _modulesRepo.ListAllAsync());
            var modules = _configService.GetModules();

            foreach (var moduleVm in mapped)
            {
                moduleVm.Sensor = modules.FirstOrDefault(x => x.Id == moduleVm.Id) as ISensor;
            }

            return mapped;
        } 
    }
}
