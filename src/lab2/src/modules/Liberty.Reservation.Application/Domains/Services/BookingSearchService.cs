using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingSearchService(
    IBookingDataPlanRepository dataPlanRepository,
    IBookingDataMetaRepository bookingDataMetaRepository,
    IBookingDetailService bookingDetailService,
    IBookingReservationService bookingReservationService
) : IBookingSearchService
{
    public async Task<IPage<BookingPlanModel>> GetAllBookingDataPlansAsync(
        long facilityId,
        long siteId,
        bool isLoadingPriceAppDate,
        BookingSearchPlanRequest search,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var timeZone = bookingReservationService.GetFacilityTimeZoneById(facilityId);

        var page = await GetPageBookingPlansAsync(
            new BookingDataPlanFilterParameter(
                facilityId,
                siteId,
                null,
                null,
                true,
                isLoadingPriceAppDate,
                timeZone,
                search
            ),
            pageable,
            cancellationToken
        );

        return page;
    }

    public async Task<IPage<BookingPlanModel>> GetAllBookingDataPlansAsync(
        long facilityId,
        long siteId,
        bool isLoadingPriceAppDate,
        BookingSearchPriceCalendarRequest search,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var timeZone = bookingReservationService.GetFacilityTimeZoneById(facilityId);
        var planIds = search.GetPlanIds().ToArray();
        var roomGroupIds = search.GetRoomIds().ToArray();

        var page = await GetPageBookingPlansAsync(
            new BookingDataPlanFilterParameter(
                facilityId,
                siteId,
                planIds,
                roomGroupIds,
                true,
                isLoadingPriceAppDate,
                timeZone,
                search.BookingSearch
            ),
            pageable,
            cancellationToken
        );

        return page;
    }

    public async Task<BookingPlanModel?> GetBookingDataDetailByPlanAsync(
        BookingPlanDetailRequest request,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        var timeZone = bookingReservationService.GetFacilityTimeZoneById(request.FacilityId);

        var page = await GetPageBookingPlansAsync(
            new BookingDataPlanFilterParameter(
                request.FacilityId,
                request.SiteId,
                [request.PlanId],
                [request.RoomGroupId],
                false,
                true,
                timeZone,
                search
            ),
            Pageable.Of(1, 1, true),
            cancellationToken
        );

        var data = page.Content.FirstOrDefault();

        if (data?.DayUse is true && search.RestNumber is not 1)
        {
            throw new BookingInvalidNightNumberOfUseDayException();
        }

        return data;
    }

    private async Task<IPage<BookingPlanModel>> GetPageBookingPlansAsync(
        BookingDataPlanFilterParameter filter,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var facilityId = filter.FacilityId;
        var siteId = filter.SiteId;

        var systemCanOnlinePaymentTask = bookingDetailService.GetSystemCanOnlinePaymentAsync(cancellationToken);
        var facilityStateTask = bookingDetailService.GetFacilitySateAsync(
            filter.FacilityId,
            cancellationToken
        );

        await Task.WhenAll(
            systemCanOnlinePaymentTask,
            facilityStateTask
        );

        var systemCanOnlinePayment = systemCanOnlinePaymentTask.Result;
        var facilityState = facilityStateTask.Result;
        var allowOnlinePayment = facilityState.IsOnLinePayment && facilityState.CanOnLinePayment && systemCanOnlinePayment;
        var allowOnSidePayment = facilityState.IsOnSidePayment;
        var paymentNotAllowed = !allowOnlinePayment && !allowOnSidePayment;

        if (paymentNotAllowed)
        {
            return new Page<BookingPlanModel>([], pageable, 0);
        }

        var page = await dataPlanRepository.GetPageBookingPlansAsync(
            filter with
            {
                SystemCanOnlinePayment = systemCanOnlinePayment,
                FacilityState = facilityState
            },
            pageable,
            cancellationToken
        );

        if (!filter.IsLoadingPriceAppDate)
        {
            return page;
        }

        var content = page.Content.ToList();

        var planIds = content.Select(x => x.Id).ToArray();
        var roomGroupIds = content
            .SelectMany(x => x.RoomGroups)
            .Select(x => x.Id)
            .Distinct()
            .ToArray();

        // Use strongly-typed tasks instead of Task[] to avoid casting overhead
        var priceDataTask = bookingDataMetaRepository.GetAllBookingMetaPriceDataModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            filter.Search,
            cancellationToken
        );
        var discountDataTask = bookingDataMetaRepository.GetAllBookingMetaDiscountDataModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            filter.Search.UseCache,
            cancellationToken
        );
        var roomAppDateTask = bookingDataMetaRepository.GetAllBookingMetaRoomAppDateModelsAsync(
            facilityId,
            siteId,
            roomGroupIds,
            filter.Search,
            cancellationToken
        );
        var planAppDateTask = bookingDataMetaRepository.GetAllBookingMetaPlanAppDateModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            filter.Search,
            cancellationToken
        );
        var personTypeTask = bookingDataMetaRepository.GetAllBookingMetaPersonTypeModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            facilityState.UseSpaTax,
            filter.Search.UseCache,
            cancellationToken
        );
        var optionItemTask = bookingDataMetaRepository.GetAllBookingMetaOptionItemModelsAsync(
            facilityId,
            siteId,
            planIds,
            filter.Search,
            null,
            cancellationToken
        );
        var reservationTask = bookingDataMetaRepository.GetAllBookingMetaReservationModelsAsync(
            facilityId,
            siteId,
            planIds,
            filter.Search,
            cancellationToken
        );

        // Wait for all tasks to complete
        await Task.WhenAll(
            priceDataTask,
            discountDataTask,
            roomAppDateTask,
            planAppDateTask,
            personTypeTask,
            optionItemTask,
            reservationTask
        );

        // Create lookup dictionaries for O(1) access instead of O(n) LINQ Where operations
        var priceDataLookup = priceDataTask.Result.ToLookup(
            x => new
            {
                x.PlanId,
                x.RoomGroupId
            }
        );
        var discountDataLookup = discountDataTask.Result.ToLookup(
            x => new
            {
                x.PlanId,
                x.RoomGroupId
            }
        );
        var roomAppDateLookup = roomAppDateTask.Result.ToLookup(
            x => x.RoomGroupId
        );
        var planAppDateLookup = planAppDateTask.Result.ToLookup(
            x => new
            {
                x.PlanId,
                x.RoomGroupId
            }
        );
        var personTypeLookup = personTypeTask.Result.ToLookup(
            x => new
            {
                x.PlanId,
                x.RoomGroupId
            }
        );
        var optionItemLookup = optionItemTask.Result.ToLookup(x => x.PlanId);
        var reservationLookup = reservationTask.Result.ToLookup(x => x.RoomGroupId);

        // Efficiently assign data using lookups
        foreach (var planModel in content)
        {
            var roomIds = planModel.RoomGroups.Select(x => x.Id).ToArray();

            foreach (var roomModel in planModel.RoomGroups)
            {
                var roomKey = roomModel.Id;
                var planRoomKey = new
                {
                    PlanId = planModel.Id,
                    RoomGroupId = roomModel.Id
                };

                roomModel.AppDates = [.. roomAppDateLookup[roomKey]];
                roomModel.PlanAppDates = [.. planAppDateLookup[planRoomKey]];
                roomModel.PriceData = [.. priceDataLookup[planRoomKey]];
                roomModel.DiscountData = [.. discountDataLookup[planRoomKey]];
                roomModel.PersonTypes = [.. personTypeLookup[planRoomKey]];
            }

            planModel.OptionItems = [.. optionItemLookup[planModel.Id]];
            planModel.Reservations = [.. roomIds.SelectMany(roomId => reservationLookup[roomId])];

            planModel.FacilityCode = facilityState.Code;
        }

        return page;
    }
}
