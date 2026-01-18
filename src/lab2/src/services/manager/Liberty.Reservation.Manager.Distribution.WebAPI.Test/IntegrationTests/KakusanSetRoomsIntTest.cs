using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.IntegrationTests;

public class KakusanSetRoomsIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/site-controller/SetRooms";

    [Fact]
    public async Task SetRooms_ReturnOk_WithRoomsSetResponse()
    {
        var roomRepo = Factory.GetRequiredService<IRoomGroupRepository>();
        var existingRooms = roomRepo!.GetQueryableWithAsNoTracking().ToList();
        existingRooms.ForEach(x => x.GroupName = $"RoomGroup{x.Id}");
        await roomRepo.UpdateRangeAsync(existingRooms, true);

        var createReq = new SetRoomsRequest
        {
            Hotels =
            [
                new()
                {
                    HotelId = MockFacilityCode,
                    RoomId = existingRooms[0].GroupName,
                    AdjustType = HotelModel.EnumAdjustType.RelativeDown,
                    FromDay = $"{DateTime.Now.AddDays(2):yyyy-MM-dd}",
                    Days = 1,
                    RoomCount = 1
                }
            ]
        };

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToXmlContent(createReq)
        );

        Assert.NotNull(response);
    }
}
