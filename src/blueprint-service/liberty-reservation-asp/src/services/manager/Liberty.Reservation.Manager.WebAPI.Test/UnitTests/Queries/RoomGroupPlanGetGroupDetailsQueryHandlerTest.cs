using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using MediatR;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupPlanGetGroupDetailsQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnGroupDetails_WhenQueryValid()
    {
        var groupId = 1;
        var planId = 100;
        var expectedResponse = new object();

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockPlanRoomGroupService = new Mock<IPlanRoomGroupService>();

        var planRoomGroup = new PlanRoomGroup { PlanId = planId };
        mockPlanRoomGroupService.Setup(x => x.FindByRoomGroupIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(planRoomGroup);
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
        mockMediator.Setup(m => m.Send(It.IsAny<PlanGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), expectedResponse));

        var query = new RoomGroupPlanGetGroupDetailsQuery(groupId, Application.Models.GroupOfPlan.Question);
        var handler = new RoomGroupPlanGetGroupDetailsQueryHandler(
            mockMapper.Object,
            mockMediator.Object,
            mockPlanRoomGroupService.Object
        );

        // Act
        var (_, result) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }
}
