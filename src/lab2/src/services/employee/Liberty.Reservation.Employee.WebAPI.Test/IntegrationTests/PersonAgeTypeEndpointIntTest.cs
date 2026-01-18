using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class PersonAgeTypeEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/person-age-type";

    [Fact]
    public async Task GetListPersonAgeTypeMaster_ReturnOk()
    {
        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resonseString = await response.Content.ReadAsStringAsync();

        var personAgeTypeResponses = JsonSerializer.Deserialize<List<PersonAgeTypeResponse>>(
            resonseString,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            }
        );

        Assert.NotNull(personAgeTypeResponses);
    }

    [Fact]
    public async Task UpdatePersonAgeType_ReturnsOk()
    {
        var existingId = await CreatePersonAgeTypeAsync();

        var updateRequest = new PersonAgeTypeUpdateRequest(
            "Updated Name",
            15,
            25,
            true,
            new MetaOfBathingTaxAgeUpdateRequest("GroupTest", FoodBeds.Food, FoodBeds.Bed, 1),
            new List<SpaOfBathingTaxAgeUpdateRequest>
            {
                new(0, 1000, 100),
                new(1001, 2000, 200)
            }
        ) { Id = long.Parse(existingId) };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{existingId}",
            TestUtil.ToJsonContent(updateRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeletePersonAgeType_ReturnsNoContent()
    {
        var existingId = await CreatePersonAgeTypeAsync();

        var response = await Client.DeleteAsync($"{BaseUrl}/{existingId}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreatePersonAgeType_ReturnsOk()
    {
        var responseString = await CreatePersonAgeTypeAsync();
        Assert.NotNull(responseString);
    }

    private async Task<string> CreatePersonAgeTypeAsync()
    {
        var createRequest = new PersonAgeTypeCreateRequest(
            "Initial Name",
            1,
            14,
            true,
            new MetaOfBathingTaxAgeUpdateRequest("GroupA", FoodBeds.Food, FoodBeds.Bed, 8),
            new List<SpaOfBathingTaxAgeUpdateRequest>
            {
                new(100000, 200000, 5),
                new(150000, 300000, 8)
            }
        );

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToJsonContent(createRequest)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }
}
