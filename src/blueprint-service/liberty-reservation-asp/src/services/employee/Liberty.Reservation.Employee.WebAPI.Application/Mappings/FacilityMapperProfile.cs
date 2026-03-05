namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class FacilityMapperProfile : Profile
{
    public FacilityMapperProfile()
    {
        CreateMap<FacilityUpdateRequest, Facility>()
            .ConstructUsing(
                src => new Facility
                {
                    Id = src.Id ?? 0,
                    CanOnLinePayment = src.CanOnLinePayment,
                    Memo = src.Memo,
                    Meta = new Reservation.Application.Contexts.DataContexts.Entities.Metas.FacilityMeta { SystemEMail = src.SystemEMail }
                }
            );

        CreateMap<Facility, FacilityResponse>();
    }
}
