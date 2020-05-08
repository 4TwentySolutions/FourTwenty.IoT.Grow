using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;
using GrowIoT.Interfaces;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace GrowIoT.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IConfiguration _configuration;
        private Semaphore semaphoreObject = new Semaphore(initialCount: 1, maximumCount: 1, name: nameof(HistoryService));
        //private DbContextOptionsBuilder<HistoryDbContext> _optionsBuilder;
        private SqLiteProvider<HistorySqlConnectionAsync> _sqlProvider;
        public bool IsInitialized { get; set; }

        public HistoryService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("local");
            _sqlProvider = serviceProvider.GetService<SqLiteProvider<HistorySqlConnectionAsync>>();

            //_optionsBuilder = new DbContextOptionsBuilder<HistoryDbContext>().UseSqlite(_sqlProvider.GetConnection());
        }

        private void RelayOnStateChanged(object? sender, RelayEventArgs e)
        {
            if (!(sender is IoTComponent component))
                return;

            semaphoreObject.WaitOne();

            var _optionsBuilder = new DbContextOptionsBuilder<HistoryDbContext>().UseSqlite(_sqlProvider.GetConnection());
            using var context = new HistoryDbContext(_optionsBuilder.Options);

            context.Histories.Add(new ModuleHistoryItem
            {
                ModuleId = component.Id,
                Date = DateTime.Now,
                Data = JsonConvert.SerializeObject(e.Data)
            });

            context.SaveChanges();

            semaphoreObject.Release();
        }

        private void SensOnDataReceived(object? sender, SensorEventArgs e)
        {
            if (!(sender is IoTComponent component))
                return;

            semaphoreObject.WaitOne();

            var _optionsBuilder = new DbContextOptionsBuilder<HistoryDbContext>().UseSqlite(_sqlProvider.GetConnection());
            using var context = new HistoryDbContext(_optionsBuilder.Options);

            context.Histories.Add(new ModuleHistoryItem
            {
                ModuleId = component.Id,
                Date = DateTime.Now,
                Data = JsonConvert.SerializeObject(e.Data)
            });

            context.SaveChanges();

            semaphoreObject.Release();
        }

        public void Initialize(InitializableOptions options)
        {
            if (options is HistoryInitOptions historyInitOptions)
            {
                if (!historyInitOptions.Modules?.Any() ?? false)
                    return;

                foreach (var module in historyInitOptions.Modules)
                {
                    switch (module)
                    {
                        case ISensor sens:
                            sens.DataReceived += SensOnDataReceived;
                            break;
                        case IRelay relay:
                            relay.StateChanged += RelayOnStateChanged;
                            break;
                    }
                }

                IsInitialized = true;
            }
        }

        public List<ModuleHistoryItem> GetModuleHistory(int moduleId)
        {
            var _optionsBuilder = new DbContextOptionsBuilder<HistoryDbContext>().UseSqlite(_sqlProvider.GetConnection());
            using var context = new HistoryDbContext(_optionsBuilder.Options);

            return context.Histories.Where(x => x.ModuleId == moduleId).OrderBy(x => x.Date).ToList();
        }
    }

    public class HistoryInitOptions : InitializableOptions
    {
        public IList<IModule> Modules { get; set; }

        public HistoryInitOptions(IList<IModule> modules)
        {
            Modules = modules;
        }
    }
}
