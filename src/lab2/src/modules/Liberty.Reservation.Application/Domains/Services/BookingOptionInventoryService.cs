using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingOptionInventoryService(
    IBookingDataMetaRepository bookingDataMetaRepository
) : IBookingOptionInventoryService
{
    public async Task ValidateOptionInventoryAsync(
        OptionInventoryModel optionInventoryModel,
        CancellationToken cancellationToken
    )
    {
        var (facilityId, siteId, planId, checkInDate, restNumber, options, existingReservationId) = (
            optionInventoryModel.FacilityId,
            optionInventoryModel.SiteId,
            optionInventoryModel.PlanId,
            optionInventoryModel.CheckInDate,
            optionInventoryModel.RestNumber,
            optionInventoryModel.Options,
            optionInventoryModel.ExistingReservationId
        );

        var checkOutDateId = AppDate.GetId(AppDate.GetDateTime(checkInDate).AddDays(restNumber));

        var bookingSearch = new BookingSearchPlanRequest
        {
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDateId,
            OptionItems = options,
            UseCache = false
        };

        var optionItems = (await bookingDataMetaRepository.GetAllBookingMetaOptionItemModelsAsync(
            facilityId,
            siteId,
            [planId],
            bookingSearch,
            existingReservationId,
            cancellationToken
        )).ToList();

        var optionGroups = options.GroupBy(o => (o.AppDateId, o.OptionItemId));

        foreach (var optionGroup in optionGroups)
        {
            var requestedNumber = optionGroup.Sum(o => o.Number);

            var remainNumber = optionItems
                .SelectMany(x => x.AppDates)
                .Where(
                    ad => ad.AppDateId == optionGroup.Key.AppDateId
                        && ad.OptionItemId == optionGroup.Key.OptionItemId
                )
                .Sum(ad => ad.RemainNumber);

            var maxSupplyNumber = optionItems
                .SelectMany(x => x.AppDates)
                .First(ad => ad.AppDateId == optionGroup.Key.AppDateId)
                .MaxSupplyNumber;

            if (
                requestedNumber > remainNumber
                || (
                    existingReservationId is null
                    && maxSupplyNumber is not null
                    && optionGroup.Any(og => og.Number > maxSupplyNumber)
                )
            )
            {
                throw new ReservationOperationNumberOverRemainException();
            }
        }
    }

    public IEnumerable<BookingReservationOptionItemData> GetAllReservationOptionItemData(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates,
        long? existingReservationId
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var optionItemsOfReservation = new List<BookingReservationOptionItemData>();

        foreach (var (appDateId, rooms) in bookingAdjustRequest.NightOptions ?? [])
        {
            var roomOptionRequests = rooms as RoomOptionOfReservationAdjustRequest[] ?? [.. rooms];

            foreach (var (roomIndex, optionItems) in roomOptionRequests)
            {
                foreach (var (optionItemId, number) in optionItems)
                {
                    var reservationOptionItem = ProcessOptionItem(
                        appDateId,
                        roomIndex,
                        optionItemId,
                        number,
                        bookingDataAvailable
                    );

                    optionItemsOfReservation.Add(reservationOptionItem);
                }
            }
        }

        return optionItemsOfReservation;
    }

    private static BookingReservationOptionItemData ProcessOptionItem(
        long appDateId,
        int roomIndex,
        long optionItemId,
        int number,
        BookingPlanModel bookingDataAvailable
    )
    {
        var optionItem = GetOptionItemById(
            appDateId,
            optionItemId,
            bookingDataAvailable
        );
        var price = optionItem?.OptionItemPrice ?? 0;

        var totalPrice = price * number;

        return new BookingReservationOptionItemData
        {
            AppDateId = appDateId,
            RoomGroupIndex = roomIndex,
            Price = price,
            Number = number,
            TotalPrice = totalPrice,
            OptionItemInfo = new()
            {
                Id = optionItemId,
                Name = optionItem?.OptionItemName,
                Price = price
            }
        };
    }

    private static BookingMetaPlanOptionItemAppDate? GetOptionItemById(
        long appDateId,
        long optionItemId,
        BookingPlanModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
            .OptionItems
            .SelectMany(x => x.AppDates)
            .Where(x => x.AppDateId == appDateId)
            .SingleOrDefault(x => x.OptionItemId == optionItemId);
    }
}
