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

namespace GrowIoT.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryDataContext _historyDataContext;

        public bool IsInitialized { get; set; }

        public HistoryService(IHistoryDataContext historyDataContext)
        {
            _historyDataContext = historyDataContext;
        }



        private void ConfigInited(object e, EventArgs args)
        {

        }

        private void RelayOnStateChanged(object? sender, RelayEventArgs e)
        {
            if (!(sender is IoTComponent component))
                return;


            //var serviceProvider = _serviceCollection.BuildServiceProvider();
            //var historyContext = serviceProvider.GetService<IHistoryDataContext>();

            _historyDataContext.Histories.Add(new ModuleHistoryItem
            {
                ModuleId = component.Id,
                Date = DateTime.Now,
                Data = JsonConvert.SerializeObject((e.Pin, e.State))
            });

            //_logger.LogInformation($@"{component.Name} - {(e.State == RelayState.Opened ? "ON" : "OFF")}");
        }

        private void SensOnDataReceived(object? sender, SensorEventArgs e)
        {
            if (!(sender is IoTComponent component))
                return;

            //var serviceProvider = _serviceCollection.BuildServiceProvider();
            //var historyContext = serviceProvider.GetService<IHistoryDataContext>();

            _historyDataContext.Histories.Add(new ModuleHistoryItem
            {
                ModuleId = component.Id,
                Date = DateTime.Now,
                Data = JsonConvert.SerializeObject(e.Data)
            });

            //_logger.LogInformation($@"{component.Name} - {e.Data}");
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
