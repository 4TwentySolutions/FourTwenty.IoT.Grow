using BlazorStrap;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Entities;
using FourTwenty.IoT.Connect.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using FourTwenty.Core.Data.Interfaces;
using GrowIoT.Interfaces;
using Microsoft.Extensions.Localization;
using System.Linq;
using GrowIoT.ViewModels;

namespace GrowIoT.Pages
{
    public partial class IoTComponentBase : BaseGrowComponent
    {
        #region fields

        [Inject] protected IAsyncRepository<GrowBoxModule, int> ModuleRepo { get; private set; }
        [Inject] protected IStringLocalizer<AppResources> Localizer { get; private set; }
        [Inject] protected IToastService ToastService { get; private set; }
        [Inject] protected NavigationManager NavigationManager { get; private set; }
        #endregion

        protected BSModal RuleModal;
        [Parameter]
        public int? Id { get; set; }
        public ModuleVm Module { get; set; } = new ModuleVm() { GrowBoxId = Infrastructure.Constants.BoxId, Pins = new int[0] };
        public ModuleRuleVm CurrentRule { get; set; } = new ModuleRuleVm();
        #region rules
        public CronRule CronRule { get; set; } = new CronRule();
        #endregion


        #region overrides

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
                Module = await BoxManager.GetModule(Id.Value);
            await base.OnInitializedAsync();
        }
        #endregion

        #region events
        protected void OnChanged(ChangeEventArgs change)
        {
            if (!(change.Value is string val && int.TryParse(val, out int length))) return;
            if (Module.Pins == null)
            {
                Module.Pins = new int[length].Select(d=>d == 0 ? 2 : d).ToArray();
            }
            else
            {
                var pins = Module.Pins;
                if (length == pins.Length) return;
                Array.Resize(ref pins, length);
                Module.Pins = pins.Select(d=>d == 0 ? 2 : d).ToArray();                
            }
        }



        protected async void PinInputChanged(ChangeEventArgs change, int index)
        {
            if (change.Value is string str && int.TryParse(str, out int val) && BoxManager.AvailableGpio.Contains(val))
            {
                Module.Pins[index] = val;
            }
            else
            {
                //TODO Ugly hack. need to replace.
                var old = Module.Pins[index];
                Module.Pins[index] = -DateTime.Now.Second;
                await Task.Run(async () =>
                {
                    await InvokeAsync(() =>
                    {
                        Module.Pins[index] = old;
                        StateHasChanged();
                    });
                });
            }
        }

        protected void AddRuleClicked()
        {
            CurrentRule = new ModuleRuleVm();
            CronRule = new CronRule();
            RuleModal.Show();
        }

        protected void OnRuleClicked(ModuleRuleVm rule)
        {
            CurrentRule = rule;
            switch (CurrentRule.RuleType)
            {
                case RuleType.CronRule:
                    CronRule = JsonConvert.DeserializeObject<CronRule>(CurrentRule.RuleContent) ?? new CronRule();
                    break;

            }
            RuleModal.Show();
        }


        protected void OnSubmitRule()
        {
            RuleModal.Hide();
            switch (CurrentRule.RuleType)
            {
                case RuleType.CronRule:
                    CurrentRule.RuleContent = JsonConvert.SerializeObject(CronRule);
                    CurrentRule.GrowBoxModuleId = Module.Id;
                    if (CurrentRule.Id == 0)
                    {
                        if (Module.Rules == null)
                            Module.Rules = new List<ModuleRuleVm>();
                        Module.Rules?.Add(CurrentRule);
                    }
                    break;
            }
            ToastService.ShowSuccess(Localizer["Rule added"]);
        }

        protected async void OnSubmit()
        {
            try
            {
                IsLoading = true;
                await BoxManager.SaveModule(Module);
                ToastService.ShowSuccess(Localizer["Module successfully saved"], Localizer["Congratulations!"]);
                NavigationManager.NavigateTo("iotcomponents");
            }
            finally
            {
                IsLoading = false;
            }

        }

        protected void RemoveRule(ModuleRuleVm rule)
        {
            if (Module.Rules.Remove(rule))
                ToastService.ShowSuccess(Localizer["Rule removed"]);
        }

        #endregion

    }
}
