using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Moq;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupUpdateSpecialCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var roomGroupId = 2;
        var planId = 20;

        // Mocks
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockPlanRoomGroupService.Setup(x => x.FindByRoomGroupIdAsync(roomGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlanRoomGroup { PlanId = planId });
        mockMediator.Setup(x => x.Send(It.IsAny<PlanUpdateSpecialCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planId);

        var command = new RoomGroupUpdateSpecialCommand(roomGroupId)
        {
            Payload = new Application.Models.Requests.RoomGroupUpdateSpecialRequest(true, "Word")
        };

        var handler = new RoomGroupUpdateSpecialCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMediator.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(planId, result);
    }
}
