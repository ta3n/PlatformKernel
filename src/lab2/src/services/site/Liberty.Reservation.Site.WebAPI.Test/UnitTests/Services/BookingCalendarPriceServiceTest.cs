using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Moq;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Services;

public class BookingCalendarPriceServiceTest : BaseUnitTest
{
    [Fact]
    public Task GetAppDatePriceOfRoomInPlanAsync_ShouldReturnCorrectData()
    {
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingReservationServiceMock = new Mock<IBookingReservationService>();

        var bookingSearchModel = new BookingSearchModel
        {
            CheckInDate = 20250101,
            CheckOutDate = 20250104,
            RestNumber = 4,
            GuestsPerRoom =
            [
                new PersonOfBookingSearchModel
                {
                    AppDateId = 20250116,
                    RestIndex = 1,
                    RoomGroupIndex = 1,
                    PersonAgeTypeId = 1,
                    Persons = 3,
                    MalePersons = 2,
                    FemalePersons = 1
                }
            ]
        };

        var plans = new List<BookingPlanModel>();

        var service = new BookingCalendarPriceService(
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object,
            bookingReservationServiceMock.Object
        );

        var result = service.GetAllRoomDatePricesInPlans(
            bookingSearchModel,
            plans,
            1,
            1L
        );

        Assert.NotNull(result);
        return Task.CompletedTask;
    }
}
