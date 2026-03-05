using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;
using Liberty.SysException;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
using static Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests.GetBookingRequest;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.UnitTests;

public class KakusanGetAllBookingQueryHandlerTest
{
    private readonly KakusanGetAllBookingQueryHandler _handler;

    public KakusanGetAllBookingQueryHandlerTest()
    {
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var mapperMock = new Mock<IMapper>();
        var configSettingMock = new Mock<IOptions<C003Setting>>();

        configSettingMock
            .Setup(x => x.Value)
            .Returns(new C003Setting());

        var configurationProvider = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation,
                    GetBookingResponse.Booking>();
            }
        );
        var checkFacilityServiceMock = new Mock<ICheckFacilityService>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var cacheServiceMock = new Mock<ICacheService>();

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
        mapperMock
            .Setup(m => m.ConfigurationProvider)
            .Returns(configurationProvider);

        _handler = new KakusanGetAllBookingQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            securityContextAccessorMock.Object,
            configSettingMock.Object,
            reservationRepositoryMock.Object,
            facilityRepositoryMock.Object,
            checkFacilityServiceMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnData()
    {
        // Arrange
        var request = new GetBookingRequest
        {
            DateType = EnumDataType.ReserveOrCancel,
            ConvertOnlyFromDay = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            ConvertOnlyToDay = $"{DateTime.UtcNow.AddDays(1):yyyy-MM-dd HH:mm}",
            ConvertOnlyFromArriveDay = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            ConvertOnlyToArriveDay = $"{DateTime.UtcNow.AddDays(1):yyyy-MM-dd HH:mm}",
            HotelIds = ["mockCode"]
        };

        var query = new KakusanGetAllBookingQuery(request);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Item2);
        Assert.Equal(ErrorCode.E4001, result.Item2.ErrorCode);
        Assert.Empty(result.Item2.Hotels);
    }
}
