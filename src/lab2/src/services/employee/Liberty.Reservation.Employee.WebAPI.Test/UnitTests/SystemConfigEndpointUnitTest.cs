using Liberty.Cache.Services;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.SystemConfig;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.SystemConfig;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class SystemConfigEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(mediator => mediator.Send(It.IsAny<SystemConfigGetQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    SystemConfigGetQuery _,
                    CancellationToken _
                ) =>
                {
                    var systemConfigResponse = new SystemConfigResponse(
                        1,
                        "Test",
                        true
                    );

                    return (new HeaderDictionary(), systemConfigResponse);
                }
            );

        mockMediator.Setup(mediator => mediator.Send(It.IsAny<SystemConfigCanOnlinePaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SystemConfigResponse(
                    1,
                    "Test",
                    true
                )
            );
        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task GetSystemConfig_ReturnsOkResultWithHeaders()
    {
        var mockCacheService = new Mock<ICacheService>();
        var controller = new SystemConfigEndpoint(MockMapper, MockMediator, mockCacheService.Object);
        var result = await controller.GetSystemConfig(CancellationToken.None);
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        await result.ExecuteResultAsync(ActionContext);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateSystemConfig_ReturnsOkResult()
    {
        var mockCacheService = new Mock<ICacheService>();
        var request = new SystemConfigCanOnlinePaymentRequest(true);
        var controller = new SystemConfigEndpoint(MockMapper, MockMediator, mockCacheService.Object);
        var result = await controller.ChangeCanOnlinePayment(request, CancellationToken.None);
        var actionResult = Assert.IsType<ActionResult>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResult, false);
        await result.ExecuteResultAsync(ActionContext);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
