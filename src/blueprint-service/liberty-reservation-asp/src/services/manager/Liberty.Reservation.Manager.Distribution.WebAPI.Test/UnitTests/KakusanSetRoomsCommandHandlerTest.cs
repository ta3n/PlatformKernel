using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Kakusan;
using Liberty.UnitOfWork.Abstractions;
using MassTransit;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
using static Liberty.Reservation.Application.Models.Responses.HotelModel;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.UnitTests;

public class KakusanSetRoomsCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenValidationFails()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var mapperMock = new Mock<IMapper>();
        var configSettingMock = new Mock<IOptions<C004Setting>>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var bookingRoomAppDateServiceMock = new Mock<IBookingRoomAppDateService>();
        var busMock = new Mock<IBus>();
        var roomAdjustmentServiceMock = new Mock<IRoomAdjustmentStatusService>();
        var appDateServiceMock = new Mock<IAppDateService>();

        // Setup config
        var configSetting = new C004Setting();
        configSettingMock.Setup(x => x.Value).Returns(configSetting);

        var facilities = new List<Facility>
            {
                new()
                {
                    Id = 1,
                    Code = "HTL001",
                    IsEnabled = true
                }
            }
            .AsQueryable()
            .AsEnumerable()
            .BuildMock();

        // Create mocks for required services
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var cacheServiceMock = new Mock<ICacheService>();
        var checkFacilityServiceMock = new Mock<ICheckFacilityService>();
        checkFacilityServiceMock
            .Setup(x => x.CheckUserManagedFacilityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        serviceProviderMock.Setup(s => s.GetService(typeof(ISecurityContextAccessor)))
            .Returns(securityContextAccessorMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(ICacheService)))
            .Returns(cacheServiceMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(ICheckFacilityService)))
            .Returns(checkFacilityServiceMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(IOptions<C004Setting>)))
            .Returns(configSettingMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(IRoomAdjustmentStatusService)))
            .Returns(roomAdjustmentServiceMock.Object);
        facilityRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(facilities);

        var request = new KakusanSetRoomsCommand { Payload = new SetRoomsRequest() };

        var handler = new KakusanSetRoomsCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            serviceProviderMock.Object,
            bookingRoomAppDateServiceMock.Object,
            facilityRepositoryMock.Object,
            appDateServiceMock.Object,
            busMock.Object
        );

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateRooms_WhenValidAndCanUpdate()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var mapperMock = new Mock<IMapper>();
        var configSettingMock = new Mock<IOptions<C004Setting>>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var bookingRoomAppDateServiceMock = new Mock<IBookingRoomAppDateService>();
        var roomAdjustmentServiceMock = new Mock<IRoomAdjustmentStatusService>();
        var appDateServiceMock = new Mock<IAppDateService>();
        var busMock = new Mock<IBus>();

        // Setup config
        var configSetting = new C004Setting();
        configSettingMock.Setup(x => x.Value).Returns(configSetting);

        var facilities = new List<Facility>
            {
                new()
                {
                    Id = 1,
                    Code = "HTL001",
                    IsEnabled = true
                }
            }
            .AsQueryable()
            .AsEnumerable()
            .BuildMock();

        bookingRoomAppDateServiceMock
            .Setup(x => x.UpdateSetRoomsAsync(It.IsAny<SetRoomModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new() { CanUpdate = true },
                    [],
                    []
                )
            );
        // Create mocks for required services
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var cacheServiceMock = new Mock<ICacheService>();
        var checkFacilityServiceMock = new Mock<ICheckFacilityService>();
        checkFacilityServiceMock
            .Setup(x => x.CheckUserManagedFacilityAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        serviceProviderMock.Setup(s => s.GetService(typeof(ISecurityContextAccessor)))
            .Returns(securityContextAccessorMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(ICacheService)))
            .Returns(cacheServiceMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(ICheckFacilityService)))
            .Returns(checkFacilityServiceMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(IOptions<C004Setting>)))
            .Returns(configSettingMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(IRoomAdjustmentStatusService)))
            .Returns(roomAdjustmentServiceMock.Object);
        facilityRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(facilities);
        var roomGroupAppDateRepositoryMock = new Mock<IRoomGroupAppDateRepository>();
        var mockRoomGroupAppDates = new List<RoomGroupAppDate>
        {
            new()
            {
                RoomGroupId = 1,
                AppDateId = 20260101,
                SellNumber = 10,
                IsNotSelled = false,
                RoomGroup = new RoomGroup
                {
                    Id = 1,
                    Code = "Room001",
                    GroupName = "Room001",
                    FacilityRoomGroups = new List<FacilityRoomGroup>
                    {
                        new()
                        {
                            FacilityId = 1,
                            Facility = new Facility { Code = "HTL001" }
                        }
                    },
                    BaseNumber = 100
                },
                ReservationPlanRoomGroupAppDates = new List<ReservationPlanRoomGroupAppDate>
                {
                    new() { Reservation = new() { ReservationState = Reservation.Application.Constants.ReservationStatus.Confirmed } }
                }
            }
        };

        var mockQueryable = mockRoomGroupAppDates.AsQueryable().BuildMock();
        roomGroupAppDateRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(mockQueryable);

        // Arrange
        var request = new KakusanSetRoomsCommand
        {
            Payload = new SetRoomsRequest
            {
                Hotels =
                [
                    new()
                    {
                        Key = 1,
                        HotelId = "HTL001",
                        RoomId = "Room001",
                        FromDay = "2026-01-01",
                        Days = 1,
                        AdjustType = EnumAdjustType.RelativeUp,
                        RoomCount = 1
                    }
                ]
            }
        };
        var handler = new KakusanSetRoomsCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            serviceProviderMock.Object,
            bookingRoomAppDateServiceMock.Object,
            facilityRepositoryMock.Object,
            appDateServiceMock.Object,
            busMock.Object
        );

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EnumResultType.Success, result.Result);
    }
}
