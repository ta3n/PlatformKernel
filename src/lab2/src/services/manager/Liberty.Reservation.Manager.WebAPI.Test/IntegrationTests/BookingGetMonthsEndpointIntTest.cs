using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingGetMonthsEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingGetMonthsEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task GetAllMonthsOfReservations_ReturnOK_WithListOfMonthsResponse()
    {
        var plan = await MockBookingData.CreatePlanAsync();
        var roomGroup = await MockBookingData.CreateRoomGroupAsync();
        var site = await MockBookingData.CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);

        var reservationRepo = Factory.GetRequiredService<IReservationRepository>();
        for (var i = 0; i < 5; i++)
        {
            var mockReservation = new ReservationEntity
            {
                Code = EntityUtil.CreateCode(),
                Facility = FacilityInfo,
                Plan = plan,
                RoomGroup = roomGroup,
                Site = site,
                MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
                Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
                ReservationDateTime = DateTime.UtcNow,
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddMonths(i))
            };
            _ = await reservationRepo!.AddAsync(mockReservation, true);
        }

        var response = await Client.GetAsync($"{BaseUrl}/months");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
