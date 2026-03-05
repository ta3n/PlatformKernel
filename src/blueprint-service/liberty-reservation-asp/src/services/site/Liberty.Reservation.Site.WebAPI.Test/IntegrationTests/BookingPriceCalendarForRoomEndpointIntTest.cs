using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class BookingPriceCalendarForRoomEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    private GetBookingData GetBookingData { get; }

    public BookingPriceCalendarForRoomEndpointIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetBookingPriceCalendar_ReturnOk_WithBookingPriceCalendarResponse()
    {
        var plan = await GetBookingData.GetPlan(false, PlanTypes.RoomOnly);
        var roomGroup = await GetBookingData.GetRoomGroup();
        var checkInDate = plan.DisplayDateStart;
        var roomNumber = 1;
        List<PersonOfBookingSearchModel> rooms =
        [
            new()
            {
                AppDateId = plan.DisplayDateStart ?? AppDate.GetId(DateTime.UtcNow),
                RestIndex = 1,
                RoomGroupIndex = 0,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 0
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
            $"{BaseUrl}/rooms/{roomGroup.Id}/plans/{plan.Id}/price-calendar",
            payload
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
