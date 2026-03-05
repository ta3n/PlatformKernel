using Moq;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.UnitOfWork.Abstractions;
using AutoMapper;
using Liberty.Reservation.Manager.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class OptionItemInventoryAdjustCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAdjustedIds_WhenInventoryIsAdjustedSuccessfully()
    {
        // Arrange
        var optionItemId = 123L;
        var appDateId = 20250101;
        var payload = new List<OptionItemChangeRemainRequest> { new(appDateId, optionItemId, 10, false) };

        var command = new OptionItemInventoryAdjustCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<OptionItemInventoryAdjustCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockAppDateService = new Mock<IAppDateService>();
        var mockOptionItemAppDateService = new Mock<IOptionItemAppDateService>();

        mockOptionItemService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockAppDateService.Setup(s => s.FindAllByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new List<AppDate>
                {
                    new()
                    {
                        Id = appDateId,
                        DateTime = DateTime.Now
                    }
                }
            ); // Mock existing app dates

        mockOptionItemAppDateService.Setup(
                s => s.FindAllByOptionItemIdsAsync(It.IsAny<long[]>(), It.IsAny<long[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<OptionItemAppDate>());

        mockOptionItemAppDateService.Setup(
                s => s.CreateRangeAsync(It.IsAny<List<OptionItemAppDate>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<OptionItemAppDate> { new() { AppDateId = appDateId } });

        mockOptionItemAppDateService.Setup(
                s => s.UpdateRangeAsync(It.IsAny<List<OptionItemAppDate>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<OptionItemAppDate> { new() { AppDateId = appDateId } });

        var handler = new OptionItemInventoryAdjustCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockOptionItemService.Object,
            mockAppDateService.Object,
            mockOptionItemAppDateService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Item1);
        Assert.Contains(appDateId, result.Item1);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowOptionItemInventoryMaximumPayloadException_WhenPayloadExceedsLimit()
    {
        // Arrange
        var optionItemId = 123L;
        var appDateId = 20250101;
        var payload = new List<OptionItemChangeRemainRequest>(101); // Exceed the max payload limit
        for (var i = 0; i < 101; i++)
        {
            payload.Add(
                new OptionItemChangeRemainRequest(appDateId, optionItemId, 10, false)
            );
        }

        var command = new OptionItemInventoryAdjustCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<OptionItemInventoryAdjustCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockAppDateService = new Mock<IAppDateService>();
        var mockOptionItemAppDateService = new Mock<IOptionItemAppDateService>();

        var handler = new OptionItemInventoryAdjustCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockOptionItemService.Object,
            mockAppDateService.Object,
            mockOptionItemAppDateService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<OptionItemInventoryMaximumPayloadException>(
            () =>
                handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowOptionItemNotfoundException_WhenOptionItemDoesNotExist()
    {
        // Arrange
        var optionItemId = 123L;
        var appDateId = 20250101;
        var payload = new List<OptionItemChangeRemainRequest> { new(appDateId, optionItemId, 10, false) };

        var command = new OptionItemInventoryAdjustCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<OptionItemInventoryAdjustCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockAppDateService = new Mock<IAppDateService>();
        var mockOptionItemAppDateService = new Mock<IOptionItemAppDateService>();

        mockOptionItemService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var handler = new OptionItemInventoryAdjustCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockOptionItemService.Object,
            mockAppDateService.Object,
            mockOptionItemAppDateService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<OptionItemNotfoundException>(
            () =>
                handler.Handle(command, CancellationToken.None)
        );
    }
}
