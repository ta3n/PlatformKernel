using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BathingTaxAgeEndpointInitTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/bathing-tax-ages";

    private async Task<string> BathingTaxAgeCreate()
    {
        var meta = new MetaOfBathingTaxAgeUpdateRequest("test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest("PersonAgeType", 99, 10, null, false, meta, spas);
        var contentReq = TestUtil.ToJsonContent(bathingTaxAgeAddRequest);
        var response = await Client.PostAsync(BaseUrl, contentReq);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    [Fact]
    public async Task BathingTaxAgeCreate_ReturnOk_WithBathingTaxAgeResponse()
    {
        var responseString = await BathingTaxAgeCreate();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task BathingTaxAgeUpdate_ReturnOk_WithBathingTaxAgeResponse()
    {
        var responseStringCreate = await BathingTaxAgeCreate();
        _ = long.TryParse(responseStringCreate, out var newPersonAgeTypeId);

        var meta = new MetaOfBathingTaxAgeUpdateRequest("test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Baby);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(200, 6000, 50),
            new(220, 6640, 550)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(
            newPersonAgeTypeId,
            "PersonAgeTypeUpdate",
            99,
            10,
            false,
            meta,
            spas
        );
        var contentReq = TestUtil.ToJsonContent(bathingTaxAgeUpdateRequest);
        var response = await Client.PutAsync(BaseUrl, contentReq);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task BathingTaxAge_ReturnOk_WithBathingTaxAgeResponse()
    {
        await BathingTaxAgeCreate();
        var response = await Client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task BathingTaxAgeChangeStatus_ReturnOk()
    {
        var responseStringCreate = await BathingTaxAgeCreate();
        _ = long.TryParse(responseStringCreate, out var newPersonAgeTypeId);
        var payload = new BathingTaxAgeChangeStatusRequest(
            newPersonAgeTypeId,
            false
        );
        var contentReq = TestUtil.ToJsonContent(payload);
        var response = await Client.PatchAsync($"{BaseUrl}/change-status", contentReq);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task BathingTaxAgeChangeSettingFacility_ReturnOk()
    {
        var payload = new BathingTaxAgeChangeSettingFacilityRequest(
            true,
            "test",
            "test"
        );
        var contentReq = TestUtil.ToJsonContent(payload);
        var response = await Client.PatchAsync($"{BaseUrl}/change-facility-setting", contentReq);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
