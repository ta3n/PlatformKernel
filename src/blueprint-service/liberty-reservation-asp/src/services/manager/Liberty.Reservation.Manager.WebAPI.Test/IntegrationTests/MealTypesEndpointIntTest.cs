using System.Net;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class MealTypesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/meal-types";

    [Fact]
    public async Task GetMealType_ReturnOk_WithMealTypeResponse()
    {
        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
