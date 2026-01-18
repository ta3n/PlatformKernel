using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class CancellationPolicyUpdateCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateCancellationPolicy_WhenValidRequest()
    {
        // Arrange
        var cancellationId = 1;
        var facilityId = 100;
        var payload = new CancellationPolicyUpdateRequest("nem", "Des", "Rule Detail", []);
        var command = new CancellationPolicyUpdateCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockCancellationDataService = new Mock<ICancellationDataService>();
        var mockCancellationTableHtmlService = new Mock<ICancellationTableHtmlService>();
        var mockDataOfCancellationService = new Mock<IDataOfCancellationService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockSecurityContextAccessor
            .Setup(x => x.FacilityKey)
            .Returns(facilityId);

        mockFacilityService
            .Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockCancellationService
            .Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMapper
            .Setup(m => m.Map<Cancellation>(payload))
            .Returns(new Cancellation { Id = cancellationId });

        mockCancellationService
            .Setup(
                x => x.UpdateAsync(
                    It.IsAny<Cancellation>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Cancellation, Cancellation, Cancellation>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Cancellation { Id = cancellationId });

        mockDataOfCancellationService
            .Setup(
                x => x.GetAllCancellationDataAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        mockCacheService
            .Setup(x => x.ResetAsync(It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IFacilityService)))
            .Returns(mockFacilityService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationService)))
            .Returns(mockCancellationService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationDataService)))
            .Returns(mockCancellationDataService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IDataOfCancellationService)))
            .Returns(mockDataOfCancellationService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationTableHtmlService)))
            .Returns(mockCancellationTableHtmlService.Object);

        var handler = new CancellationPolicyUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockCacheService.Object,
            mockServiceProvider.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(cancellationId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotFoundException_WhenFacilityDoesNotExist()
    {
        var cancellationId = 1;
        var facilityId = 100;
        var payload = new CancellationPolicyUpdateRequest("nem", "Des", "Rule Detail", []);
        var command = new CancellationPolicyUpdateCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockCancellationDataService = new Mock<ICancellationDataService>();
        var mockDataOfCancellationService = new Mock<IDataOfCancellationService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockCancellationTableHtmlService = new Mock<ICancellationTableHtmlService>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockSecurityContextAccessor
            .Setup(x => x.FacilityKey)
            .Returns(facilityId);

        mockFacilityService
            .Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        mockCancellationService
            .Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMapper
            .Setup(m => m.Map<Cancellation>(payload))
            .Returns(new Cancellation { Id = cancellationId });

        mockCancellationService
            .Setup(
                x => x.UpdateAsync(
                    It.IsAny<Cancellation>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Cancellation, Cancellation, Cancellation>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Cancellation { Id = cancellationId });

        mockCacheService
            .Setup(x => x.ResetAsync(It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IFacilityService)))
            .Returns(mockFacilityService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationService)))
            .Returns(mockCancellationService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationDataService)))
            .Returns(mockCancellationDataService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IDataOfCancellationService)))
            .Returns(mockDataOfCancellationService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationTableHtmlService)))
            .Returns(mockCancellationTableHtmlService.Object);

        var handler = new CancellationPolicyUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockCacheService.Object,
            mockServiceProvider.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCancellationNotFoundException_WhenCancellationDoesNotExist()
    {
        var cancellationId = 1;
        var facilityId = 100;
        var payload = new CancellationPolicyUpdateRequest("nem", "Des", "Rule Detail", []) { Id = 1 };
        var command = new CancellationPolicyUpdateCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockCancellationDataService = new Mock<ICancellationDataService>();
        var mockDataOfCancellationService = new Mock<IDataOfCancellationService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockCancellationTableHtmlService = new Mock<ICancellationTableHtmlService>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockSecurityContextAccessor
            .Setup(x => x.FacilityKey)
            .Returns(facilityId);

        mockFacilityService
            .Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockCancellationService
            .Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        mockMapper
            .Setup(m => m.Map<Cancellation>(payload))
            .Returns(new Cancellation { Id = cancellationId });

        mockCancellationService
            .Setup(
                x => x.UpdateAsync(
                    It.IsAny<Cancellation>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Cancellation, Cancellation, Cancellation>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Cancellation { Id = cancellationId });

        mockCacheService
            .Setup(x => x.ResetAsync(It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IFacilityService)))
            .Returns(mockFacilityService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationService)))
            .Returns(mockCancellationService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationDataService)))
            .Returns(mockCancellationDataService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IDataOfCancellationService)))
            .Returns(mockDataOfCancellationService.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICancellationTableHtmlService)))
            .Returns(mockCancellationTableHtmlService.Object);

        var handler = new CancellationPolicyUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockCacheService.Object,
            mockServiceProvider.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<CancellationNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
