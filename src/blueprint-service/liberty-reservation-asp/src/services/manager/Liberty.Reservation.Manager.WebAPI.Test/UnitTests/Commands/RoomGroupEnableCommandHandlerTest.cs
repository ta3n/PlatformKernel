using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupEnableCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        var facilityKey = 1;
        var roomGroupId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupEnableCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanService = new Mock<IPlanService>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockPlanRoomGroupRepository = new Mock<IPlanRoomGroupRepository>();
        var mockCacheService = new Mock<ICacheService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);
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
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(roomGroup);

        var planForRoomOnly = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan For Room Only" } },
            Code = "Code"
        };

        mockPlanService.Setup(
                x => x.GetPlanWithRoomOnlyTypeAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(planForRoomOnly);

        var planRoomGroups = new List<PlanRoomGroup>
            {
                new()
                {
                    RoomGroupId = roomGroupId,
                    Plan = planForRoomOnly
                }
            }
            .AsQueryable()
            .BuildMock();

        mockPlanRoomGroupRepository.Setup(x => x.GetQueryable()).Returns(planRoomGroups);

        var enableRequest = new RoomGroupEnabledRequest(true);
        var command = new RoomGroupEnableCommand(roomGroupId) { Payload = enableRequest };

        var handler = new RoomGroupEnableCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupService.Object,
            mockPlanService.Object,
            mockPlanRoomGroupRepository.Object,
            mockCacheService.Object
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(roomGroupId, result);
    }
}
