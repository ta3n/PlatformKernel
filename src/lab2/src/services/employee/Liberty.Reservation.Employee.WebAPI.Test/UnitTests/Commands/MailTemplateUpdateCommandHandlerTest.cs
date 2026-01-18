using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;
using AutoMapper;
using Liberty.Pagination;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class MailTemplateUpdateCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidRequest_UpdatesMailTemplateAndReturnsIoType()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSystemConfigService = new Mock<ISystemConfigService>();
        var mockMailTemplate = new SystemConfig { TemplateFormatData = new TemplateFormatData() };
        var page = new Page<SystemConfig>([mockMailTemplate], PageableBinderConfig.DefaultPageable, 1);
        mockSystemConfigService.Setup(s => s.FindAllAsync(PageableConstants.UnPaged, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        mockSystemConfigService
            .Setup(
                s => s.UpdateAsync(
                    It.IsAny<SystemConfig>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<SystemConfig, SystemConfig, SystemConfig>?>(),
                    It.IsAny<CancellationToken>()
                )
            );

        var commandHandler = new MailTemplateUpdateCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSystemConfigService.Object
        );

        var command = new MailTemplateUpdateCommand
        {
            Payload = new MailTemplateUpdateRequest("{\"Subject\":\"Test\",\"Body\":\"Test\"}") { IoType = IoType.IO10001 }
        };

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await commandHandler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(IoType.IO10001, result);
    }
}
