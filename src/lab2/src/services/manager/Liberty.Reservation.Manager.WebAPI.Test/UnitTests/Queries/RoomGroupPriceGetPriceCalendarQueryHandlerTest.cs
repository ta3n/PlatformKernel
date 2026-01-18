using AutoMapper;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.AspNetCore.Http;
using Moq;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using MediatR;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupPriceGetPriceCalendarQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPriceCalendar_WhenQueryValid()
    {
        var facilityId = 1;
        var roomTypeId = 1;
        var siteId = 1;
        var startDate = AppDate.GetId(DateTime.UtcNow);
        var endDate = AppDate.GetId(DateTime.UtcNow.AddDays(10));
        var expectedResponse = new PlanRoomDetailPriceCalendarResponse(1, 1, 1, []);

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMediator.Setup(m => m.Send(It.IsAny<PlanPriceGetPriceCalendarQuery>(), It.IsAny<CancellationToken>()))
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

        var query = new RoomGroupPriceGetPriceCalendarQuery(roomTypeId, siteId, startDate, endDate);
        var handler = new RoomGroupPriceGetPriceCalendarQueryHandler(
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
        var startDate = AppDate.GetId(DateTime.UtcNow);
        var endDate = AppDate.GetId(DateTime.UtcNow.AddDays(10));
        var expectedResponse = new PlanRoomDetailPriceCalendarResponse(1, 1, 1, []);

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMediator.Setup(m => m.Send(It.IsAny<PlanPriceGetPriceCalendarQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), expectedResponse));

        var query = new RoomGroupPriceGetPriceCalendarQuery(roomTypeId, siteId, startDate, endDate);
        var handler = new RoomGroupPriceGetPriceCalendarQueryHandler(
            mockMapper.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        await Assert.ThrowsAsync<RoomOnlyTypeOfPlanNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
