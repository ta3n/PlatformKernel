using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanPriceUpdatePriceCalendarCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        long planId = 1, roomGroupId = 1, site = 1;

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDapperUnitOfWork = new Mock<IDapperUnitOfWork>();
        var mockPlanRoomGroupSiteAppDateService = new Mock<IPlanRoomGroupSiteAppDateService>();
        var mockPlanRoomGroupSiteAppDatePriceDataService = new Mock<IPlanRoomGroupSiteAppDatePriceDataService>();
        var mockMapper = new Mock<IMapper>();
        var mockLogger = new Mock<ILogger<PlanPriceUpdatePriceCalendarCommandHandler>>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockCacheService = new Mock<ICacheService>();

        var request = new RoomGroupPriceUpdatePriceCalendarRequest([]);

        var command = new PlanPriceUpdatePriceCalendarCommand(1, 1, 1) { Payload = request };
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanRoomGroupSiteService = new Mock<IPlanRoomGroupSiteService>();
        var expectedFacility = new FacilityMinimumPriceData(false, 100);

        var mockPlanRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = roomGroupId,
            SiteId = site,
            IsEnabledMinimumPrice = false,
            MinimumPrice = 100
        };
        mockPlanService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockPlanRoomGroupSiteService
            .Setup(
                f => f.FindByPlanIdAndRomTypeIdAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(mockPlanRoomGroupSite);

        mockFacilityService.Setup(r => r.GetMinimumPriceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFacility);
        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(1);

        mockServiceProvider.Setup(sp => sp.GetService(typeof(ISecurityContextAccessor)))
            .Returns(mockSecurityContextAccessor.Object);

        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICacheService)))
            .Returns(mockSecurityContextAccessor.Object);

        var handler = new PlanPriceUpdatePriceCalendarCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockDapperUnitOfWork.Object,
            mockMapper.Object,
            mockPlanRoomGroupSiteAppDateService.Object,
            mockPlanRoomGroupSiteAppDatePriceDataService.Object,
            mockPlanService.Object,
            mockFacilityService.Object,
            mockPlanRoomGroupSiteService.Object,
            mockSecurityContextAccessor.Object,
            mockCacheService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
