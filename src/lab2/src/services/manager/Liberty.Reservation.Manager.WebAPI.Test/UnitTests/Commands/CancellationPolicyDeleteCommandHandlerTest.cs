using AutoMapper;
using Liberty.Cache.Services;
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

public class CancellationPolicyDeleteCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldDeleteCancellationPolicy_WhenValidRequest()
    {
        var cancellationId = 1;
        var payload = new CancellationPolicyDeleteRequest(cancellationId);
        var command = new CancellationPolicyDeleteCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyDeleteCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityCancellationService = new Mock<IFacilityCancellationService>();
        var mockDataOfCancellationService = new Mock<IDataOfCancellationService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockPlanService = new Mock<IPlanService>();
        var mockCancellationDataService = new Mock<ICancellationDataService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(s => s.GetService(typeof(ICancellationService)))
            .Returns(mockCancellationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IFacilityCancellationService)))
            .Returns(mockFacilityCancellationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(ICancellationDataService)))
            .Returns(mockCancellationDataService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IDataOfCancellationService)))
            .Returns(mockDataOfCancellationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IPlanService)))
            .Returns(mockPlanService.Object);

        mockCancellationService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mockDataOfCancellationService.Setup(x => x.FindAllByCancellationIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        mockCancellationService.Setup(x => x.DeleteAsync(It.IsAny<long>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cancellation { Id = cancellationId });
        mockFacilityCancellationService.Setup(x => x.DeleteFacilityCancellation(It.IsAny<long>(), false))
            .ReturnsAsync(new List<FacilityCancellation>().AsEnumerable());
        mockCacheService.Setup(x => x.ResetAsync(It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockPlanService.Setup(x => x.IsAnyPlanEnabledUsingCancellationAsync(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CancellationPolicyDeleteCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockServiceProvider.Object,
            mockSecurityContextAccessor.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(cancellationId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCancellationNotfoundException()
    {
        var cancellationId = 1;
        var payload = new CancellationPolicyDeleteRequest(cancellationId);
        var command = new CancellationPolicyDeleteCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<CancellationPolicyDeleteCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockFacilityCancellationService = new Mock<IFacilityCancellationService>();
        var mockDataOfCancellationService = new Mock<IDataOfCancellationService>();
        var mockCancellationDataService = new Mock<ICancellationDataService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockPlanService = new Mock<IPlanService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(s => s.GetService(typeof(ICancellationService)))
            .Returns(mockCancellationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IFacilityCancellationService)))
            .Returns(mockFacilityCancellationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(ICancellationDataService)))
            .Returns(mockCancellationDataService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IDataOfCancellationService)))
            .Returns(mockDataOfCancellationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IPlanService)))
            .Returns(mockPlanService.Object);

        mockCancellationService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mockDataOfCancellationService.Setup(x => x.FindAllByCancellationIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        mockCancellationService.Setup(x => x.DeleteAsync(It.IsAny<long>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cancellation { Id = cancellationId });
        mockFacilityCancellationService.Setup(x => x.DeleteFacilityCancellation(It.IsAny<long>(), false))
            .ReturnsAsync(new List<FacilityCancellation>().AsEnumerable());
        mockCacheService.Setup(x => x.ResetAsync(It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockPlanService.Setup(x => x.IsAnyPlanEnabledUsingCancellationAsync(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        //mockPlanRepository.Setup(x =>
        //    x.GetQueryableWithAsNoTracking()
        //    .AnyAsync(t => t.CancellationId == It.IsAny<long>(),It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(false);

        var handler = new CancellationPolicyDeleteCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockServiceProvider.Object,
            mockSecurityContextAccessor.Object
        );

        // Act
        await Assert.ThrowsAsync<CancellationNotfoundException>(
            async () => await handler.Handle(command, CancellationToken.None)
        );
    }
}
