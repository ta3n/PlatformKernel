using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class CancellationDeleteCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnCancellationId_WhenCommandValid()
    {
        // Arrange
        var facilityKey = 1;
        var cancellationId = 1;

        // Mocks
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockLogger = new Mock<ILogger<CancellationPolicyDeleteCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationService = new Mock<ICancellationService>();
        var mockDataOfCancellationService = new Mock<IDataOfCancellationService>();
        var mockFacilityCancellationService = new Mock<IFacilityCancellationService>();
        var mockCancellationDataService = new Mock<ICancellationDataService>();
        var mockPlanService = new Mock<IPlanService>();
        var mockCacheService = new Mock<ICacheService>();

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

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        var config = new MapperConfiguration(
            _ =>
            {
            }
        );

        var cancellation = new Cancellation
        {
            Id = cancellationId,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } }
        };
        mockCancellationService.Setup(x => x.CreateAsync(It.IsAny<Cancellation>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cancellation);

        mockCancellationService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockCancellationService.Setup(s => s.DeleteAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cancellation);

        mockDataOfCancellationService.Setup(x => x.FindAllByCancellationIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        mockPlanService.Setup(x => x.IsAnyPlanEnabledUsingCancellationAsync(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CancellationPolicyDeleteRequest(1);
        var command = new CancellationPolicyDeleteCommand { Payload = request };
        var handler = new CancellationPolicyDeleteCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockCacheService.Object,
            mockServiceProvider.Object,
            mockSecurityContextAccessor.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(cancellationId, result);
    }
}
