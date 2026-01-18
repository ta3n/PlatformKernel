using System.Net;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class CancellationPolicyEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/cancellation-policies";

    private async Task<string> CreateCancellationAsync()
    {
        var cancellation = new CancellationPolicyCreateRequest(
            "Cancel 1",
            "Cancel 1 Description"
        );

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToJsonContent(cancellation)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }

    [Fact]
    public async Task CreateCancellation_ReturnsOk_WithCancellationId()
    {
        var responseString = await CreateCancellationAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderCancellation_Return_NoContent()
    {
        var newCancellationJson1 = await CreateCancellationAsync();
        _ = long.TryParse(newCancellationJson1, out var newCancellationId1);

        var newCancellationJson2 = await CreateCancellationAsync();
        _ = long.TryParse(newCancellationJson2, out var newCancellationId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newCancellationId1, newCancellationId2
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
    public async Task UpdateCancellation_ReturnNoContent()
    {
        var cancellationId = await CreateCancellationAsync();
        var id = long.Parse(cancellationId);

        var updateReq = new CancellationPolicyUpdateRequest(
            "Cancellation 2",
            "Cancellation 2 Description",
            "Rule Detail",
            new List<DataOfCancellationUpdateRequest>
            {
                new(
                    1,
                    10,
                    11,
                    1,
                    "Description",
                    0
                )
            }
        ) { Id = id };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{id}",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllCancellation_ReturnOk_WithListCancellationResponse()
    {
        _ = await CreateCancellationAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllEnableCancellation_ReturnOk_WithListCancellationResponse()
    {
        _ = await CreateCancellationAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetCancellation_ReturnOk_WithCancellationResponse()
    {
        var cancellationIdString = await CreateCancellationAsync();
        var cancellationId = long.Parse(cancellationIdString);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{cancellationId}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task EnableCancellation_ReturnNoContent()
    {
        var cancellationIdString = await CreateCancellationAsync();
        var cancellationId = long.Parse(cancellationIdString);

        var enableReq = new CancellationPolicyEnabledRequest(false);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{cancellationId}/enable",
            TestUtil.ToJsonContent(enableReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCancellation_ReturnNoContent()
    {
        var cancellationIdString = await CreateCancellationAsync();
        var cancellationId = long.Parse(cancellationIdString);

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{cancellationId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
