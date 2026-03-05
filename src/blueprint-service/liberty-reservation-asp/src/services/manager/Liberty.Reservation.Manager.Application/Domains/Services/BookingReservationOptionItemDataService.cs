using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingReservationOptionItemDataService : IBookingReservationOptionItemDataService
{
    public IEnumerable<BookingReservationOptionItemData> GetAllReservationOptionItemData(
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var optionItemsOfReservation = new List<BookingReservationOptionItemData>();

        foreach (var (appDateId, rooms) in bookingAdjustRequest.NightOptions ?? [])
        {
            foreach (var (roomIndex, optionItems) in rooms)
            {
                foreach (var (optionItemId, number) in optionItems)
                {
                    var optionItem = GetOptionItemById(
                        optionItemId,
                        bookingDataAvailable
                    );
                    var price = optionItem?.Price ?? 0;

                    var optionItemAppDate = GetOptionItemAppDate(
                        appDateId,
                        optionItemId,
                        bookingDataAvailable
                    );
                    var optionNumber = optionItemAppDate?.RemainNumber ?? 0;

                    if (number > optionNumber)
                    {
                        throw new ReservationOperationNumberOverRemainException(number, optionNumber);
                    }

                    var optionItemTotalPrice = price * number;

                    var reservationOptionItem = new BookingReservationOptionItemData
                    {
                        AppDateId = appDateId,
                        RoomGroupIndex = roomIndex,
                        Price = price,
                        Number = number,
                        TotalPrice = optionItemTotalPrice,
                        OptionItemInfo = new()
                        {
                            Id = optionItemId,
                            Name = optionItem?.Name,
                            Price = price
                        }
                    };

                    optionItemsOfReservation.Add(reservationOptionItem);
                }
            }
        }

        return optionItemsOfReservation;
    }

    private static OptionItem? GetOptionItemById(
        long optionItemId,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
            .AppDatesOfOptionItems
            .Select(x => x.OptionItem)
            .SingleOrDefault(x => x?.Id == optionItemId);
    }

    private static OptionItemAppDate? GetOptionItemAppDate(
        long appDateId,
        long optionItemId,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
            .AppDatesOfOptionItems
            .Where(x => x.OptionItemId == optionItemId && x.AppDateId == appDateId)
            .SingleOrDefault(x => x.RemainNumber != null);
    }
}
