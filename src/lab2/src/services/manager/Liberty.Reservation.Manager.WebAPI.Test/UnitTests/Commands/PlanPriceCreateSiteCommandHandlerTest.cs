using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanPriceCreateSiteCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockLogger = new Mock<ILogger<PlanPriceCreateSiteCommandHandler>>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanRoomGroupSiteService = new Mock<IPlanRoomGroupSiteService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPlanRoomGroupSitePersonAgeTypeService = new Mock<IPlanRoomGroupSitePersonAgeTypeService>();
        var mockMapper = new Mock<IMapper>();

        var planRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = 1,
            SiteId = 1
        };
        mockPlanRoomGroupSiteService.Setup(
                x => x.FindByPlanIdAndRomTypeIdAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(planRoomGroupSite);

        var facility = new Facility
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Code = "Code"
        };
        mockFacilityService.Setup(x => x.FindByIdWithIncludePersonAgeTypeAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        mockPlanRoomGroupSiteService
            .Setup(x => x.CreateAsync(It.IsAny<PlanRoomGroupSite>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planRoomGroupSite);

        var request = new RoomGroupSiteCreateRequest(1, 1, 1);

        var command = new PlanPriceCreateSiteCommand { Payload = request };
        var handler = new PlanPriceCreateSiteCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupSiteService.Object,
            mockFacilityService.Object,
            mockPlanRoomGroupSitePersonAgeTypeService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
