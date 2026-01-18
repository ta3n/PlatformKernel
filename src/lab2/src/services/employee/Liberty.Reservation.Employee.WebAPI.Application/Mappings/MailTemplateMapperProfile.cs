using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class MailTemplateMapperProfile : Profile
{
    public MailTemplateMapperProfile()
    {
        CreateMap<IIoTemplate, MailTemplatePreviewResponse>();
    }
}
