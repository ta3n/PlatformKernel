using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.UnitTests;

public class KakusanGetAllRoomQueryHandlerTest
{
    private readonly KakusanGetAllRoomQueryHandler _handler;

    public KakusanGetAllRoomQueryHandlerTest()
    {
        var roomGroupAppDateRepositoryMock = new Mock<IRoomGroupAppDateRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();
        var configSettingMock = new Mock<IOptions<C002Setting>>();
        var checkFacilityServiceMock = new Mock<ICheckFacilityService>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        configSettingMock
            .Setup(x => x.Value)
            .Returns(new C002Setting());
        var facilityRoomGroups = new List<FacilityRoomGroup>
        {
            new()
            {
                RoomGroup = new RoomGroup
                {
                    Id = 200,
                    BaseNumber = 5,
                    CapacityMax = 3,
                    Name = new()
                    {
                        { "ja", "部屋A" },
                        { "en", "Room A" }
                    }
                }
            }
        };

        var facilities = new List<Facility>
        {
            new()
            {
                Id = 1,
                FacilityRoomGroups = facilityRoomGroups
            }
        };

        var mockQueryable = facilities.AsQueryable().AsEnumerable().BuildMock();

        facilityRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(mockQueryable);
        _handler = new KakusanGetAllRoomQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            securityContextAccessorMock.Object,
            configSettingMock.Object,
            roomGroupAppDateRepositoryMock.Object,
            facilityRepositoryMock.Object,
            checkFacilityServiceMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnData()
    {
        // Arrange
        var request = new GetRoomsRequest
        {
            HotelIds = ["mockcode"],
            FromDay = DateTime.Now,
            ToDay = DateTime.Now.AddDays(5)
        };

        var query = new KakusanGetAllRoomQuery(request);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Item2);
    }
}
