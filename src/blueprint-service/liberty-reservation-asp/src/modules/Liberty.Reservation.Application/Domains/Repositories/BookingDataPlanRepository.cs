using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataPlanRepository(
    DbContext dataContext
) : RepositoryBase<Plan>(dataContext), IBookingDataPlanRepository
{
    public async Task<Plan?> GetByIdAsync(
        long facilityId,
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityPlans!.Any(
                    t => t.FacilityId == facilityId
                )
            );

        var existingEntity = await queryable.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );

        return existingEntity;
    }

    public async Task<IPage<BookingPlanModel>> GetPageBookingPlansAsync(
        BookingDataPlanFilterParameter filter,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var systemCanOnlinePayment = filter.SystemCanOnlinePayment;
        var facilityState = filter.FacilityState;

        var page = await GetQueryableBookingPlan()
            .UsePageableAsync(
                pageable,
                cancellationToken: cancellationToken
            );

        return page;

        IQueryable<BookingPlanModel> GetQueryableBookingPlan()
        {
            var queryable = GetQueryableWithAsNoTracking()
                .Where(
                    x => x.PlanRoomGroups!.Any(
                        t => t.IsEnabled
                    )
                )
                .ApplyFilter(
                    new BookingSearchQueryableFilterParameter(
                        filter.FacilityId,
                        filter.SiteId,
                        filter.Search.CheckInDate,
                        filter.Search.RestNumber,
                        filter.Search.Secret,
                        filter.IsSecret,
                        filter.PlanIds,
                        filter.RoomGroupIds,
                        filter.Search.DayUse
                    )
                )
                .Select(
                    plan => new BookingPlanModel(
                        plan.Id,
                        plan.IsEnabled,
                        plan.UseAcceptPersonNumber,
                        plan.AcceptPersonNumberMin,
                        plan.AcceptPersonNumberMax,
                        plan.NumberOfStayLimitMin,
                        plan.NumberOfStayLimitMax,
                        plan.IsOnLinePayment && facilityState.CanOnLinePayment && facilityState.IsOnLinePayment && systemCanOnlinePayment,
                        plan.CheckInStart,
                        plan.CheckInEnd,
                        plan.CheckOut,
                        plan.IsOnSidePayment && facilityState.IsOnSidePayment,
                        plan.UseDaySaleLimit,
                        plan.PlanDaySaleLimitType,
                        plan.RoomNumberDaySaleLimit,
                        plan.UseDisplayDate,
                        plan.DisplayDateStart,
                        plan.DisplayDateEnd,
                        plan.UseAcceptDate,
                        plan.AcceptDateStart,
                        plan.AcceptDateEnd,
                        plan.GroupNumberDaySaleLimit,
                        plan.ReceptionDayLimit,
                        plan.ReceptionLimit,
                        plan.UseBookingReception,
                        plan.BookingReceptionStart,
                        plan.BookingReceptionEnd,
                        plan.Name,
                        plan.Tag,
                        plan.Summary,
                        plan.Description,
                        plan.PlanType,
                        facilityState.UseDailyPerson,
                        plan.DisplayOrder,
                        plan.DayUse,
                        facilityState.UseSpaTax,
                        plan.PlanCategories!
                            .Where(
                                planCategory => planCategory.Category!.CategoryType == CategoryTypes.Plan
                                    && !planCategory.Category.IsMaster
                            )
                            .Select(
                                planCategory => new CategoryOfBookingPlanModel(
                                    planCategory.Category!.Id,
                                    planCategory.Category!.Name,
                                    planCategory.Category!.CategoryType,
                                    planCategory.Category!.IsMaster
                                ) { Code = planCategory.Category!.Code }
                            ),
                        plan.FilePlans!
                            .Where(filePlan => filePlan.File!.IsEnabled)
                            .OrderBy(filePlan => filePlan.Index)
                            .Select(
                                filePlan => new MediaOfBookingPlanModel(
                                    filePlan.File!.Code,
                                    filePlan.File!.ContentType,
                                    filePlan.Index,
                                    filePlan.File!.IsEnabled
                                )
                            ),
                        plan.PlanMealTypes!
                            .OrderBy(x => x.MealType!.DisplayOrder)
                            .Select(
                                planMealType => new MealTypeOfBookingPlanModel(
                                    planMealType.MealType!.Id,
                                    planMealType.MealType!.Name,
                                    planMealType.MealTypeEatType
                                ) { Code = planMealType.MealType!.Code }
                            ),
                        plan.PlanRoomGroups!
                            .Where(
                                planRoomGroup => planRoomGroup.IsEnabled
                                    && planRoomGroup.RoomGroup!.IsEnabled
                                    && (filter.RoomGroupIds == null || filter.RoomGroupIds.Contains(planRoomGroup.RoomGroupId))
                            )
                            .OrderByDescending(planRoomGroup => planRoomGroup.RoomGroup!.DisplayOrder)
                            .Select(
                                planRoomGroup => new RoomGroupOfBookingPlanModel(
                                    planRoomGroup.RoomGroup!.Id,
                                    planRoomGroup.RoomGroup!.Name,
                                    planRoomGroup.RoomGroup!.GroupName,
                                    planRoomGroup.RoomGroup!.Tag,
                                    planRoomGroup.RoomGroup!.Description,
                                    planRoomGroup.RoomGroup!.Overview,
                                    planRoomGroup.RoomGroup!.DisplayOrder,
                                    planRoomGroup.RoomGroup!.IsEnabledSmoking,
                                    planRoomGroup.RoomGroup!.IsOverviewVisible,
                                    planRoomGroup.RoomGroup!.IsDescriptionVisible,
                                    planRoomGroup.RoomGroup!.IsRoomSizeVisible,
                                    planRoomGroup.RoomGroup!.IsBedTypeVisible,
                                    planRoomGroup.RoomGroup!.CapacityMin,
                                    planRoomGroup.RoomGroup!.CapacityMax,
                                    planRoomGroup.RoomGroup!.FileRoomGroups!
                                        .Where(fileRoomGroup => fileRoomGroup.File!.IsEnabled)
                                        .OrderBy(fileRoomGroup => fileRoomGroup.Index)
                                        .Select(
                                            fileRoomGroup => new MediaOfBookingPlanModel(
                                                fileRoomGroup.File!.Code,
                                                fileRoomGroup.File!.ContentType,
                                                fileRoomGroup.Index,
                                                fileRoomGroup.File!.IsEnabled
                                            )
                                        )
                                ) { Code = planRoomGroup.RoomGroup!.Code }
                            ),
                        plan.IsCancelSameAccept,
                        plan.CancelDayLimit,
                        plan.CancelLimit,
                        new BookingCancellationPolicyModel
                        {
                            Id = plan.Cancellation!.Id,
                            Name = plan.Cancellation!.Name,
                            CanOnLinePayment = plan.Cancellation!.CanOnLinePayment,
                            PaymentLimit = plan.Cancellation!.PaymentLimit,
                            TableSource = plan.Cancellation!.TableSource,
                            Description = plan.Cancellation!.Description,
                            RuleDetail = plan.Cancellation!.RuleDetail,
                            CancellationData = plan.Cancellation!.CancellationCancellationDatas!.Select(
                                y => new BookingCancellationDataPolicyModel
                                {
                                    DayEnd = y.CancellationData!.DayEnd,
                                    DayStart = y.CancellationData!.DayStart,
                                    Rate = y.CancellationData!.Rate
                                }
                            )
                        }
                    ) { Code = plan.Code }
                )
                .AsSingleQuery();

            return queryable;
        }
    }
}

