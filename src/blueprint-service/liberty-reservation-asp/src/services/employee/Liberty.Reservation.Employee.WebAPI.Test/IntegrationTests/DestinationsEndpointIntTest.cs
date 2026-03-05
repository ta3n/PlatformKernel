using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class DestinationsEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/destinations";

    private async Task<string> CreateDestinationAsync()
    {
        var createReq = new DestinationCreateRequest(
            "Name",
            "Short Name",
            "https://yopaz.vn"
        );

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToJsonContent(createReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }

    [Fact]
    public async Task CreateDestination_ReturnsOk()
    {
        var responseString = await CreateDestinationAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderDestination_Return_NoContent()
    {
        var newDestination1 = await CreateDestinationAsync();
        _ = long.TryParse(newDestination1, out var newDestinationId1);

        var newDestination2 = await CreateDestinationAsync();
        _ = long.TryParse(newDestination2, out var newDestinationId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newDestinationId1, newDestinationId2
            ]
        );

        var response = await Client.PutAsync(
            $"{BaseUrl}/order",
            TestUtil.ToJsonContent(orderRequest)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDestination_ReturnNoContent()
    {
        var destinationId = await CreateDestinationAsync();

        var updateReq = new DestinationUpdateRequest(
            EntityUtil.CreateCode(),
            "Name",
            "Short Name",
            "https://yopaz.vn"
        ) { Id = long.Parse(destinationId) };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{destinationId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableDestination_ReturnNoContent()
    {
        var destinationId = await CreateDestinationAsync();

        var enableReq = new DestinationEnabledRequest(
            true
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{destinationId}/enable",
            TestUtil.ToJsonContent(enableReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDestination_ReturnNoContent()
    {
        var destinationId = await CreateDestinationAsync();

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{destinationId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllDestinations_ReturnOK_WithDestinations()
    {
        await CreateDestinationAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllEnableDestinations_ReturnOK_WithDestinations()
    {
        await CreateDestinationAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetDestination_ReturnOK_WithDestination()
    {
        var destinationId = await CreateDestinationAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{destinationId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
