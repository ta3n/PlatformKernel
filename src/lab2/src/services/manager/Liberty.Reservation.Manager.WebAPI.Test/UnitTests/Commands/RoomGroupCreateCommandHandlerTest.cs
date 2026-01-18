using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupCreateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        var facilityKey = 1;
        var roomGroupId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupCreateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockFacilityRoomGroupService = new Mock<IFacilityRoomGroupService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        // InfrastructureOfTest IFacilityService: trả về count > 0
        mockFacilityService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var roomGroup = new RoomGroup
        {
            Id = roomGroupId,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room group" } },
            GroupName = "Group name",
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            CapacityMax = 10,
            CapacityMin = 1,
            BaseNumber = 1,
            Code = "Code"
        };

        mockRoomGroupService.Setup(
                x =>
                    x.CreateRoomGroupWithFacilityAsync(
                        It.IsAny<RoomGroup>(),
                        It.IsAny<long>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(roomGroup);

        mockFacilityRoomGroupService.Setup(
                x =>
                    x.CheckGroupNameExisting(
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(false);

        var createRequest = new RoomGroupCreateRequest(
            "Name",
            "Description",
            "Group Name",
            1,
            10,
            1
        );

        var command = new RoomGroupCreateCommand { Payload = createRequest };

        var handler = new RoomGroupCreateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object,
            mockRoomGroupService.Object,
            mockFacilityRoomGroupService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(roomGroupId, result);
    }
}
