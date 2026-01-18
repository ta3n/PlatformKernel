using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class OptionItemInventoryEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/option-item-inventory";

    [Fact]
    public async Task AdjustAppDatesOfOptionItems_ReturnNoContent()
    {
        var optionItemOfFacilityRepo = Factory.GetRequiredService<IFacilityOptionItemRepository>();
        var optionItemOfFacility = new FacilityOptionItem
        {
            FacilityId = FacilityInfo.Id,
            OptionItem = new()
            {
                Code = Guid.NewGuid().ToString(),
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                IsEnabled = true
            }
        };

        var newOptionItemOfFacility = await optionItemOfFacilityRepo!.AddAsync(
            optionItemOfFacility,
            true
        );

        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();
        var dateData = DateTime.UtcNow.Date;
        var appDate = new AppDate
        {
            Id = AppDate.GetId(dateData),
            DateTime = dateData
        };
        var newAppDate = await appDateRepo!.AddAsync(
            appDate,
            true
        );

        var putRequest = new List<OptionItemChangeRemainRequest>
        {
            new(
                newAppDate.Id,
                newOptionItemOfFacility.OptionItemId,
                0,
                false
            )
        };

        var response = await Client.PutAsync(
            BaseUrl,
            TestUtil.ToJsonContent(putRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllFacilities_ReturnOk_WithResponse()
    {
        var response = await Client.GetAsync(
            $"{BaseUrl}?facilityId={FacilityInfo.Id}&startAppDateId=20241013&endAppDateId=20281014"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
