using System.Net;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class GetBookingDetailsEndpointReturnRoomGroupNotfoundIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking";

    private GetBookingData GetBookingData { get; }

    public GetBookingDetailsEndpointReturnRoomGroupNotfoundIntTest()
    {
        GetBookingData = new GetBookingData(Factory, Client);
    }

    [Fact]
    public async Task GetBookingDetails_ReturnRoomGroupNotfoundException()
    {
        var plan = await GetBookingData.GetPlan();

        var roomGroupRepo = Factory.GetRequiredService<IRoomGroupRepository>()
            ?? throw new ArgumentException(nameof(IRoomGroupRepository));

        var idMax = await roomGroupRepo
            .GetQueryableWithAsNoTracking()
            .MaxAsync(x => x.Id);

        var url = $"{BaseUrl}/plans/{plan.Id}/rooms/{idMax + 100}";
        var response = await Client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
