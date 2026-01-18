using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupUpdateCancelCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        // Mocks
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();
        var mockMediator = new Mock<IMediator>();

        var planRoomGroup = new PlanRoomGroup
        {
            PlanId = 1,
            RoomGroupId = 1
        };

        mockPlanRoomGroupService.Setup(x => x.FindByRoomGroupIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planRoomGroup);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateCancelCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var request = new RoomGroupUpdateCancelRequest(true, 1, new TimeSpan(0, 1, 0), 1);
        var command = new RoomGroupUpdateCancelCommand(1) { Payload = request };

        var handler = new RoomGroupUpdateCancelCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMediator.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(planRoomGroup.PlanId, result);
    }
}
