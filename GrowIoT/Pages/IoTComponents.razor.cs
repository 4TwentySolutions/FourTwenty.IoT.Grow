using System;
using Blazored.Toast.Services;
using FourTwenty.IoT.Connect.Entities;
using GrowIoT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Interfaces;
using Microsoft.Extensions.Logging;

namespace GrowIoT.Pages
{
    public class IoTComponentsBase : BaseGrowComponent
    {
        #region fields

        [Inject] protected ILogger<IoTComponentsBase> Logger { get; private set; }
        [Inject] protected IStringLocalizer<AppResources> Localizer { get; private set; }
        [Inject] protected IToastService ToastService { get; private set; }
        #endregion


        #region properties
        public List<ModuleVm> Modules { get; set; }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Modules = (await BoxManager.GetModules()).ToList();
            //var workedModules = ConfigService.GetModules();
            foreach (var moduleVm in Modules)
            {
                moduleVm.DataReceived += OnDataReceived;
            }
            await base.OnInitializedAsync();
        }

        private async void OnDataReceived(object? sender, SensorEventArgs e)
        {
            Logger.LogInformation($"Data received: {e.Data}");
            await InvokeAsync(() =>
            {
                try
                {
                    StateHasChanged();
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, nameof(OnDataReceived));
                }

            });
        }

        protected async void Delete(ModuleVm module)
        {
            try
            {
                IsLoading = true;
                Modules.Remove(module);
                await BoxManager.DeleteModule(module);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
