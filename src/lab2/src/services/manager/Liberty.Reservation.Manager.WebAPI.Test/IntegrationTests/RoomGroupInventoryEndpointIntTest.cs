using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class RoomGroupInventoryEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/room-group-inventory";

    [Fact]
    public async Task AdjustAppDatesOfRoomGroups_ReturnNoContent()
    {
        var roomGroupOfFacilityRepo = Factory.GetRequiredService<IFacilityRoomGroupRepository>();
        var roomGroupOfFacility = new FacilityRoomGroup
        {
            FacilityId = FacilityInfo.Id,
            RoomGroup = new()
            {
                Code = Guid.NewGuid().ToString(),
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
                IsEnabled = true
            }
        };
        var newRoomGroupOfFacility = await roomGroupOfFacilityRepo!.AddAsync(
            roomGroupOfFacility,
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

        var putRequest = new List<RoomGroupChangeRemainRequest>
        {
            new(
                newAppDate.Id,
                newRoomGroupOfFacility.RoomGroupId,
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
}
