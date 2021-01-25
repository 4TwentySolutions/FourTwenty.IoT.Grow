using GrowIoT.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GrowIoT.Pages
{
    public abstract class BaseGrowComponent : ComponentBase
    {

        [Inject] protected IGrowboxManager BoxManager { get; private set; }
        [Inject] protected IIoTConfigService ConfigService { get; private set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                StateHasChanged();
            }
        }
    }
}
