using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class CancellationPolicyEnableCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldEnableCancellationPolicy_WhenValidRequest()
    {
        // Arrange
        var cancellationId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyEnableCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockPlanService = new Mock<IPlanService>();
        mockCancellationService.Setup(
                x => x.EnableAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Cancellation());
        mockPlanService.Setup(x => x.IsAnyPlanEnabledUsingCancellationAsync(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CancellationPolicyEnabledRequest(true);
        var command = new CancellationPolicyEnableCommand(1) { Payload = request };
        var handler = new CancellationPolicyEnableCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCancellationService.Object,
            mockPlanService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(cancellationId, result);
    }
}
