using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FourTwenty.IoT.Connect.Entities;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Connect.Models;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GrowIoT.Managers
{
    public class GrowboxManager : IGrowboxManager
    {
        #region fields

        private readonly IGrowDataContext _context;
        private readonly IIoTConfigService _configService;
        private readonly ILogger _logger;

        private readonly IMapper _mapper;

        #endregion

        public int[] AvailableGpio => new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };

        public GrowboxManager(
            IGrowDataContext context,
            IMapper mapper,
            IIoTConfigService configService,
            ILogger<GrowboxManager> logger)
        {
            _context = context;
            _mapper = mapper;
            _configService = configService;
            _logger = logger;
        }

        private GrowBox _box;
        public async Task<GrowBoxViewModel> GetBox()
        {
            _box = await _context.Boxes.FindAsync(Infrastructure.Constants.BoxId);
            return _mapper.Map(_box, new GrowBoxViewModel());
        }

        public async Task<GrowBoxViewModel> GetBoxWithRules()
        {
            _box = await _context.Boxes.Include(d => d.Modules)
                .ThenInclude(d => d.Rules).FirstOrDefaultAsync();
            return _mapper.Map(_box, new GrowBoxViewModel());
        }

        public async Task SaveBox(GrowBoxViewModel box)
        {
            _mapper.Map(box, _box);
            await _context.CommitAsync();
        }


        public async Task<ModuleVm> GetModule(int id)
        {
            var module = _mapper.Map<ModuleVm>(await _context.Modules.Include(d => d.Rules).FirstOrDefaultAsync(d => d.Id == id));
            var moduleRaw = _configService.GetModule(id);

            module.Sensor = moduleRaw as ISensor;
            module.Relay = moduleRaw as IRelay;

            foreach (var moduleRuleVm in module.Rules)
            {
                var rule = JsonConvert.DeserializeObject<CronRuleData>(moduleRuleVm.RuleContent) ?? new CronRuleData();
                moduleRuleVm.Job = rule.Job;
                moduleRuleVm.Pins = module.Pins.ToList();
            }

            return module;
        }

        public async Task SaveModule(ModuleVm module)
        {
            _mapper.Map(module, module.DbEntity);
            if (module.Id == default)
                await _context.Modules.AddAsync(module.DbEntity);
            await _context.CommitAsync();
        }

        public async Task DeleteModule(ModuleVm module)
        {
            if (module.DbEntity == null)
                return;
            _context.Modules.Remove(module.DbEntity);
            await _context.CommitAsync();
        }
        public async Task DeleteRule(ModuleRule rule)
        {
            _context.Rules.Remove(rule);
            await _context.CommitAsync();
        }

        public async Task<IReadOnlyList<ModuleVm>> GetModules()
        {
            var mapped = _mapper.Map<IReadOnlyList<ModuleVm>>(await _context.Modules.ToListAsync());
            var modules = _configService.GetModules();

            foreach (var moduleVm in mapped)
            {
                moduleVm.Sensor = modules.FirstOrDefault(x => x.Id == moduleVm.Id) as ISensor;
                moduleVm.Relay = modules.FirstOrDefault(x => x.Id == moduleVm.Id) as IRelay;
            }

            return mapped;
        }
    }
}
