using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class BathingTaxAgeVisibleCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityId_WhenEntityFoundAndUpdated()
    {
        // Arrange
        var facilityKey = 123L;
        var dummyPersonAgeType = new PersonAgeType
        {
            Id = 50,
            IsVisible = false
        };

        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPersonAgeTypeService = new Mock<IPersonAgeTypeService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockPersonAgeTypeService.Setup(
                s => s.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyPersonAgeType);

        mockPersonAgeTypeService.Setup(
                s => s.UpdateAsync(
                    It.IsAny<PersonAgeType>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<PersonAgeType, PersonAgeType, PersonAgeType>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new PersonAgeType());

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var payload = new BathingTaxAgeVisibleRequest(50, true);
        var command = new BathingTaxAgeVisibleCommand { Payload = payload };

        var handler = new BathingTaxAgeVisibleCommandHandler(
            MockUnitOfWork,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockPersonAgeTypeService.Object,
            mockFacilityService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(50, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityKey = 123L;
        var dummyPersonAgeType = new PersonAgeType
        {
            Id = 50,
            IsVisible = false
        };

        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPersonAgeTypeService = new Mock<IPersonAgeTypeService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        mockPersonAgeTypeService.Setup(
                s => s.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyPersonAgeType);

        mockPersonAgeTypeService.Setup(
                s => s.UpdateAsync(
                    It.IsAny<PersonAgeType>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<PersonAgeType, PersonAgeType, PersonAgeType>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new PersonAgeType());

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var payload = new BathingTaxAgeVisibleRequest(50, true);
        var command = new BathingTaxAgeVisibleCommand { Payload = payload };

        var handler = new BathingTaxAgeVisibleCommandHandler(
            MockUnitOfWork,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockPersonAgeTypeService.Object,
            mockFacilityService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
