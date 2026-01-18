using System.Globalization;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Site.Application.Exceptions;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class PlanService(
    IFacilityPlanRepository facilityPlanRepository,
    IPlanRoomGroupRepository planRoomGroupRepository,
    IPlanRepository planRepository,
    ISystemConfigRepository systemConfigRepository
) : IPlanService
{
    public async Task<(bool IsAvailable, PlanTypes PlanType)> CheckPlanAlreadyAsync(
        long facilityId,
        long planId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityPlanRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.Facility!.IsEnabled)
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.IsEnabled)
            .Select(
                x => new
                {
                    x.PlanId,
                    x.Plan!.PlanType
                }
            );

        var existingPlan = await queryable.SingleOrDefaultAsync(cancellationToken);

        return (existingPlan?.PlanId > 0, existingPlan?.PlanType ?? PlanTypes.Combo);
    }

    public async Task<TimeSpan?> GetCheckOutTimeAsync(
        long planId,
        CancellationToken cancellationToken
    )
    {
        var checkOutTime = await planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Select(x => x.CheckOut)
            .SingleOrDefaultAsync(cancellationToken);

        return checkOutTime;
    }

    public async Task<Plan?> GetPlanDetailCancellation(
        long planId,
        CancellationToken cancellationToken
    )
    {
        var plan = await planRepository.GetQueryableWithAsNoTracking()
            .Include(p => p.Cancellation!)
            .ThenInclude(c => c.CancellationCancellationDatas!)
            .ThenInclude(ccd => ccd.CancellationData)
            .Include(x => x.PlanMealTypes!)
            .ThenInclude(t => t.MealType!)
            .FirstOrDefaultAsync(
                x => x.Id == planId,
                cancellationToken
            );

        return plan;
    }

    public async Task<bool> IsValidCheckInTimeAsync(
        long planId,
        long checkInDate,
        CancellationToken cancellationToken
    )
    {
        var plan = await planRepository.GetQueryableWithAsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == planId,
                cancellationToken
            );

        if (plan == null)
        {
            return false;
        }

        if (plan.CheckInEnd == null)
        {
            return true;
        }

        var queryable = facilityPlanRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Facility!.IsEnabled)
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.IsEnabled)
            .Select(
                x => new
                {
                    x.PlanId,
                    x.Plan!.PlanType,
                    x.Facility!.TimeZone
                }
            );

        var existingPlan = await queryable.SingleOrDefaultAsync(cancellationToken);

        var checkInDateAtFacility = AppDate
            .GetDateTime(checkInDate)
            .Date;

        var todayAtFacility = DateTime.UtcNow
            .Add(existingPlan!.TimeZone ?? DefaultValues.DefaultTimeZoneOffset)
            .Date;

        if (checkInDateAtFacility > todayAtFacility)
        {
            return true;
        }
        var time = DateTime.UtcNow.Add(existingPlan!.TimeZone ?? DefaultValues.DefaultTimeZoneOffset).TimeOfDay;

        return plan.CheckInEnd > time;
    }

    public async Task<bool> CheckRoomAlreadyInPlanAsync(
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.RoomGroupId == roomGroupId)
            .Where(x => x.RoomGroup!.IsEnabled)
            .Where(x => x.IsEnabled);

        var count = await queryable.CountAsync(cancellationToken);

        return count == 1;
    }

    public async Task<bool> CheckPaymentOnlinePaymentAvailableAsync(
        long facilityId,
        long planId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityPlanRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.Facility!.IsEnabled)
            .Where(x => x.Facility!.CanOnLinePayment)
            .Where(x => x.Facility!.IsOnLinePayment)
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.Plan!.IsOnLinePayment)
            .Where(x => x.IsEnabled);

        var glocalCanOnlinePayment = await systemConfigRepository
                .GetQueryableWithAsNoTracking()
                .Select(x => x.CanOnlinePayment)
                .SingleOrDefaultAsync(cancellationToken)
            ?? throw new SystemConfigNotfoundException();
        var count = await queryable.CountAsync(cancellationToken);

        return count is 1 && glocalCanOnlinePayment;
    }

    public async Task<Plan?> GetPlanAtCreateBookingAsync(
        long planId,
        CancellationToken cancellationToken
    )
    {
        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Select(
                x => new Plan
                {
                    CheckOut = x.CheckOut,
                    PlanMealTypes = x.PlanMealTypes!.Select(
                            y => new PlanMealType
                            {
                                MealType = new MealType { Name = y.MealType!.Name },
                                MealTypeEatType = y.MealTypeEatType
                            }
                        )
                        .ToList(),
                    Cancellation = new Cancellation
                    {
                        Description = x.Cancellation!.Description,
                        TableSource = x.Cancellation.TableSource
                    }
                }
            );
        var existingPlan = await queryable.SingleOrDefaultAsync(cancellationToken);

        return existingPlan;
    }

    public async Task<bool> CheckPaymentOnSitePaymentAvailableAsync(
        long facilityId,
        long planId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityPlanRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.Facility!.IsEnabled)
            .Where(x => x.Facility!.IsOnSidePayment)
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.Plan!.IsOnSidePayment)
            .Where(x => x.IsEnabled);

        var count = await queryable.CountAsync(cancellationToken);

        return count is 1;
    }
}
