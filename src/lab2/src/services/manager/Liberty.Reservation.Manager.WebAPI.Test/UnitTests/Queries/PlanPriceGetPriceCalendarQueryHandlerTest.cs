using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetPriceCalendarQueryHandlerTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(mediator => mediator.Send(It.IsAny<PlanPriceGetPriceCalendarQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    PlanPriceGetPriceCalendarQuery _,
                    CancellationToken _
                ) =>
                {
                    var planRoomDetailPriceCalendarResponse = new PlanRoomDetailPriceCalendarResponse(
                        1,
                        1,
                        1,
                        []
                    );

                    return (new HeaderDictionary(), planRoomDetailPriceCalendarResponse);
                }
            );
        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var controller = new PlanPricesEndpoint(MockMapper, MockMediator);
        var result = await controller.GetRoomTypePriceCalendar(
            1,
            1,
            1,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow.AddMonths(4)),
            CancellationToken.None
        );
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        await result.ExecuteResultAsync(ActionContext);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
