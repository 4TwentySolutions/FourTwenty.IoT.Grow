using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces;
using GrowIoT.ViewModels;

namespace GrowIoT.Interfaces
{
	public interface IIoTConfigService : IInitializeService<GrowBoxViewModel>
	{
		IList<IModule> GetModules();
		GpioController Gpio { get; }
		IModule GetModule(int id);
		Task ControlModuleJobs(int moduleId, WorkState WorkState);
	}
}
