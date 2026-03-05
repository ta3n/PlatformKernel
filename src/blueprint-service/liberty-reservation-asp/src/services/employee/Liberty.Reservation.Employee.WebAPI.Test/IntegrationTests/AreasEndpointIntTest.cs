using System.Net;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class AreasEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/areas";

    private async Task<string> CreateAreaAsync()
    {
        var createReq = new AreaCreateRequest(
            "Name"
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
    public async Task CreateArea_ReturnsOk()
    {
        var responseString = await CreateAreaAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderArea_Return_NoContent()
    {
        var newArea1Json = await CreateAreaAsync();
        _ = long.TryParse(newArea1Json, out var newAreaId1);

        var newArea2Json = await CreateAreaAsync();
        _ = long.TryParse(newArea2Json, out var newAreaId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newAreaId1, newAreaId2
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
    public async Task UpdateArea_ReturnNoContent()
    {
        var areaId = await CreateAreaAsync();

        var updateReq = new AreaUpdateRequest(
            "Name update"
        ) { Id = long.Parse(areaId) };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{areaId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableArea_ReturnNoContent()
    {
        var areaId = await CreateAreaAsync();

        var enableReq = new AreaEnabledRequest(
            true
        ) { Id = long.Parse(areaId) };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{areaId}/enable",
            TestUtil.ToJsonContent(enableReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteArea_ReturnNoContent()
    {
        var areaId = await CreateAreaAsync();

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{areaId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllArea_ReturnOK_WithAreas()
    {
        await CreateAreaAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllEnableArea_ReturnOK_WithAreas()
    {
        await CreateAreaAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetArea_ReturnOK_WithArea()
    {
        var areaId = await CreateAreaAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{areaId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
