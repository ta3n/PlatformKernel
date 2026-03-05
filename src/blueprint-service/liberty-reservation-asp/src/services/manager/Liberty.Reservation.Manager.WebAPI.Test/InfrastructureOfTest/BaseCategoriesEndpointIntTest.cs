using System.Net;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public abstract class BaseCategoriesEndpointIntTest(
    string categoryUrl
) : BaseIntegrationTest
{
    private string BaseUrl { get; set; } = categoryUrl;

    private async Task<string> CreateCategoryAsync()
    {
        var createReq = new CategoryCreateRequest(
            "Category 1",
            "Category 1 Description"
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
    public async Task CreateCategory_ReturnsOk_WithCategoryId()
    {
        var responseString = await CreateCategoryAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderCategory_Return_NoContent()
    {
        var newCategory1Json = await CreateCategoryAsync();
        _ = long.TryParse(newCategory1Json, out var newCategoryId1);

        var newCategory2Json = await CreateCategoryAsync();
        _ = long.TryParse(newCategory2Json, out var newCategoryId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newCategoryId1, newCategoryId2
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
    public async Task UpdateCategory_ReturnNoContent()
    {
        var newCategoryId = await CreateCategoryAsync();
        var id = long.Parse(newCategoryId);

        var updateReq = new CategoryUpdateRequest(
            "Category 2",
            "Category 2 Description"
        ) { Id = id };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{id}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetCategory_ReturnOk_WithCategoryResponse()
    {
        var newCategoryId = await CreateCategoryAsync();
        var id = long.Parse(newCategoryId);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{id}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllCategories_ReturnOk_WithCategoryResponseList()
    {
        _ = await CreateCategoryAsync();

        var response = await Client.GetAsync(
            BaseUrl
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllEnableCategories_ReturnOk_WithCategoryResponseList()
    {
        _ = await CreateCategoryAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
