using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class AlertMessageEndpointUnitTest : BaseUnitTest
{
    private IAlertMessageService MockAlertMessageService { get; set; } = null!;

    protected override void InitData()
    {
        var mockAlertMessageService = new Mock<IAlertMessageService>();

        mockAlertMessageService
            .Setup(
                s =>
                    s.GetFirstAlertMessageAsync(
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                new AlertMessage
                {
                    Id = 1,
                    Title = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test title" } },
                    Content = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test content" } },
                    Color = "#FF0000",
                    Icon = "test-icon",
                    IsEnabled = true
                }
            );

        mockAlertMessageService
            .Setup(
                s =>
                    s.ExistingAlertMessageAsync(
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(true);

        MockAlertMessageService = mockAlertMessageService.Object;
    }

    [Fact]
    public async Task GetAlertMessage_ShouldReturnAlertMessage()
    {
        var alertMessage = new AlertMessage
        {
            Id = 1,
            Title = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test title" } },
            Content = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test content" } },
            Color = "#FF0000",
            Icon = "test-icon",
            IsEnabled = true
        };

        var alertMessageResponse = new AlertMessageResponse(
            1,
            "Test title",
            "Test content",
            "#FF0000",
            "test-icon",
            true
        );

        var mockMapper = MockServices.MockMapper((alertMessage, alertMessageResponse));

        var controller = new AlertMessageEndpoint(
            mockMapper,
            MockMediator,
            MockAlertMessageService
        );

        var result = await controller.GetAlertMessage(CancellationToken.None);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task UpdateAlertMessage_ShouldReturnNoContent()
    {
        var request = new AlertMessageUpdateRequest(
            "Updated title",
            "Updated content",
            "#00FF00",
            "updated-icon",
            false
        ) { Id = 1 };

        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(
                s =>
                    s.Send(It.IsAny<AlertMessageUpdateCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);

        var controller = new AlertMessageEndpoint(
            MockMapper,
            mockMediator.Object,
            MockAlertMessageService
        );
        var result = await controller.UpdateAlertMessage(request, 1, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AlertMessage.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(request.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }
}
