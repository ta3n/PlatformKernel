using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class EmailSentAuditLogMapperProfile : Profile
{
    public EmailSentAuditLogMapperProfile()
    {
        CreateMap<BookingAggregateMailAuditLog, EmailSentAuditLogResponse>()
            .ForMember(
                dest => dest.SentAt,
                opt => opt.MapFrom(
                    src => AppDate.ConvertLongToDateTime(src.CreatedAt, null)
                )
            );
    }
}
