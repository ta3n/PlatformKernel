using System.Net;
using System.Xml.Linq;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.IntegrationTests;

public class KakusanGetRoomsIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/site-controller/GetRooms";

    [Fact]
    public async Task GetRooms_ReturnOk_WithRoomsResponse()
    {
        var facilityRepository = Factory.GetRequiredService<IFacilityRepository>();
        var facilityCode = await facilityRepository!.GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Select(x => x.Code)
            .SingleOrDefaultAsync();

        var roomRepo = Factory.GetRequiredService<IRoomGroupRepository>();
        var existingRooms = await roomRepo!.GetQueryableWithAsNoTracking().ToListAsync();
        existingRooms.ForEach(x => x.GroupName = $"RoomGroup{x.Id}");
        await roomRepo.UpdateRangeAsync(existingRooms, true);

        var createReq = new GetRoomsRequest
        {
            FromDay = DateTime.Now,
            ToDay = DateTime.Now.AddDays(1),
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

        var hotel = xdoc.Descendants("hotel").FirstOrDefault();
        Assert.NotNull(hotel);
        Assert.Equal(facilityCode, hotel.Attribute("id")?.Value);

        var rooms = hotel.Elements("room").ToList();
        Assert.Equal(5, rooms.Count);

        foreach (var room in rooms)
        {
            var roomId = room.Attribute("id")?.Value;
            Assert.NotEmpty(roomId!);

            var date = room.Element("date");
            Assert.NotNull(date);

            Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), date.Attribute("value")?.Value);
            Assert.Equal("3", date.Attribute("total")?.Value);
            Assert.Equal("3", date.Attribute("vacant")?.Value);
            Assert.Equal("0", date.Attribute("close")?.Value);
        }
    }
}
