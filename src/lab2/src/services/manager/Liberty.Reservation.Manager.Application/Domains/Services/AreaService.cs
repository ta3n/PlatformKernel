namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class AreaService(
    ILogger<AreaService> logger,
    IAreaRepository areaRepository
) : BaseService<Area>(logger, areaRepository, new AreaNotfoundException()), IAreaService;
