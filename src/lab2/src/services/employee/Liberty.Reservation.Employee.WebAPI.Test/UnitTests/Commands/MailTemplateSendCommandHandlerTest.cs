using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;
using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.UnitOfWork.Abstractions;
using AutoMapper;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class MailTemplateSendCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidRequest_CallsSendAsyncAndReturnsEmail()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();

        // Mock the IMailService
        var mockMailService = new Mock<IMailService>();
        mockMailService.Setup(m => m.SendAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var commandHandler = new MailTemplateSendCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMailService.Object
        );

        var command = new MailTemplateSendCommand(
            new MailTemplateSendRequest(
                "Test Subject",
                "Test Body",
                "test@example.com"
            )
        );

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await commandHandler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal("test@example.com", result);
    }
}
