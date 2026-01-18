using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Mappings;

public class AdjustmentMapperProfile : Profile
{
    public AdjustmentMapperProfile()
    {
        CreateMap<AdjustmentResult, AdjustmentResultResponse>()
            .ForMember(dest => dest.HotelId, opt => opt.MapFrom(src => src.HotelId))
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
            .ForMember(
                dest => dest.Reason,
                opt => opt.MapFrom(
                    src =>
                        src.Reason.HasValue ? src.Reason.Value.GetEnumDescriptions() : string.Empty
                )
            )
            .ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => src.IsSuccess));

        CreateMap<RoomAdjustmentStatus, RoomAdjustmentStatusResponse>()
            .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount))
            .ForMember(dest => dest.SuccessCount, opt => opt.MapFrom(src => src.SuccessCount))
            .ForMember(dest => dest.ErrorCount, opt => opt.MapFrom(src => src.ErrorCount));
    }
}
