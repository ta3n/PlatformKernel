using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class BathingTaxAgeCreateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPersonAgeTypeId_WhenCommandValid()
    {
        // Arrange
        var facilityKey = 123L;
        var expectedPersonAgeTypeId = 100L;

        // Mocks
        var mockLogger = new Mock<ILogger<BathingTaxAgeCreateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityPersonAgeTypeService = new Mock<IFacilityPersonAgeTypeService>();
        var mockPersonAgeTypeSpaTaxDataService = new Mock<IPersonAgeTypeSpaTaxDataService>();
        var mockFacilityService = new Mock<IFacilityService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        // InfrastructureOfTest IFacilityService: trả về count > 0
        mockFacilityService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var dummyFacilityPersonAgeType = new FacilityPersonAgeType
        {
            PersonAgeTypeId = expectedPersonAgeTypeId,
            FacilityId = facilityKey,
            PersonAgeType = new PersonAgeType
            {
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                Code = "DUMMY",
                AgeMax = 99,
                AgeMin = 18,
                DisplayOrder = 1,
                IsEnabled = true,
                Meta = new PersonAgeTypeMeta
                {
                    FoodBed = 0,
                    PersonAgeGroup = PersonAgeGroups.TeenB,
                    GroupName = "Adult"
                }
            }
        };
        mockFacilityPersonAgeTypeService
            .Setup(s => s.CreateAsync(It.IsAny<FacilityPersonAgeType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyFacilityPersonAgeType);

        mockPersonAgeTypeSpaTaxDataService
            .Setup(
                s => s.CreateRangeAsync(It.IsAny<IEnumerable<PersonAgeTypeSpaTaxData>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<PersonAgeTypeSpaTaxData>());

        var meta = new MetaOfBathingTaxAgeUpdateRequest(
            "Adult Group",
            FoodBeds.Food,
            FoodBeds.Bed,
            PersonAgeGroups.Adult
        );

        var spas = new List<SpaOfBathingTaxAgeUpdateRequest> { };

        var payload = new BathingTaxAgeCreateRequest(
            "Test Age Type",
            99,
            18,
            100,
            true,
            meta,
            spas
        );

        var command = new BathingTaxAgeCreateCommand { Payload = payload };

        // Tạo instance handler
        var handler = new BathingTaxAgeCreateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityPersonAgeTypeService.Object,
            mockPersonAgeTypeSpaTaxDataService.Object,
            mockFacilityService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(100, result);
    }
}
