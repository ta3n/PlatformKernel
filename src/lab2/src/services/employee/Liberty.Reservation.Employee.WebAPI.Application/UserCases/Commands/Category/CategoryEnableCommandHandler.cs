using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Category;

public class CategoryEnableCommandHandler(
    ILogger<CategoryEnableCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    ICategoryService categoryService,
    IFacilityService facilityService,
    IFacilityCategoryService facilityCategoryService
) : UpdateCommandHandlerBase<CategoryEnableCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        CategoryEnableCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
            var categoryId = request.Payload.Id ?? 0;

            _ = await categoryService.EnableAsync(
                categoryId,
                request.Payload.IsEnabled,
                cancellationToken: cancellationToken
            );

            if (!request.Payload.IsEnabled)
            {
                var facilities = (await facilityService.FindAllFacilitiesHasCategoryAsync(
                    categoryId,
                    cancellationToken
                )).ToList();

                facilities.ForEach(x => x.CategoryId = null);

                await facilityService.UpdateRangeAsync(
                    facilities,
                    false,
                    cancellationToken: cancellationToken
                );

                var facilityCategories = await facilityCategoryService.FindAllFacilitiesOfCategoryAsync(
                    categoryId,
                    cancellationToken
                );

                await facilityCategoryService.DeleteRangeAsync(
                    facilityCategories,
                    false,
                    cancellationToken
                );
            }

            await cacheService.ResetAsync(
                CacheKeys.ResetPatternManagerFacility,
                false,
                cancellationToken
            );

            await cacheService.ResetAsync(
                CacheKeys.ResetPatternBookingDetails,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return categoryId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Enable category failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
