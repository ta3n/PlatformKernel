using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class MailTemplatesEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<MailTemplateSendCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("email");

        mockMediator
            .Setup(m => m.Send(It.IsAny<MailTemplateGetQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (new HeaderDictionary(), new MailTemplateResponse(
                    "Test Subject",
                    "Test Body",
                    "https://example.com"
                ))
            );

        mockMediator
            .Setup(m => m.Send(It.IsAny<MailTemplatePreviewCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new MailTemplatePreviewResponse(
                    "Test Subject",
                    "Test Body"
                )
            );

        mockMediator
            .Setup(m => m.Send(It.IsAny<MailTemplateUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("UpdatedMailTemplate");

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task SendMail_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new MailTemplatesEndpoint(MockMapper, MockMediator);
        var request = new MailTemplateSendRequest("Test Subject", "Body", "email@test.com");

        // Act
        var result = await controller.SendMail(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.IsType<string>(okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetMailTemplate_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new MailTemplatesEndpoint(MockMapper, MockMediator);

        // Act
        var result = await controller.GetMailTemplate("Test", CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        _ = Assert.IsType<MailTemplateResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task PreviewMail_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new MailTemplatesEndpoint(MockMapper, MockMediator);
        var request = new MailTemplatePreviewRequest("Format");

        // Act
        var result = await controller.PreviewMail(request, "Test", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result, false);
        _ = Assert.IsType<MailTemplatePreviewResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateMailTemplate_ReturnsNoContentWithHeaders()
    {
        // Arrange
        var controller = new MailTemplatesEndpoint(MockMapper, MockMediator);
        var request = new MailTemplateUpdateRequest("FormatFake");
        const string type = "SomeType";

        // Act
        var result = await controller.UpdateMailTemplate(type, request, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}
