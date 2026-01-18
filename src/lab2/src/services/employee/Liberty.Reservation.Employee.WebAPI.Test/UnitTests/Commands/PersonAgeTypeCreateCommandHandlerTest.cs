using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class PersonAgeTypeCreateCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldCreatePersonAgeTypeAndReturnsId()
    {
        // Arrange
        var expectedPersonAgeType = new PersonAgeType
        {
            Id = 123,
            Name = new MultilingualText { { LanguageHeaderUtil.DefaultLanguageCode, "Test" } },
            AgeMin = 0,
            AgeMax = 99,
            Meta = new PersonAgeTypeMeta
            {
                FoodBed = 0,
                PersonAgeGroup = PersonAgeGroups.Child,
                GroupName = "TestGroup"
            }
        };

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockService = new Mock<IPersonAgeTypeService>();
        var mockPersonAgeTypeSpaTaxDataService = new Mock<IPersonAgeTypeSpaTaxDataService>();
        var mocklogger = new Mock<Microsoft.Extensions.Logging.ILogger<PersonAgeTypeCreateCommandHandler>>();

        mockService
            .Setup(
                s => s.CreateAsync(
                    It.IsAny<PersonAgeType>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedPersonAgeType);

        mockPersonAgeTypeSpaTaxDataService
            .Setup(
                s => s.CreateRangeAsync(
                    It.IsAny<List<PersonAgeTypeSpaTaxData>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new List<PersonAgeTypeSpaTaxData>());

        mockMapper.Setup(s => s.Map<PersonAgeType>(It.IsAny<PersonAgeTypeCreateRequest>()))
            .Returns(expectedPersonAgeType);

        mockService
            .Setup(
                s => s.CheckExistingPersonAgeGroupAsync(
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var handler = new PersonAgeTypeCreateCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mocklogger.Object,
            mockService.Object,
            mockPersonAgeTypeSpaTaxDataService.Object
        );

        var payload = new PersonAgeTypeCreateRequest(
            "Test",
            0,
            99,
            true,
            new MetaOfBathingTaxAgeUpdateRequest("TestGroup", FoodBeds.Food, FoodBeds.Bed, 8),
            new List<SpaOfBathingTaxAgeUpdateRequest>()
        );

        var command = new PersonAgeTypeCreateCommand { Payload = payload };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPersonAgeType.Id, result);
    }
}
