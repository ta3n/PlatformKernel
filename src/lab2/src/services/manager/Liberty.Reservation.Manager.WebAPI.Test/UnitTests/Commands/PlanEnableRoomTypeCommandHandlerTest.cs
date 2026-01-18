using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanEnableRoomTypeCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        // Arrange
        var facilityKey = 1;
        var planId = 1;

        // Mocks
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();
        var mockCacheService = new Mock<ICacheService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var planRoom = new PlanRoomGroup
        {
            PlanId = planId,
            RoomGroupId = 1,
            IsEnabled = true
        };
        mockPlanRoomGroupService.Setup(
                x => x.FindByPlanIdAndRomTypeIdAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(planRoom);

        mockPlanRoomGroupService.Setup(x => x.UpdateAsync(It.IsAny<PlanRoomGroup>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planRoom);

        var request = new PlanEnabledRoomTypeRequest(true);
        var command = new PlanEnabledRoomTypeCommand(1, 1) { Payload = request };
        var handler = new PlanEnabledRoomTypeCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockPlanRoomGroupService.Object,
            mockSecurityContextAccessor.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
