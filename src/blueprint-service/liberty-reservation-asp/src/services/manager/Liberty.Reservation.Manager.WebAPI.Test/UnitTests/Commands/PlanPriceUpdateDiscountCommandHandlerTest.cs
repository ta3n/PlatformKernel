using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanPriceUpdateDiscountCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanRoomGroupSiteDiscountService = new Mock<IPlanRoomGroupSiteDiscountDataService>();
        var mockMapper = new Mock<IMapper>();
        var mockLogger = new Mock<ILogger<PlanPriceUpdateDiscountCommandHandler>>();

        var request = new RoomGroupPriceUpdateDiscountRequest([]);

        var command = new PlanPriceUpdateDiscountCommand(1, 1, 1) { Payload = request };
        var mockPlanService = new Mock<IPlanService>();
        mockPlanService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new PlanPriceUpdateDiscountCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockPlanRoomGroupSiteDiscountService.Object,
            mockPlanService.Object,
            mockLogger.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
