using System.Net;
using System.Xml.Linq;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.IntegrationTests;

public class KakusanGetRoomTypeIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/site-controller/GetRoomType";

    [Fact]
    public async Task GetAllergen_ReturnOk_WithAllergenResponse()
    {
        var facilityRepository = Factory.GetRequiredService<IFacilityRepository>();
        var facilityCode = facilityRepository!.GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Select(x => x.Code)
            .SingleOrDefault();

        var roomRepo = Factory.GetRequiredService<IRoomGroupRepository>();
        var existingRooms = roomRepo!.GetQueryableWithAsNoTracking().ToList();
        existingRooms.ForEach(x => x.GroupName = $"RoomGroup{x.Id}");
        _ = await roomRepo.UpdateRangeAsync(existingRooms, true);

        var createReq = new RoomTypeRequest
        {
            HotelIds =
            [
                facilityCode ?? string.Empty
            ]
        };

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToXmlContent(createReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
        var xdoc = XDocument.Parse(responseString);

        var hotelElement = xdoc.Descendants("hotel").FirstOrDefault();
        Assert.NotNull(hotelElement);
        Assert.Equal(facilityCode, hotelElement.Attribute("id")?.Value);

        var rooms = hotelElement.Elements("room").ToList();
        Assert.Equal(5, rooms.Count);

        Assert.Equal("Room group 1", rooms[0].Attribute("name")?.Value);
        Assert.Equal("10", rooms[0].Attribute("rooms")?.Value);
        Assert.Equal("600", rooms[0].Attribute("people")?.Value);
    }
}
