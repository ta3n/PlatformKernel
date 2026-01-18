using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityUpdateClassificationCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<FacilityUpdateClassificationCommandHandler> logger,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider,
    ICacheService cacheService
) : UpdateCommandHandlerBase<FacilityUpdateClassificationCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        FacilityUpdateClassificationCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityAllergenService = serviceProvider.GetRequiredService<IFacilityAllergenService>();
        var facilityCategoryService = serviceProvider.GetRequiredService<IFacilityCategoryService>();
        var allergenService = serviceProvider.GetRequiredService<IAllergenService>();
        var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        var facilityService = serviceProvider.GetRequiredService<IFacilityService>();

        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        if (payload.Allergens is { Count: > 0 })
        {
            var existingAllergenCount = await allergenService.CountByIdsAsync(
                [.. payload.Allergens],
                cancellationToken
            );

            if (existingAllergenCount != payload.Allergens.Count)
            {
                throw new AllergenNotfoundException();
            }
        }

        List<long> listCategoryIds =
        [
            .. payload.Features ?? [],
            .. payload.Equipments ?? [],
            .. payload.Services ?? [],
            .. payload.Baths ?? [],
            .. payload.Sceneries ?? [],
            .. payload.Amenities ?? [],
            .. payload.Meals ?? []
        ];
        if (listCategoryIds.Count > 0)
        {
            var existingCategoryCount = await categoryService.CountByIdsWithoutFacilityAsync(
                [.. listCategoryIds],
                [
                    CategoryTypes.FacilityFeature,
                    CategoryTypes.FacilityEquipment,
                    CategoryTypes.Leisure,
                    CategoryTypes.Spa,
                    CategoryTypes.View,
                    CategoryTypes.Amenity,
                    CategoryTypes.MealType
                ],
                false,
                cancellationToken
            );

            if (existingCategoryCount != listCategoryIds.Count)
            {
                throw new CategoryNotfoundException();
            }
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await facilityAllergenService.ChangeFacilityAllergenAsync(
                facilityId,
                payload.Allergens ?? [],
                false,
                cancellationToken
            );

            _ = await facilityCategoryService.ChangeFacilityCategoryAsync(
                facilityId,
                listCategoryIds,
                false,
                cancellationToken
            );

            await facilityService.UpdateLastModifiedAsync(facilityId, cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            await cacheService.ResetAsync(
                $"{nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Facility)}*",
                cancellationToken: cancellationToken
            );

            return facilityId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(FacilityUpdateClassificationCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
