using FourTwenty.IoT.Connect.Entities;
using FourTwenty.IoT.Server.Components;
using FourTwenty.IoT.Server.Models;
using GrowIoT.ViewModels;
using Newtonsoft.Json;

namespace GrowIoT
{
    public class GrowProfile : AutoMapper.Profile
    {
        public GrowProfile()
        {
            CreateMap<GrowBox, GrowBoxViewModel>().ReverseMap();
            CreateMap<GrowBoxModule, ModuleVm>().AfterMap((entity, vm) =>
            {
	            vm.SetEntity(entity);

	            if (string.IsNullOrEmpty(entity.AdditionalData))
		            return;

	            vm.DisplayData = JsonConvert.DeserializeObject<AdditionalData>(entity.AdditionalData);

            }).ReverseMap().AfterMap((vm, entity) =>
            {
	            if (vm.DisplayData == null)
		            return;

	            entity.AdditionalData = JsonConvert.SerializeObject(vm.DisplayData);

            });
            CreateMap<ModuleRule, ModuleRuleVm>().AfterMap((entity, vm) => vm.SetEntity(entity)).ReverseMap();
        }
    }
}
