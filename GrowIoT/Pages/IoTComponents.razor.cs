using System;
using Blazored.Toast.Services;
using GrowIoT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using GrowIoT.Common;
using GrowIoT.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

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
		[Inject] protected IIoTConfigService IoTConfigService { get; private set; }
		#endregion


		#region properties
		public List<ModuleVm> Modules { get; set; }

		#endregion

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (!firstRender)
				await JSRuntime.InvokeVoidAsync("initPopovers");
			await base.OnAfterRenderAsync(firstRender);
		}

		protected override async Task OnInitializedAsync()
		{
			if (_isInitialized) return;

			Modules = (await BoxManager.GetModules()).ToList();
			foreach (var moduleVm in Modules)
			{
				moduleVm.Subscribe();
				moduleVm.VisualStateChanged += OnVisualStateChangeRequestReceived;
				if (moduleVm.IotModule != null && moduleVm.IotModule.RulesWorkState != WorkState.Paused)
				{
					//moduleVm.IotModule.RulesWorkState =  moduleVm.IotModule.Rules.All(x => x.IsEnabled) ? WorkState.Running : moduleVm.IotModule.Rules.All(x => !x.IsEnabled) ? WorkState.Stopped : WorkState.Mixed;
				}
				
			}
			await base.OnInitializedAsync();
			_isInitialized = true;
		}

		private async void OnVisualStateChangeRequestReceived(object? sender, VisualStateEventArgs e)
		{
			try
			{
				await InvokeAsync(StateHasChanged);
			}
			catch (Exception exception)
			{
				Logger.LogCritical(exception, "IoTComponentsBase -> OnVisualStateChangeRequestReceived");
				Console.WriteLine($"InvokeAsync Exception: {exception}");
			}
		}

		protected async void StopModule(ModuleVm module)
		{
			try
			{
				IsLoading = true;

				var mod = Modules.FirstOrDefault(x => x.Id == module.Id);
				if(mod == null)
					return;

				await IoTConfigService.ControlModuleJobs(module.Id, WorkState.Stopped);
				mod.CurrentValueString = string.Empty;
			}
			finally
			{
				IsLoading = false;
			}
		}

		protected async void ChangeState(ModuleVm module)
		{
			try
			{
				IsLoading = true;

				var mod = Modules.FirstOrDefault(x => x.Id == module.Id);
				if(mod == null)
					return;

				switch (mod.State)
				{
					case WorkState.Running:
						await IoTConfigService.ControlModuleJobs(module.Id, WorkState.Paused);
						break;
					case WorkState.Paused:
						await IoTConfigService.ControlModuleJobs(module.Id, WorkState.Running);
						break;
					case WorkState.Stopped:
						await IoTConfigService.ControlModuleJobs(module.Id, WorkState.Running);
						break;
				}
			}
			finally
			{
				IsLoading = false;
			}
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
