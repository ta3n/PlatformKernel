using System.Net;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

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
    public async Task CreateCategory_ReturnsOk_WithCategory()
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
    public async Task UpdateCategory_ReturnsOk_WithCategory()
    {
        var newCategoryJson = await CreateCategoryAsync();
        _ = long.TryParse(newCategoryJson, out var newCategoryId);

        var updateReq = new CategoryUpdateRequest(
            "Category 2",
            "Category 2 Description"
        ) { Id = newCategoryId };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{newCategoryId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableCategory_ReturnsOk_WithCategory()
    {
        var newCategoryJson = await CreateCategoryAsync();
        _ = long.TryParse(newCategoryJson, out var newCategoryId);

        var updateReq = new CategoryEnabledRequest(true) { Id = newCategoryId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newCategoryId}/enable",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsOk_WithCategory()
    {
        var newCategoryJson = await CreateCategoryAsync();
        _ = long.TryParse(newCategoryJson, out var newCategoryId);

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{newCategoryId}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsOk_WithCategoryList()
    {
        await CreateCategoryAsync();

        var response = await Client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllEnableCategories_ReturnsOk_WithCategoryList()
    {
        await CreateCategoryAsync();

        var response = await Client.GetAsync($"{BaseUrl}/?enable=true");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetCategory_ReturnsOk_WithCategory()
    {
        var newCategoryJson = await CreateCategoryAsync();
        _ = long.TryParse(newCategoryJson, out var newCategoryId);

        var response = await Client.GetAsync($"{BaseUrl}/{newCategoryId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
