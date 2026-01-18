using AutoMapper;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.AspNetCore.Http;
using Moq;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;
using MediatR;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupPriceGetSaleQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnSalePrice_WhenQueryValid()
    {
        var facilityId = 100;
        var roomTypeId = 1;
        var siteId = 10;
        var expectedResponse = new PlanRoomDetailSaleResponse();

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMediator.Setup(m => m.Send(It.IsAny<PlanPriceGetSaleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), expectedResponse));

        var planForRoomOnly = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
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

        var query = new RoomGroupPriceGetSaleQuery(roomTypeId, siteId);
        var handler = new RoomGroupPriceGetSaleQueryHandler(
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
        var facilityId = 100;
        var roomTypeId = 1;
        var siteId = 10;
        var expectedResponse = new PlanRoomDetailSaleResponse();

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMediator.Setup(m => m.Send(It.IsAny<PlanPriceGetSaleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), expectedResponse));

        var query = new RoomGroupPriceGetSaleQuery(roomTypeId, siteId);
        var handler = new RoomGroupPriceGetSaleQueryHandler(
            mockMapper.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        await Assert.ThrowsAsync<RoomOnlyTypeOfPlanNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
