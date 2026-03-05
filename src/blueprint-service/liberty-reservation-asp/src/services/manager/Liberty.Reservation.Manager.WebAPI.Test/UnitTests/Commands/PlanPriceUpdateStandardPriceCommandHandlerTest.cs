using AutoMapper;
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

public class PlanPriceUpdateStandardPriceCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        long planId = 1, roomGroupId = 1, site = 1;

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanRoomGroupSiteAppdateTypePriceService = new Mock<IPlanRoomGroupSiteAppDateTypePriceService>();
        var mockAppdateTypeService = new Mock<IAppDateTypeService>();
        var mockLogger = new Mock<ILogger<PlanPriceUpdateStandardPriceCommandHandler>>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mapper = new Mock<IMapper>();

        var mockPlanRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = roomGroupId,
            SiteId = site,
            IsEnabledMinimumPrice = false,
            MinimumPrice = 100
        };

        mockAppdateTypeService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new RoomGroupPriceUpdateStandardPriceRequest([new(20250101, 1, 10, 100)]);

        var command = new PlanPriceUpdateStandardPriceCommand(1, 1, 1) { Payload = request };
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanRoomGroupSiteService = new Mock<IPlanRoomGroupSiteService>();
        var facilityMinimumPriceData = new FacilityMinimumPriceData(false, 100);
        mockPlanService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockPlanRoomGroupSiteService
            .Setup(
                f => f.FindByPlanIdAndRomTypeIdAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(mockPlanRoomGroupSite);

        mockFacilityService.Setup(r => r.GetMinimumPriceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(facilityMinimumPriceData);
        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(1);

        var handler = new PlanPriceUpdateStandardPriceCommandHandler(
            mockUnitOfWork.Object,
            mapper.Object,
            mockPlanRoomGroupSiteAppdateTypePriceService.Object,
            mockAppdateTypeService.Object,
            mockPlanService.Object,
            mockFacilityService.Object,
            mockPlanRoomGroupSiteService.Object,
            mockSecurityContextAccessor.Object,
            mockLogger.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
