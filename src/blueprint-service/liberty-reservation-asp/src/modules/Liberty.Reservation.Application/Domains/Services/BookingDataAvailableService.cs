using System.Linq.Expressions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingDataAvailableService(
    IDbContextFactory<DbContext> dbContextFactory,
    IBookingDataPlanRepository dataPlanRepository,
    IBookingOptionItemRepository optionItemRepository,
    IBookingRoomGroupRepository roomGroupRepository,
    IBookingReservationRepository reservationRepository
) : IBookingDataAvailableService
{
    public async Task<IEnumerable<OptionItemAppDate>> FindAllAppDatesOfOptionItemsAsync(
        long facilityId,
        long[] optionItemIds,
        long startAppDateCheckId,
        long endAppDateCheckId,
        long ignoreReservationId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = optionItemRepository
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.IsEnabled)
            .Where(
                x => x.FacilityOptionItems!.Any(
                    t => t.FacilityId == facilityId
                        && t.Facility!.IsEnabled
                        && t.IsEnabled
                )
            )
            .Where(x => optionItemIds.Contains(x.Id))
            .SelectMany(
                x => x.OptionItemAppDates!
                    .Select(
                        y => new OptionItemAppDate
                        {
                            AppDateId = y.AppDateId,
                            IsEnabled = y.IsEnabled,
                            IsNotSelled = y.IsNotSelled,
                            SellNumber = y.SellNumber,
                            OptionItemId = y.OptionItemId,
                            OptionItem = new OptionItem
                            {
                                Id = y.OptionItem!.Id,
                                Name = y.OptionItem!.Name,
                                Price = y.OptionItem!.Price
                            },
                            ReservationRoomGroupAppDateOptionItems = x.ReservationRoomGroupAppDateOptionItems!
                                .Where(t => t.Reservation!.CheckInDate + t.RestIndex == y.AppDateId && t.ReservationId != ignoreReservationId)
                                .Where(
                                    t =>
                                        t.Reservation!.ReservationState == ReservationStatus.Confirmed
                                        || t.Reservation!.ReservationState == ReservationStatus.Reserved
                                        || t.Reservation!.ReservationState == ReservationStatus.Modified
                                )
                                .Select(
                                    z => new ReservationRoomGroupAppDateOptionItem
                                    {
                                        ReservationId = z.ReservationId,
                                        RoomGroupIndex = z.RoomGroupIndex,
                                        Number = z.Number,
                                        Reservation = new ReservationEntity { ReservationState = z.Reservation!.ReservationState }
                                    }
                                )
                                .ToList()
                        }
                    )
            )
            .Where(x => x.AppDateId >= startAppDateCheckId)
            .Where(x => x.AppDateId <= endAppDateCheckId)
            .Where(x => x.IsEnabled)
            .Where(x => !x.IsNotSelled)
            .Where(x => x.SellNumber > 0)
            .OrderBy(x => x.AppDateId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<RoomGroupAppDate>> FindAllAppDatesOfRoomGroupAsync(
        long facilityId,
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.Id == roomGroupId)
            .Where(x => x.IsEnabled)
            .Where(
                x => x.FacilityRoomGroups!.Any(
                    t => t.FacilityId == facilityId
                        && t.Facility!.IsEnabled
                        && t.IsEnabled
                )
            )
            .SelectMany(x => x.RoomGroupAppDates!)
            .Include(x => x.RoomGroup)
            .Where(x => x.IsEnabled)
            .Where(x => x.AppDateId >= startAppDateCheckId)
            .Where(x => x.AppDateId <= endAppDateCheckId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<PlanRoomGroupSitePersonAgeType>> FindAllPersonAgeTypesOfSiteInPlanRoomAsync(
        long facilityId,
        long siteId,
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dataPlanRepository
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.Id == planId)
            .Where(x => x.IsEnabled)
            .Where(
                x => x.FacilityPlans!.Any(
                    t => t.FacilityId == facilityId
                        && t.Facility!.IsEnabled
                        && t.IsEnabled
                )
            )
            .Where(
                x => x.PlanSites!.Any(
                    t => t.SiteId == siteId
                        && t.Site!.IsEnabled
                        && t.IsEnabled
                )
            )
            .Where(
                x => x.PlanRoomGroups!.Any(
                    t => t.RoomGroupId == roomGroupId
                        && t.RoomGroup!.IsEnabled
                        && t.IsEnabled
                )
            )
            .SelectMany(x => x.PlanRoomGroupSitePersonAgeTypes!)
            .Include(x => x.PersonAgeType!)
            .ThenInclude(x => x.PersonAgeTypeSpaTaxDatas!)
            .ThenInclude(x => x.SpaTaxData!)
            .Include(x => x.PersonAgeType!)
            .ThenInclude(x => x.FacilityPersonAgeTypes!)
            .ThenInclude(x => x.Facility!)
            .Where(x => x.SiteId == siteId)
            .Where(x => x.RoomGroupId == roomGroupId)
            .Where(x => x.IsEnabled)
            .Where(x => x.PersonAgeType!.IsEnabled && x.PersonAgeType!.IsVisible);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<ReservationEntity> GetReservationByUserCodeAsync(
        long id,
        string? userCode,
        CancellationToken cancellationToken = default
    )
    {
        return await GetReservationAsync(
            x => x.Id == id && x.UserCode == userCode,
            cancellationToken
        );
    }

    public async Task<ReservationEntity> GetReservationByFacilityIdAsync(
        long id,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return await GetReservationAsync(
            x => x.Id == id && x.FacilityId == facilityId,
            cancellationToken
        );
    }

    private async Task<ReservationEntity> GetReservationAsync(
        Expression<Func<ReservationEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        return await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(predicate)
                .Select(
                    x => new ReservationEntity
                    {
                        FacilityId = x.Facility!.Id,
                        PlanId = x.Plan!.Id,
                        SiteId = x.SiteId,
                        RoomGroupId = x.RoomGroupId,
                        CheckInDate = x.CheckInDate,
                        CheckInTime = x.CheckInTime,
                        ReservationState = x.ReservationState,
                        RoomNumber = x.RoomNumber,
                        Plan = new Plan
                        {
                            Id = x.Plan!.Id,
                            PlanType = x.Plan!.PlanType,
                            UseAcceptPersonNumber = x.Plan!.UseAcceptPersonNumber,
                            AcceptPersonNumberMin = x.Plan!.AcceptPersonNumberMin,
                            AcceptPersonNumberMax = x.Plan!.AcceptPersonNumberMax,
                            NumberOfStayLimitMin = x.Plan!.NumberOfStayLimitMin,
                            NumberOfStayLimitMax = x.Plan!.NumberOfStayLimitMax,
                            IsOnLinePayment = x.Plan!.IsOnLinePayment,
                            IsOnSidePayment = x.Plan!.IsOnSidePayment,
                            UseDaySaleLimit = x.Plan!.UseDaySaleLimit,
                            PlanDaySaleLimitType = x.Plan!.PlanDaySaleLimitType,
                            RoomNumberDaySaleLimit = x.Plan!.RoomNumberDaySaleLimit,
                            UseDisplayDate = x.Plan!.UseDisplayDate,
                            DisplayDateStart = x.Plan!.DisplayDateStart,
                            DisplayDateEnd = x.Plan!.DisplayDateEnd,
                            UseAcceptDate = x.Plan!.UseAcceptDate,
                            AcceptDateStart = x.Plan!.AcceptDateStart,
                            AcceptDateEnd = x.Plan!.AcceptDateEnd,
                            GroupNumberDaySaleLimit = x.Plan!.GroupNumberDaySaleLimit,
                            ReceptionLimit = x.Plan.ReceptionLimit,
                            ReceptionDayLimit = x.Plan.ReceptionDayLimit
                        },
                        Facility = new Facility
                        {
                            UseDailyPerson = x.Facility.UseDailyPerson,
                            CanAddRoomOnModify = x.Facility.CanAddRoomOnModify
                        },
                        BookingData = new Contexts.DataContexts.Entities.Metas.BookingData
                        {
                            Facility = new Contexts.DataContexts.Entities.Metas.FacilityData(),
                            Site = new Contexts.DataContexts.Entities.Metas.SiteData(),
                            RoomGroup = new Contexts.DataContexts.Entities.Metas.RoomGroupData(),
                            Plan = new Contexts.DataContexts.Entities.Metas.PlanData(),
                            TimeZoneOffset = x.BookingData!.TimeZoneOffset
                        }
                    }
                )
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationNotfoundException();
    }
}
