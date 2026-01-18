using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Booking.Worker.Application.Mappings;

public class FaxAuditLogMapperProfile : Profile
{
    public FaxAuditLogMapperProfile()
    {
        CreateMap<SaveBookingAggregateFaxAuditLogRequest, BookingAggregateFaxAuditLog>()
            .ForMember(
                dest => dest.AggregateCode,
                opt => opt.MapFrom(
                    src => src.ReservationCode
                )
            );
    }
}
