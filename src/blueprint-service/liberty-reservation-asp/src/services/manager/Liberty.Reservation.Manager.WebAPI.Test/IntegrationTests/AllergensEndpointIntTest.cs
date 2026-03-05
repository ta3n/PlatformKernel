using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class AllergensEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/allergens";

    [Fact]
    public async Task GetAllergen_ReturnOk_WithAllergenResponse()
    {
        var allergenRepo = Factory.GetRequiredService<IAllergenRepository>();
        var allergen = new Allergen
        {
            Code = Guid.NewGuid().ToString(),
            Name = "Allergen"
        };

        await allergenRepo!.AddAsync(allergen, true);

        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
