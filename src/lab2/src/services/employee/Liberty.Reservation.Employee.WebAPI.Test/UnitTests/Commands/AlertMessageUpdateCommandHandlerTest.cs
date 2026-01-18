using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class AlertMessageUpdateCommandHandlerTest
{
    [Fact]
    public async Task HandlerAsync_ValidRequest_UpdateAlertMessage()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockAlertMessageService = new Mock<IAlertMessageService>();
        var mockCacheService = new Mock<ICacheService>();

        var payload = new AlertMessageUpdateRequest(
            "title test",
            "content test",
            null,
            null,
            false
        ) { Id = 1 };

        var fakeAlertMessage = new AlertMessage { Id = 1 };

        var command = new AlertMessageUpdateCommand { Payload = payload };

        var cancellationToken = CancellationToken.None;

        mockMapper.Setup(s => s.Map<AlertMessage>(It.IsAny<AlertMessageUpdateRequest>()))
            .Returns(fakeAlertMessage);

        mockAlertMessageService
            .Setup(s => s.ExistingAlertMessageAsync(It.IsAny<long>(), cancellationToken))
            .ReturnsAsync(true);

        mockAlertMessageService
            .Setup(
                s => s.UpdateAsync(
                    It.IsAny<AlertMessage>(),
                    true,
                    It.IsAny<Func<AlertMessage, AlertMessage, AlertMessage>>(),
                    cancellationToken
                )
            )
            .ReturnsAsync(fakeAlertMessage);

        var commandHandler = new AlertMessageUpdateCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockAlertMessageService.Object
        );

        var result = await commandHandler.Handle(command, cancellationToken);

        Assert.Equal(payload.Id, result);
    }

    [Fact]
    public async Task HandlerAsync_ThrowNotFoundException_UpdateAlertMessage()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockAlertMessageService = new Mock<IAlertMessageService>();
        var cancellationToken = CancellationToken.None;
        var mockCacheService = new Mock<ICacheService>();

        var payload = new AlertMessageUpdateRequest(
            "title test",
            "content test",
            null,
            null,
            false
        ) { Id = -1 };

        var command = new AlertMessageUpdateCommand { Payload = payload };

        var commandHandler = new AlertMessageUpdateCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockAlertMessageService.Object
        );

        await Assert.ThrowsAsync<AlertMessageNotFoundException>(
            () => commandHandler.Handle(command, cancellationToken)
        );
    }
}