public static class BookingSearchQueryableExtensions
{
    public static IQueryable<Plan> ApplyFilter(
        this IQueryable<Plan> queryable,
        BookingSearchQueryableFilterParameter filterParams
    )
    {
        var bookingSearchDate = DateTime.UtcNow.AddHours(
            DefaultValues.TimeZoneOffset
        );
        var bookingSearchDateId = AppDate.GetId(bookingSearchDate);

        queryable = ApplyBasicFilters(queryable)
            .ApplyDateRangeFilters(
                bookingSearchDateId,
                filterParams.RestNumber
            )
            .ApplyFacilityAndSiteFilters(
                filterParams.FacilityId,
                filterParams.SiteId
            )
            .ApplyPlanIdAndRoomIdFilters(
                filterParams.PlanIds,
                filterParams.RoomGroupIds
            )
            .ApplySecretFilters(
                filterParams.Secret,
                filterParams.ISecret
            )
            .ApplyUseDayFilters(filterParams.DayUse)
            .OrderByDescending(x => x.DisplayOrder);

        return queryable;
    }

    private static IQueryable<Plan> ApplyBasicFilters(
        IQueryable<Plan> queryable
    )
    {
        return queryable
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnLinePayment || x.IsOnSidePayment)
            .Where(x => x.Cancellation != null && x.Cancellation.IsEnabled)
            .Where(x => x.CheckInEnd != null && x.CheckInStart != null && x.CheckOut != null);
    }

    private static IQueryable<Plan> ApplyDateRangeFilters(
        this IQueryable<Plan> queryable,
        long bookingSearchDateId,
        int restNumber
    )
    {
        return queryable
            .Where(x => !x.UseDisplayDate || x.DisplayDateStart == null || x.DisplayDateStart <= bookingSearchDateId)
            .Where(x => !x.UseDisplayDate || x.DisplayDateEnd == null || x.DisplayDateEnd >= bookingSearchDateId)
            .Where(x => !x.NumberOfStayLimitMax.HasValue || x.NumberOfStayLimitMax >= restNumber)
            .Where(x => !x.NumberOfStayLimitMin.HasValue || x.NumberOfStayLimitMin <= restNumber);
    }

    private static IQueryable<Plan> ApplyFacilityAndSiteFilters(
        this IQueryable<Plan> queryable,
        long facilityId,
        long siteId
    )
    {
        return queryable
            .Where(x => x.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.PlanSites!.Any(t => t.SiteId == siteId))
            .Where(
                x => x.PlanRoomGroupSites!.Any(
                    t =>
                        t.SiteId == siteId && t.IsEnabled && t.RoomGroup!.IsEnabled
                )
            );
    }

    private static IQueryable<Plan> ApplyPlanIdAndRoomIdFilters(
        this IQueryable<Plan> queryable,
        long[]? planIds,
        long[]? roomGroupIds
    )
    {
        if (planIds is not null)
        {
            queryable = queryable.Where(x => planIds.Contains(x.Id));
        }

        if (roomGroupIds is not null)
        {
            queryable = queryable.Where(
                x => x.PlanRoomGroups!.Any(t => roomGroupIds.Contains(t.RoomGroupId))
            );
        }

        return queryable;
    }

    private static IQueryable<Plan> ApplySecretFilters(
        this IQueryable<Plan> queryable,
        string? secret,
        bool iSecret
    )
    {
        if (!iSecret)
        {
            return queryable;
        }

        return string.IsNullOrEmpty(secret)
            ? queryable.Where(x => !x.IsSecret)
            : queryable.Where(x => x.IsSecret && x.SecretWord == secret);
    }

    private static IQueryable<Plan> ApplyUseDayFilters(
        this IQueryable<Plan> queryable,
        bool? useDay
    )
    {
        return useDay is not null ? queryable.Where(x => x.DayUse == useDay) : queryable;
    }
}
