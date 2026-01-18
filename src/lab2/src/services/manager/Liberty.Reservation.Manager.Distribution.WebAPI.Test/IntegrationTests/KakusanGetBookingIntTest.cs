using System.Net;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.IntegrationTests;

public class KakusanGetBookingIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/site-controller/GetBooking";

    [Fact]
    public async Task GetBooking_ReturnOk_WithBookingResponse()
    {
        var facilityRepository = Factory.GetRequiredService<IFacilityRepository>();
        var facilityCode = facilityRepository!.GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Select(x => x.Code)
            .SingleOrDefault();

        var createReq = new GetBookingRequest
        {
            DateType = GetBookingRequest.EnumDataType.ReserveOrCancel,
            ConvertOnlyFromDay = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
            ConvertOnlyToDay = $"{DateTime.UtcNow.AddDays(1):yyyy-MM-dd HH:mm:ss}",
            ConvertOnlyFromArriveDay = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
            ConvertOnlyToArriveDay = $"{DateTime.UtcNow.AddDays(1):yyyy-MM-dd HH:mm:ss}",
            HotelIds =
            [
                facilityCode ?? string.Empty
            ]
        };

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToXmlContent(createReq)
        );

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
