using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class BathingTaxAgeChangeSettingFacilityCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityId_WhenFacilityExists()
    {
        // Arrange
        var facilityKey = 123L;

        // Mocks
        var mockLogger = new Mock<ILogger<BathingTaxAgeChangeSettingFacilityCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var dummyFacility = new Facility
        {
            Id = facilityKey,
            Meta = new FacilityMeta { UseSpaTax = true },
            SpaTaxComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "New Comment" } },
            SpaTaxTable = new MultilingualText { { TestUtil.DefaultLanguageCode, "New Table" } }
        };
        mockFacilityService.Setup(
                s => s.UpdateSpaTaxChangeAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyFacility);

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var payload = new BathingTaxAgeChangeSettingFacilityRequest(
            SpaTaxComment: "New Comment",
            SpaTaxTable: "New Table",
            UseSpaTax: true
        );

        var command = new BathingTaxAgeChangeSettingFacilityCommand { Payload = payload };

        // Tạo instance handler
        var handler = new BathingTaxAgeChangeSettingFacilityCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(123, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityKey = 123L;
        var mockLogger = new Mock<ILogger<BathingTaxAgeChangeSettingFacilityCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        // InfrastructureOfTest SecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var payload = new BathingTaxAgeChangeSettingFacilityRequest(
            SpaTaxComment: "New Comment",
            SpaTaxTable: "New Table",
            UseSpaTax: true
        );
        var command = new BathingTaxAgeChangeSettingFacilityCommand { Payload = payload };

        var handler = new BathingTaxAgeChangeSettingFacilityCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
