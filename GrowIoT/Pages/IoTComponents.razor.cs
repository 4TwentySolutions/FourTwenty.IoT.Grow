using Blazored.Toast.Services;
using FourTwenty.IoT.Connect.Entities;
using GrowIoT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowIoT.Pages
{
    public class IoTComponentsBase : BaseGrowComponent
    {
        #region fields

        [Inject] protected IStringLocalizer<AppResources> Localizer { get; private set; }
        [Inject] protected IToastService ToastService { get; private set; }
        #endregion


        #region properties
        public List<ModuleVm> Modules { get; set; }
   
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Modules = (await BoxManager.GetModules()).ToList();

            
            await base.OnInitializedAsync();
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
