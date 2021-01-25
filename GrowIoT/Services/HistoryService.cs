﻿using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;
using GrowIoT.Interfaces;
using Infrastructure.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GrowIoT.Services
{
    public class HistoryService : IHistoryService, IDisposable
    {
        private readonly ILogger _logger;
        private SemaphoreSlim _locker = new SemaphoreSlim(1, 1);
        private IHistoryDataContext _historyDataContext;
        private readonly DbContextOptions<HistoryDbContext> _contextOptions;

        public HistoryService(DbContextOptions<HistoryDbContext> options, ILogger<HistoryService> logger)
        {
            _logger = logger;
            _historyDataContext = new HistoryDbContext(options);
            _contextOptions = options;
        }

        private async void SensOnDataReceived(object? sender, ModuleResponseEventArgs e)
        {
            if (!(sender is IoTComponent component))
                return;
            try
            {
	            await _locker.WaitAsync();

	            if (e.Data.IsSuccess)
                {
	                await _historyDataContext.Histories.AddAsync(new ModuleHistoryItem
	                {
		                ModuleId = component.Id,
		                Date = DateTime.Now,
		                Data = JsonConvert.SerializeObject(e.Data)
	                });
	                await _historyDataContext.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SensOnDataReceived));
            }
            finally
            {
                _locker.Release();
            }

        }

        public bool IsInitialized { get; private set; }

        public ValueTask Initialize(IList<IModule> modules)
        {
            _logger.LogInformation($"{nameof(HistoryService)}_{nameof(Initialize)} Started");
            foreach (var mod in modules)
            {
                switch (mod)
                {
                    case ISensor sensor:
                        sensor.DataReceived += SensOnDataReceived;
                        break;
                    case IRelay relay:
                        relay.StateChanged += SensOnDataReceived;
                        break;
                }
            }
            _logger.LogInformation($"{nameof(HistoryService)}_{nameof(Initialize)} Finished");
            IsInitialized = true;
            return new ValueTask();
        }

        public async Task<List<ModuleHistoryItem>> GetModuleHistory(int moduleId, DateTime dateTo, int count = 50)
        {
	        await using var context = new HistoryDbContext(_contextOptions);
            return await context.Histories.Where(x => x.ModuleId == moduleId && x.Date > dateTo)
                .OrderBy(x => x.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ModuleHistoryItem>> GetModuleHistory(int moduleId, int? count = 50)
        {
	        await using var context = new HistoryDbContext(_contextOptions);

	        var queryable = context.Histories.Where(x => x.ModuleId == moduleId).OrderBy(x => x.Date);

	        if (count.HasValue)
		        queryable.Take(count.GetValueOrDefault(50));

	        return await queryable.ToListAsync();
        }

        public async Task<List<ModuleHistoryItem>> GetModuleHistory(int moduleId, DateTime dateFrom, DateTime dateTo, int? count = 50)
        {
            await using var context = new HistoryDbContext(_contextOptions);

            var queryable = context.Histories.Where(x => x.ModuleId == moduleId && x.Date < dateTo && x.Date > dateFrom)
	            .OrderBy(x => x.Date);

            if (count.HasValue) 
	            queryable.Take(count.GetValueOrDefault(50));

            return await queryable.ToListAsync();


        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _locker?.Dispose();
                _locker = null;
                _historyDataContext?.Dispose();
                _historyDataContext = null;
            }
            _disposedValue = true;
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }

}
