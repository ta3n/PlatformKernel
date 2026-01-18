namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;

public class PriceTypeDeleteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IAppDateTypeService appDateTypeService
) : DeleteCommandHandlerBase<PriceTypeDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PriceTypeDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var removePriceType = await appDateTypeService.DeleteAsync(
            payload.Id ?? 0,
            true,
            cancellationToken
        );

        return removePriceType.Id;
    }
}
