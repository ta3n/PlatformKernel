using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Moq;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupUpdateSaleCommandHandlerTest
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

        mockPlanRoomGroupService.Setup(x => x.FindByRoomGroupIdAsync(roomGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlanRoomGroup { PlanId = planId });
        mockMediator.Setup(x => x.Send(It.IsAny<PlanUpdateSaleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planId);

        var command = new RoomGroupUpdateSaleCommand(roomGroupId)
        {
            Payload = new RoomGroupUpdateSaleRequest(
                new TimeSpan(1, 1, 1),
                new TimeSpan(1, 1, 1),
                new TimeSpan(1, 1, 1),
                0,
                1,
                1,
                Reservation.Application.Constants.PlanDaySaleLimitTypes.RoomGroup,
                true,
                1,
                2,
                true,
                1,
                1,
                1,
                1
            )
        };

        var handler = new RoomGroupUpdateSaleCommandHandler(
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
