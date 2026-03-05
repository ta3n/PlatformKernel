using System.Net;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BedTypesEndpointIntTest : BaseIntegrationTest
{
    private string BaseUrl { get; set; } = "/api/bed-types";

    [Fact]
    public async Task GetAllBedTypes_ReturnsOk_WithBedTypeList()
    {
        var response = await Client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseString);
    }
}
