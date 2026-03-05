using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class CancellationPolicyCreateCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldCreateCancellationPolicy_WhenValidRequest()
    {
        var facilityId = 100;
        var cancellationId = 1;
        var payload = new CancellationPolicyCreateRequest("Name", "Decription");
        var command = new CancellationPolicyCreateCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyCreateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityCancellationService = new Mock<IFacilityCancellationService>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockFacilityService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mockMapper.Setup(m => m.Map<Cancellation>(payload))
            .Returns(new Cancellation { Id = cancellationId });
        mockCancellationService.Setup(m => m.CreateAsync(It.IsAny<Cancellation>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cancellation { Id = cancellationId });
        mockFacilityCancellationService.Setup(m => m.CreateAsync(It.IsAny<FacilityCancellation>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FacilityCancellation());

        var handler = new CancellationPolicyCreateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationService.Object,
            mockFacilityService.Object,
            mockFacilityCancellationService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(cancellationId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityNotfoundException()
    {
        var facilityId = 100;
        var cancellationId = 1;
        var payload = new CancellationPolicyCreateRequest("Name", "Decription");
        var command = new CancellationPolicyCreateCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyCreateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockFacilityCancellationService = new Mock<IFacilityCancellationService>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockFacilityService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mockMapper.Setup(m => m.Map<Cancellation>(payload))
            .Returns(new Cancellation { Id = cancellationId });
        mockCancellationService.Setup(m => m.CreateAsync(It.IsAny<Cancellation>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cancellation { Id = cancellationId });
        mockFacilityCancellationService.Setup(m => m.CreateAsync(It.IsAny<FacilityCancellation>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FacilityCancellation());

        var handler = new CancellationPolicyCreateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationService.Object,
            mockFacilityService.Object,
            mockFacilityCancellationService.Object
        );

        // Act
        await Assert.ThrowsAsync<FacilityNotfoundException>(
            async () => await handler.Handle(command, CancellationToken.None)
        );
    }
}
