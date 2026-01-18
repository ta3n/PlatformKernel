using System.Net;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class OptionItemsEndpointIntTest : BaseIntegrationTest
{
    private string BaseUrl { get; set; } = "/api/option-items";

    private async Task<string> CreateOptionItemAsync()
    {
        var createReq = new OptionItemCreateRequest(
            "Option Item 1",
            "Option Item 1 Description",
            4,
            600
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
    public async Task CreateOptionItem_ReturnsOk_WithOptionItem()
    {
        var responseString = await CreateOptionItemAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderOptionItem_Return_NoContent()
    {
        var newOptionItemJson1 = await CreateOptionItemAsync();
        _ = long.TryParse(newOptionItemJson1, out var newOptionItemId1);

        var newOptionItemJson2 = await CreateOptionItemAsync();
        _ = long.TryParse(newOptionItemJson2, out var newOptionItemId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newOptionItemId1, newOptionItemId2
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
    public async Task UpdateOptionItem_ReturnsOk_WithOptionItem()
    {
        var newOptionItemResponse = await CreateOptionItemAsync();
        var newOptionItemId = long.TryParse(newOptionItemResponse, out var id) ? id : 0;

        var updateReq = new OptionItemUpdateRequest(
            "Option Item 2",
            "Option Item 2 Description",
            4,
            2,
            600,
            [],
            [],
            [],
            []
        ) { Id = newOptionItemId };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{newOptionItemId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableOptionItem_ReturnsNoContent_WithOptionItem()
    {
        var newOptionItemResponse = await CreateOptionItemAsync();
        var newOptionItemId = long.TryParse(newOptionItemResponse, out var id) ? id : 0;

        var updateReq = new OptionItemEnabledRequest(true) { Id = newOptionItemId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newOptionItemId}/enable",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOptionItem_ReturnsOk_WithOptionItem()
    {
        var newOptionItemResponse = await CreateOptionItemAsync();
        var newOptionItemId = long.TryParse(newOptionItemResponse, out var id) ? id : 0;

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{newOptionItemId}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllOptionItems_ReturnsOk_WithOptionItemList()
    {
        await CreateOptionItemAsync();

        var response = await Client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllEnableOptionItems_ReturnsOk_WithOptionItemList()
    {
        await CreateOptionItemAsync();

        var response = await Client.GetAsync($"{BaseUrl}/?enable=true");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetOptionItem_ReturnsOk_WithOptionItem()
    {
        var newOptionItemResponse = await CreateOptionItemAsync();
        var newOptionItemId = long.TryParse(newOptionItemResponse, out var id) ? id : 0;

        var response = await Client.GetAsync($"{BaseUrl}/{newOptionItemId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
