using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanPriceUpdateChildrenPriceCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanRoomGroupSitePersonAgeTypeService = new Mock<IPlanRoomGroupSitePersonAgeTypeService>();
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();

        var planRoomGroupSites = new List<PlanRoomGroupSitePersonAgeType>
        {
            new()
            {
                PlanId = planId,
                RoomGroupId = 1,
                SiteId = 1
            }
        };
        mockPlanRoomGroupSitePersonAgeTypeService.Setup(
                x => x.FindAllByPlanIdAndRomTypeIdAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(planRoomGroupSites);

        var request = new RoomGroupPriceUpdateChildrenPriceRequest([]);

        var command = new PlanPriceUpdateChildrenPriceCommand(1, 1, 1) { Payload = request };
        var mockPlanService = new Mock<IPlanService>();
        mockPlanService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new PlanPriceUpdateChildrenPriceCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockPlanRoomGroupSitePersonAgeTypeService.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockPlanService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
