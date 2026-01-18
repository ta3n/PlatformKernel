using System.Net;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingCancellationEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "/api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingCancellationEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task BookingCancelAndRefundWithMethodPaymentOnline_ReturnsNoContent()
    {
        var reservation = (await MockBookingData.CreateReservationsOnlinePaymentAsync()).Reservations.First();
        await MockBookingData.CreateSystemConfigAsync();
        var orderId = await MockBookingData.CreateOrderReservationAsync(reservation.Id);

        await MockBookingData.CreatePaymentOnlineAsync(orderId);

        var bookingAdjustReq = new BookingCancellationRequest { Id = reservation.Id };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{reservation.Id}/cancellation",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
