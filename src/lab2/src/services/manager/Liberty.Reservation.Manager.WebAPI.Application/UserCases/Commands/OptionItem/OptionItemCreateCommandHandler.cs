using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;

public class OptionItemCreateCommandHandler(
    ILogger<OptionItemCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemService optionItemService,
    IFacilityOptionItemService facilityOptionItemService
) : CreateCommandHandlerBase<OptionItemCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        OptionItemCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var optionItem = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.OptionItem>(payload);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newOptionItem = await optionItemService.CreateAsync(
                optionItem,
                false,
                cancellationToken
            );

            var newFacilityOptionItem = new FacilityOptionItem
            {
                FacilityId = facilityId,
                OptionItem = newOptionItem,
                IsEnabled = true
            };
            await facilityOptionItemService.CreateAsync(
                newFacilityOptionItem,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return newOptionItem.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create option item failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
