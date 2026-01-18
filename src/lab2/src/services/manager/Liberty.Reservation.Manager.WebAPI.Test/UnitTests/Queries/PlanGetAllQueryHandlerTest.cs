using AutoMapper;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanGetAllQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var mockPlanRepository = new Mock<IPlanRepository>();
        var mockAccessor = new Mock<ISecurityContextAccessor>();
        var mockPage = new Mock<IPageable>();

        mockAccessor.Setup(s => s.FacilityKey).Returns(1);

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Plan, PlanResponse>();
            }
        );

        var plans = new List<Plan>();

        mockPlanRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plans.AsQueryable().BuildMock());

        var command = new PlanGetAllQuery(mockPage.Object);
        var handler = new PlanGetAllQueryHandler(
            config.CreateMapper(),
            mockAccessor.Object,
            mockPlanRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
