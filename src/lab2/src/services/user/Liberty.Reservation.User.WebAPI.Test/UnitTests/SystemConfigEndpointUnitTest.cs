using Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.SystemConfig;
using Liberty.Reservation.User.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests;

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
        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task GetSystemConfig_ReturnsOkResultWithHeaders()
    {
        var controller = new SystemConfigEndpoint(MockMapper, MockMediator);
        var result = await controller.GetSystemConfig(CancellationToken.None);
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        await result.ExecuteResultAsync(ActionContext);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
