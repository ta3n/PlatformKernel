using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupUpdateDisplaySettingCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        var roomGroupId = 1;
        var facilityId = 100;

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupUpdateDisplaySettingCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockPlanService = new Mock<IPlanService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var payload = new RoomGroupUpdateDisplaySettingRequest(
            [1, 2],
            [3, 4],
            [5],
            [6],
            [7],
            ["Tag1", "Tag2"]
        ) { Id = roomGroupId };

        var command = new RoomGroupUpdateDisplaySettingCommand { Payload = payload };

        mockCategoryService.Setup(
                x => x.CountMasterByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), false, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(6);

        mockCategoryService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var roomGroup = new RoomGroup
        {
            Id = roomGroupId,
            Tag = "Tag1"
        };

        mockRoomGroupService.Setup(x => x.UpdateDisplaySettingOfRoomGroupAsync(It.IsAny<RoomGroup>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomGroup);

        var plan = new Plan
        {
            Id = 10,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan For Room Only" } },
            Tag = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "" } }
        };

        mockPlanService.Setup(x => x.GetPlanWithRoomOnlyTypeAsync(roomGroupId, facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var handler = new RoomGroupUpdateDisplaySettingCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupService.Object,
            mockCategoryService.Object,
            mockPlanService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(roomGroupId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCategoryNotfoundException()
    {
        var roomGroupId = 1;
        var facilityId = 100;

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupUpdateDisplaySettingCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockPlanService = new Mock<IPlanService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var payload = new RoomGroupUpdateDisplaySettingRequest(
            [1, 2],
            [3, 4],
            [5],
            [6],
            [7],
            ["Tag1", "Tag2"]
        ) { Id = roomGroupId };

        var command = new RoomGroupUpdateDisplaySettingCommand { Payload = payload };

        mockCategoryService.Setup(
                x => x.CountMasterByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), false, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(6);

        mockCategoryService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var roomGroup = new RoomGroup
        {
            Id = roomGroupId,
            Tag = "Tag1"
        };

        mockRoomGroupService.Setup(x => x.UpdateDisplaySettingOfRoomGroupAsync(It.IsAny<RoomGroup>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomGroup);

        var plan = new Plan
        {
            Id = 10,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan For Room Only" } },
            Tag = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "" } }
        };

        mockPlanService.Setup(x => x.GetPlanWithRoomOnlyTypeAsync(roomGroupId, facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var handler = new RoomGroupUpdateDisplaySettingCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupService.Object,
            mockCategoryService.Object,
            mockPlanService.Object
        );

        await Assert.ThrowsAsync<CategoryNotfoundException>(
            () =>
                handler.Handle(command, CancellationToken.None)
        );
    }
}
