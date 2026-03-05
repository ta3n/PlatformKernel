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

public class RoomGroupUpdatePaymentMethodCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var roomGroupId = 1;
        var planId = 10;

        // Mocks
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        var planRoomGroup = new PlanRoomGroup { PlanId = planId };
        mockPlanRoomGroupService.Setup(x => x.FindByRoomGroupIdAsync(roomGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(planRoomGroup);

        mockMediator.Setup(x => x.Send(It.IsAny<PlanUpdatePaymentMethodCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planId);

        var command = new RoomGroupUpdatePaymentMethodCommand(roomGroupId)
        {
            Payload = new RoomGroupUpdatePaymentMethodRequest(true, false)
        };
        var handler = new RoomGroupUpdatePaymentMethodCommandHandler(
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
