namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class DestinationMapperProfile : Profile
{
    public DestinationMapperProfile()
    {
        CreateMap<Site, SiteResponse>();
    }
}
