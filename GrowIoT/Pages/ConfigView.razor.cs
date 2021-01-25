using System.Threading.Tasks;
using Blazored.Toast.Services;
using FourTwenty.IoT.Connect.Dto;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace GrowIoT.Pages
{
    public partial class ConfigViewBase : BaseGrowComponent
    {
        #region fields

        [Inject] protected IToastService ToastService { get; private set; }
        [Inject] protected IIoTConfigService ConfigService { get; private set; }
        [Inject] protected NavigationManager Navigation { get; private set; }
        [Inject] protected IStringLocalizer<AppResources> Localizer { get; private set; }

        #endregion

        public ConfigDto Config { get; set; }
        public GrowBoxViewModel Box { get; set; } = new GrowBoxViewModel();
        protected override async Task OnInitializedAsync()
        {
            //Config = //new ConfigDto();//ConfigService.GetConfig();
            var box = await BoxManager.GetBox();
            if (box != null)
            {
                Box = box;
            }
            await base.OnInitializedAsync();
        }


        protected async void HandleValidSubmit()
        {
            try
            {
                IsLoading = true;
                await BoxManager.SaveBox(Box);
                ToastService.ShowSuccess("Growbox successfully saved", "Congratulations!");
                Navigation.NavigateTo("/");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

