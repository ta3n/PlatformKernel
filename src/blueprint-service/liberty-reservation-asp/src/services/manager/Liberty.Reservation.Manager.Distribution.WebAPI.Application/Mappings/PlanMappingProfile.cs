using System.Globalization;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Mappings;

public class PlanMappingProfile : Profile
{
    public PlanMappingProfile()
    {
        CreateMap<UpdatePublishAcceptPlanRequest, Plan>()
            .ForMember(dest => dest.UseBookingReception, opt => opt.MapFrom(src => src.UseBookingReception))
            .ForMember(dest => dest.UseDisplayDate, opt => opt.MapFrom(src => src.UseDisplayDate))
            .ForMember(dest => dest.UseAcceptDate, opt => opt.MapFrom(src => src.UseAcceptDate))
            .ForMember(dest => dest.BookingReceptionStart, opt => opt.MapFrom(src => ParseNullableLong(src.BookingReceptionStart)))
            .ForMember(dest => dest.BookingReceptionEnd, opt => opt.MapFrom(src => ParseNullableLong(src.BookingReceptionEnd)))
            .ForMember(dest => dest.DisplayDateStart, opt => opt.MapFrom(src => ParseNullableLong(src.DisplayDateStart)))
            .ForMember(dest => dest.DisplayDateEnd, opt => opt.MapFrom(src => ParseNullableLong(src.DisplayDateEnd)))
            .ForMember(dest => dest.AcceptDateStart, opt => opt.MapFrom(src => ParseNullableLong(src.AcceptDateStart)))
            .ForMember(dest => dest.AcceptDateEnd, opt => opt.MapFrom(src => ParseNullableLong(src.AcceptDateEnd)))
            .ForMember(dest => dest.ReceptionDayLimit, opt => opt.MapFrom(src => ParseNullableLong(src.ReceptionDayLimit)))
            .ForMember(dest => dest.ReceptionLimit, opt => opt.MapFrom(src => ParseNullableTimeSpan(src.ReceptionLimit)));

        CreateMap<Plan, UpdatePublishAcceptPlanResponse>()
            .ForMember(dest => dest.PlanId, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.UseBookingReception, opt => opt.MapFrom(src => src.UseBookingReception))
            .ForMember(dest => dest.UseDisplayDate, opt => opt.MapFrom(src => src.UseDisplayDate))
            .ForMember(dest => dest.UseAcceptDate, opt => opt.MapFrom(src => src.UseAcceptDate))
            .ForMember(dest => dest.BookingReceptionStart, opt => opt.MapFrom(src => FormatNullable(src.BookingReceptionStart)))
            .ForMember(dest => dest.BookingReceptionEnd, opt => opt.MapFrom(src => FormatNullable(src.BookingReceptionEnd)))
            .ForMember(dest => dest.DisplayDateStart, opt => opt.MapFrom(src => FormatNullable(src.DisplayDateStart)))
            .ForMember(dest => dest.DisplayDateEnd, opt => opt.MapFrom(src => FormatNullable(src.DisplayDateEnd)))
            .ForMember(dest => dest.AcceptDateStart, opt => opt.MapFrom(src => FormatNullable(src.AcceptDateStart)))
            .ForMember(dest => dest.AcceptDateEnd, opt => opt.MapFrom(src => FormatNullable(src.AcceptDateEnd)))
            .ForMember(dest => dest.ReceptionDayLimit, opt => opt.MapFrom(src => FormatNullable(src.ReceptionDayLimit)))
            .ForMember(dest => dest.ReceptionLimit, opt => opt.MapFrom(src => FormatNullable(src.ReceptionLimit)));
    }

    private static long? ParseNullableLong(
        string? value
    )
    {
        return string.IsNullOrEmpty(value) ? null : long.Parse(value);
    }

    private static TimeSpan? ParseNullableTimeSpan(
        string? value
    )
    {
        return string.IsNullOrEmpty(value) ? null : TimeSpan.Parse(value, CultureInfo.InvariantCulture);
    }

    private static string FormatNullable<T>(
        T? value
    ) where T : struct
    {
        return value?.ToString() ?? string.Empty;
    }
}
