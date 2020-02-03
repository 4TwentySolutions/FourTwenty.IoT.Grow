using FourTwenty.IoT.Connect.Entities;
using GrowIoT.ViewModels;

namespace GrowIoT
{
    public class GrowProfile : AutoMapper.Profile
    {
        public GrowProfile()
        {
            CreateMap<GrowBox, GrowBoxViewModel>().ReverseMap();
        }
    }
}
