using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Moq;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupPriceCreateSiteCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var roomTypeId = 3;
        var planId = 30;
        var facilityId = 100;

        // Mocks
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();
        var mockPlanService = new Mock<IPlanService>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockPlanRoomGroupService.Setup(x => x.GetPlanWithRoomOnlyTypeAsync(roomTypeId, facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Plan { Id = planId });
        mockMediator.Setup(x => x.Send(It.IsAny<PlanPriceCreateSiteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planId);

        var command = new RoomGroupPriceCreateSiteCommand { Payload = new RoomGroupSiteCreateRequest(planId, roomTypeId, 1) };

        var handler = new RoomGroupPriceCreateSiteCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupService.Object,
            mockPlanService.Object,
            mockRoomGroupService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(planId, result);
    }
}
