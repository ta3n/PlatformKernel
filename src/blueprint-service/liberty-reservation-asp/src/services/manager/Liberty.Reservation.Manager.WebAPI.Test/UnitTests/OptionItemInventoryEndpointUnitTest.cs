using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class OptionItemInventoryEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemInventoryAdjustCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(([20250101], [20250202]));

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemInventoryGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    OptionItemInventoryGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new List<OptionItemAppDateResponse>
                    {
                        new(20250101, 1, "Option 1", 1, true)
                        {
                            RemainNumber = 1,
                            ReservedNumber = 1
                        },
                        new(20250101, 2, "Option 2", 1, true)
                        {
                            RemainNumber = 1,
                            ReservedNumber = 1
                        }
                    };

                    return (new HeaderDictionary(), response);
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task AdjustAppDatesOfOptionItems_ReturnCorrectResult()
    {
        var request = new List<OptionItemChangeRemainRequest>
        {
            new(20250101, 1, 1, false),
            new(20250202, 2, 1, false)
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new OptionItemInventoryEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var result = await controller.AdjustAppDatesOfOptionItems(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.OptionItemInventory.Updated", headers["X-Liberty-alert"]);
        Assert.NotNull(headers["X-Liberty-params"].ToString());
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAppDatesOfOptionItems_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new OptionItemInventoryEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var mockPageable = new Mock<IPageable>();

        var result = await controller.GetAllAppDatesOfOptionItems(mockPageable.Object, 20250101, 20250202, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<OptionItemAppDateResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(2, response.Count);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
