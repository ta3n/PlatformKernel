using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Moq;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupPriceUpdateDiscountCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var roomTypeId = 5;
        var planId = 50;
        var facilityId = 300;
        var siteId = 3;

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
        mockMediator.Setup(x => x.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planId);

        var command = new RoomGroupPriceUpdateDiscountCommand(
            roomTypeId,
            siteId
        ) { Payload = new Application.Models.Requests.RoomGroupPriceUpdateDiscountRequest([]) };

        var handler = new RoomGroupPriceUpdateDiscountCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupService.Object,
            mockRoomGroupService.Object,
            mockPlanService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }
}
