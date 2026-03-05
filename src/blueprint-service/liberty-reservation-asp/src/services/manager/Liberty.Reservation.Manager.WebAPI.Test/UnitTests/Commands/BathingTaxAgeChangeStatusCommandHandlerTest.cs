using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class BathingTaxAgeChangeStatusCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityId_WhenFacilityExists()
    {
        // Arrange
        var facilityKey = 123L;

        var mockLogger = new Mock<ILogger<BathingTaxAgeChangeStatusCommandHandler>>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPersonAgeTypeService = new Mock<IPersonAgeTypeService>();

        var dummyPersonAgeType = new PersonAgeType { Id = 100 };

        mockPersonAgeTypeService.Setup(
                s => s.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyPersonAgeType);

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var dummyFacility = new Facility { Id = 100 };
        mockFacilityService.Setup(
                s => s.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyFacility);

        var payload = new BathingTaxAgeChangeStatusRequest(50, true);
        var command = new BathingTaxAgeChangeStatusCommand { Payload = payload };

        var handler = new BathingTaxAgeChangeStatusCommandHandler(
            mockLogger.Object,
            MockUnitOfWork,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object,
            mockPersonAgeTypeService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(dummyPersonAgeType.Id, result);
    }

    [Fact]
    public async Task HandleAsync_WhenFacilityNotFound()
    {
        // Arrange
        var facilityKey = 123L;

        var mockLogger = new Mock<ILogger<BathingTaxAgeChangeStatusCommandHandler>>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPersonAgeTypeService = new Mock<IPersonAgeTypeService>();

        var dummyPersonAgeType = new PersonAgeType { Id = 100 };

        mockPersonAgeTypeService.Setup(
                s => s.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyPersonAgeType);

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var dummyFacility = new Facility { Id = 100 };
        mockFacilityService.Setup(
                s => s.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyFacility);

        var payload = new BathingTaxAgeChangeStatusRequest(50, true);
        var command = new BathingTaxAgeChangeStatusCommand { Payload = payload };

        var handler = new BathingTaxAgeChangeStatusCommandHandler(
            mockLogger.Object,
            MockUnitOfWork,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object,
            mockPersonAgeTypeService.Object
        );

        // Act
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
