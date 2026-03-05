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

public class BathingTaxAgeUpdateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityId_WhenUpdateSuccessful()
    {
        // Arrange
        var facilityKey = 123L;
        var payloadRequest = new BathingTaxAgeUpdateRequest(
            50,
            AgeMin: 18,
            AgeMax: 99,
            Name: "Updated Age Type",
            IsEnabled: true,
            Meta: new MetaOfBathingTaxAgeUpdateRequest("Adult Group", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Adult),
            Spas:
            [
                new(PriceMax: 1000, PriceMin: 0, Tax: 100),
                new(PriceMax: 2000, PriceMin: 1000, Tax: 200)
            ]
        );
        var command = new BathingTaxAgeUpdateCommand { Payload = payloadRequest };

        // Mocks
        var mockLogger = new Mock<ILogger<BathingTaxAgeUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityPersonAgeTypeService = new Mock<IFacilityPersonAgeTypeService>();
        var mockPersonAgeTypeSpaTaxDataService = new Mock<IPersonAgeTypeSpaTaxDataService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPlanService = new Mock<IPlanService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockFacilityService.Setup(
                s => s.CountByIdsAsync(
                    It.Is<long[]>(ids => ids.Contains(facilityKey)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var dummyFacilityPersonAgeType = new FacilityPersonAgeType
        {
            PersonAgeTypeId = 50,
            PersonAgeType = new PersonAgeType
            {
                Id = 50,
                AgeMin = 0,
                AgeMax = 0,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                IsEnabled = false,
                Meta = new PersonAgeTypeMeta
                {
                    FoodBed = 0,
                    PersonAgeGroup = 0,
                    GroupName = "Old Group"
                }
            }
        };

        mockFacilityPersonAgeTypeService.Setup(
                s => s.FindByFacilityIdAsync(
                    facilityKey,
                    payloadRequest.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyFacilityPersonAgeType);

        mockFacilityPersonAgeTypeService.Setup(
                s => s.UpdateAsync(
                    It.IsAny<FacilityPersonAgeType>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new FacilityPersonAgeType());

        mockPersonAgeTypeSpaTaxDataService.Setup(
                s => s.ChangeSpaTaxDataOfPersonAgeType(
                    payloadRequest.Id,
                    facilityKey,
                    It.IsAny<List<PersonAgeTypeSpaTaxData>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(([], [], []));

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var handler = new BathingTaxAgeUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockFacilityPersonAgeTypeService.Object,
            mockPersonAgeTypeSpaTaxDataService.Object,
            mockFacilityService.Object,
            mockPlanService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<long>(result);
    }
}
