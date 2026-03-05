using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Mappings;

public class FacilityMappingProfile : Profile
{
    public FacilityMappingProfile()
    {
        CreateMap<Facility, FacilityRoomGroupDto>()
            .ForMember(
                dest => dest.Address1,
                opt => opt.MapFrom(src => src.Address1!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Address2,
                opt => opt.MapFrom(src => src.Address2!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Address3,
                opt => opt.MapFrom(src => src.Address3!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Address4,
                opt => opt.MapFrom(src => src.Address4!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Heading,
                opt => opt.MapFrom(src => src.Heading1!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.RoomGroup,
                opt => opt.MapFrom(
                    src => src.FacilityRoomGroups!
                        .Where(x => x.RoomGroup!.IsEnabled)
                        .Select(
                            x => new RoomGroupDto(
                                x.RoomGroup!.Code,
                                x.RoomGroup!.Name!.GetValueByHeader()
                            )
                        )
                        .ToList()
                )
            )
            ;
    }
}
