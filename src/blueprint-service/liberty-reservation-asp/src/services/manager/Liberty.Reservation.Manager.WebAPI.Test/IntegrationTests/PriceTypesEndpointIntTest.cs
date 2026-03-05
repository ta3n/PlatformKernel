using System.Net;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class PriceTypesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/price-types";

    private async Task<string> CreatePriceTypeAsync()
    {
        var createReq = new PriceTypeCreateRequest(
            "TestName",
            "Desc",
            "#dfe"
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
    public async Task CreatePriceType_ReturnNoContent()
    {
        var responseString = await CreatePriceTypeAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task UpdatePriceType_ReturnNoContent()
    {
        var priceTypeId = await CreatePriceTypeAsync();

        var updateReq = new PriceTypeUpdateRequest(
            "TestUpdate",
            "DescUpdate",
            "#bgf"
        ) { Id = long.Parse(priceTypeId) };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{priceTypeId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePriceType_ReturnNoContent()
    {
        await CreatePriceTypeAsync();
        var priceTypeId = "1";

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{priceTypeId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllPriceType_ReturnOK_WithPriceTypes()
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
    public async Task GetAllEnablePriceType_ReturnOK_WithPriceTypes()
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
    public async Task GetPriceType_ReturnOK_WithPriceType()
    {
        var priceTypeId = await CreatePriceTypeAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{priceTypeId}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
