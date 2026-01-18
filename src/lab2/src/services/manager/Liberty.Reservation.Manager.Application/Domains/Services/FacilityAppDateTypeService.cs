namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityAppDateTypeService(
    ILogger<FacilityAppDateTypeService> logger,
    IFacilityAppDateTypeRepository repository
) : BaseServiceRelation<FacilityAppDateType>(logger, repository), IFacilityAppDateTypeService;
