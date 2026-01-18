using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingCancellationReturnReservationNotFoundExceptionEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "/api/reservations";
    private MockBookingData MockBookingData { get; }

    public BookingCancellationReturnReservationNotFoundExceptionEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task BookingCancelAndRefundWithMethodPaymentOnline_ReturnsReservationNotFoundException()
    {
        var reservation = (await MockBookingData.CreateReservationsOnlinePaymentAsync()).Reservations.First();

        var orderId = await MockBookingData.CreateOrderReservationAsync(reservation.Id);

        await MockBookingData.CreatePaymentOnlineAsync(orderId);

        var bookingAdjustReq = new BookingCancellationRequest { Id = reservation.Id };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{reservation.Id + 100}/cancellation",
            TestUtil.ToJsonContent(bookingAdjustReq)
        );

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            var exception = JsonConvert.DeserializeObject<ReservationNotfoundException>(json);

            Assert.NotNull(exception);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }
}
