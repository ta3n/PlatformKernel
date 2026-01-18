using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class BookingSearchByRoomEndpointIntTest : BaseIntegrationTest
{
    private GetBookingData GetBookingData { get; }
    private const string BaseUrl = "api/booking";

    public BookingSearchByRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetAllBooking_ReturnOk_WithBookingResponse()
    {
        var plan = await GetBookingData.GetPlan();
        var checkInDate = plan.DisplayDateStart;
        var roomNumber = 1;
        List<PersonOfBookingSearchModel> rooms =
        [
            new()
            {
                AppDateId = plan.DisplayDateStart ?? AppDate.GetId(DateTime.UtcNow),
                RestIndex = 1,
                RoomGroupIndex = 1,
                PersonAgeTypeId = 2,
                Persons = 2,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];

        var displayCheckInDate = checkInDate ?? AppDate.GetId(DateTime.UtcNow);
        var displayCheckOutDate = checkInDate.HasValue
            ? AppDate.GetId(AppDate.GetDateTime(checkInDate)?.AddDays(1)) ?? 0
            : AppDate.GetId(DateTime.UtcNow.AddDays(1));

        var bookingRequest = new BookingSearchPlanRequest
        {
            CheckInDate = displayCheckInDate,
            CheckOutDate = displayCheckOutDate,
            DisplayCheckInDate = displayCheckInDate,
            DisplayCheckOutDate = displayCheckOutDate,
            Secret = null,
            MaxPrice = null,
            MinPrice = null,
            OptionItems = null,
            RestNumber = 1,
            RoomNumber = roomNumber,
            GuestsPerRoom = rooms
        };
        var payload = TestUtil.ToJsonContent(bookingRequest);
        var response = await Client.PostAsync(
            $"{BaseUrl}/search-by-room",
            payload
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
