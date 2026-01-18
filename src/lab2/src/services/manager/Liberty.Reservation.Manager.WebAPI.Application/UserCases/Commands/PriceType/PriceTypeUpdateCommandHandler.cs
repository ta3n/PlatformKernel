namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;

public class PriceTypeUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IAppDateTypeService appDateTypeService
) : UpdateCommandHandlerBase<PriceTypeUpdateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PriceTypeUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingAppDateTypeCount = await appDateTypeService.CountByIdsAsync(
            [payload.Id ?? 0],
            cancellationToken
        );
        if (existingAppDateTypeCount == 0)
        {
            throw new AppDateTypeNotfoundException();
        }

        var appDateType = Mapper.Map<AppDateType>(payload);

        var editAppDateType = await appDateTypeService.UpdateAsync(
            appDateType,
            cancellationToken: cancellationToken
        );

        return editAppDateType.Id;
    }
}
