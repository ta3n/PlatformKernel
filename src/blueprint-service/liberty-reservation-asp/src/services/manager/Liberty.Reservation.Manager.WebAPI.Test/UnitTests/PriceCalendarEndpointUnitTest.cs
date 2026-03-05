using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceCalendar;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceCalendar;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PriceCalendarEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceCalendarCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceCalendarGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PriceCalendarGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new List<PriceCalendarResponse>
                    {
                        new(1, 20250101),
                        new(1, 20250102)
                    };

                    return (new HeaderDictionary(), response);
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task AdjustPriceOfCalendar_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new PriceCalendarEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new PriceCalendarCreateRequest(
            [
                new FacilityCalendarCreateRequest(1, 20250101, false)
            ]
        );

        // Act
        var result = await controller.AdjustPriceOfCalendar(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllPricesOfCalendar_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new PriceCalendarEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllPricesOfCalendar(mockPageable.Object, 20250101, 20250202, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<IEnumerable<PriceCalendarResponse>>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.NotNull(response);
        Assert.Equal(2, response.Count());
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
