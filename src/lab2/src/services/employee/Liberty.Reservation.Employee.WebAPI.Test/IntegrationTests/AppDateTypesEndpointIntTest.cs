using System.Net;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class AppDateTypesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/app-date-types";

    private async Task<string> CreateAppDateAsync()
    {
        var createReq = new AppDateTypeCreateRequest(
            "Type 1",
            "T1",
            "#F00",
            "Description"
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
    public async Task CreateAppDateType_ReturnsOk()
    {
        var responseString = await CreateAppDateAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderAppDateType_Return_NoContent()
    {
        var newAppDate1Json = await CreateAppDateAsync();
        _ = long.TryParse(newAppDate1Json, out var newAppDateId1);

        var newAppDate2Json = await CreateAppDateAsync();
        _ = long.TryParse(newAppDate2Json, out var newAppDateId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newAppDateId1, newAppDateId2
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
    public async Task UpdateAppDateType_ReturnNoContent()
    {
        var appDateTypeId = await CreateAppDateAsync();

        var updateReq = new AppDateTypeUpdateRequest(
            "Type 1 update",
            "T1 update",
            "Red update",
            "Description Update"
        ) { Id = long.Parse(appDateTypeId) };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{appDateTypeId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableAppDateType_ReturnNoContent()
    {
        var appDateTypeId = await CreateAppDateAsync();

        var enableReq = new AppDateTypeEnabledRequest(
            true
        ) { Id = long.Parse(appDateTypeId) };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{appDateTypeId}/enable",
            TestUtil.ToJsonContent(enableReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAppDateType_ReturnNoContent()
    {
        var appDateTypeId = await CreateAppDateAsync();

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{appDateTypeId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllAppDateType_ReturnOK_WithAppDateTypes()
    {
        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllEnableAppDateType_ReturnOK_WithAppDateTypes()
    {
        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAppDateType_ReturnOK_WithAppDateType()
    {
        var appDateTypeId = await CreateAppDateAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{appDateTypeId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
