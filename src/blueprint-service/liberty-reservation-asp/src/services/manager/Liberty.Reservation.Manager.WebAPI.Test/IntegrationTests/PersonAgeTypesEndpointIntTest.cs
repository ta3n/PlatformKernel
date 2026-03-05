using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class PersonAgeTypesEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/person-age-types";

    [Fact]
    public async Task GetAllPersonAgeTypeOfFacility_ReturnOK_WithPersonAgeTypes()
    {
        var personAgeTypeRepo = Factory.GetRequiredService<IPersonAgeTypeRepository>();
        var mockPersonAgeType = new PersonAgeType
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Code = EntityUtil.CreateCode()
        };
        var personAgeType = await personAgeTypeRepo!.AddAsync(mockPersonAgeType, true);

        var facilityPersonAgeRepo = Factory.GetRequiredService<IFacilityPersonAgeTypeRepository>();
        var facilityPersonAge = new FacilityPersonAgeType
        {
            FacilityId = FacilityInfo.Id,
            PersonAgeTypeId = personAgeType.Id
        };
        await facilityPersonAgeRepo!.AddAsync(facilityPersonAge, true);

        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
