using PlatformKernel.Entity.ValueObjects;

namespace PlatformKernel.Service.WebApi.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
    }
}
