using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupPriceGetChildrenPriceQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnChildrenPrice_WhenQueryValid()
    {
        var facilityId = 1;
        var roomTypeId = 1;
        var siteId = 1;
        var expectedResponse = new PlanRoomDetailChildrenPriceResponse(1, 1, 1, []);

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMediator.Setup(m => m.Send(It.IsAny<PlanPriceGetChildrenPriceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), expectedResponse));
        var planForRoomOnly = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan For Room Only" } },
            Code = "Code",
            Summary = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "summary" } }
        };
        mockPlanRoomGroupService.Setup(
                x => x.GetPlanWithRoomOnlyTypeAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(planForRoomOnly);
        var query = new RoomGroupPriceGetChildrenPriceQuery(roomTypeId, siteId);
        var handler = new RoomGroupPriceGetChildrenPriceQueryHandler(
            mockMapper.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        var (_, result) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRoomOnlyTypeOfPlanNotfoundException()
    {
        var facilityId = 1;
        var roomTypeId = 1;
        var siteId = 1;
        var expectedResponse = new PlanRoomDetailChildrenPriceResponse(1, 1, 1, []);

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMediator.Setup(m => m.Send(It.IsAny<PlanPriceGetChildrenPriceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), expectedResponse));

        var query = new RoomGroupPriceGetChildrenPriceQuery(roomTypeId, siteId);
        var handler = new RoomGroupPriceGetChildrenPriceQueryHandler(
            mockMapper.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        // Assert
        await Assert.ThrowsAsync<RoomOnlyTypeOfPlanNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
