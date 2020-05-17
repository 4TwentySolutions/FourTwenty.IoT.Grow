using System;
using Blazored.Toast.Services;
using GrowIoT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrowIoT.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace GrowIoT.Pages
{
    [Authorize]
    public class IoTComponentsBase : BaseGrowComponent, IDisposable
    {
        #region fields

        private bool _isInitialized;
        [Inject] protected ILogger<IoTComponentsBase> Logger { get; private set; }
        [Inject] protected IStringLocalizer<AppResources> Localizer { get; private set; }
        [Inject] protected IToastService ToastService { get; private set; }
        #endregion


        #region properties
        public List<ModuleVm> Modules { get; set; }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (_isInitialized) return;

            Modules = (await BoxManager.GetModules()).ToList();
            foreach (var moduleVm in Modules)
            {
                moduleVm.Subscribe();
                moduleVm.VisualStateChanged += OnVisualStateChangeRequestReceived;
            }
            await base.OnInitializedAsync();
            _isInitialized = true;
        }

        private async void OnVisualStateChangeRequestReceived(object? sender, VisualStateEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
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

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                if (Modules != null && Modules.Any())
                    foreach (ModuleVm moduleVm in Modules)
                    {
                        moduleVm.Unsubscribe();
                        moduleVm.VisualStateChanged -= OnVisualStateChangeRequestReceived;
                    }
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
