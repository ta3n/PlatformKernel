using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class PersonAgeTypeUpdateCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_WhenPersonAgeTypeNotFound_ThrowsNotfoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockService = new Mock<IPersonAgeTypeService>();
        var mockPersonAgeTypeSpaTaxDataService = new Mock<IPersonAgeTypeSpaTaxDataService>();
        var mockLogger = new Mock<ILogger<PersonAgeTypeUpdateCommandHandler>>();

        mockService
            .Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var handler = new PersonAgeTypeUpdateCommandHandler(
            mockUnitOfWork.Object,
            mockLogger.Object,
            mockMapper.Object,
            mockService.Object,
            mockPersonAgeTypeSpaTaxDataService.Object
        );

        var payload = new PersonAgeTypeUpdateRequest(
            "Test",
            12,
            19,
            true,
            new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, 16),
            new List<SpaOfBathingTaxAgeUpdateRequest>()
        ) { Id = 999 };

        var command = new PersonAgeTypeUpdateCommand { Payload = payload };

        // Act & Assert
        await Assert.ThrowsAsync<PersonAgeTypeNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WhenPersonAgeTypeExists_UpdatesAndReturnsId()
    {
        // Arrange
        var existing = new PersonAgeType
        {
            Id = 123,
            Name = new MultilingualText { { LanguageHeaderUtil.DefaultLanguageCode, "Test" } },
            AgeMin = 0,
            AgeMax = 99,
            IsEnabled = true,
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
        var mockLogger = new Mock<ILogger<PersonAgeTypeUpdateCommandHandler>>();
        var mockPersonAgeTypeSpaTaxDataService = new Mock<IPersonAgeTypeSpaTaxDataService>();

        mockService
            .Setup(
                s => s.CountByIdsAsync(
                    It.IsAny<long[]>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMapper
            .Setup(
                s => s.Map<PersonAgeType>(
                    It.IsAny<PersonAgeTypeUpdateRequest>()
                )
            )
            .Returns(existing);

        mockService
            .Setup(
                s => s.UpdateAsync(
                    It.IsAny<PersonAgeType>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<PersonAgeType, PersonAgeType, PersonAgeType>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existing);

        mockService
            .Setup(
                s => s.GetPersonAgeTypeIsMainAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new PersonAgeType());

        mockService
            .Setup(
                s => s.CheckExistingPersonAgeGroupAsync(
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        mockPersonAgeTypeSpaTaxDataService.Setup(
                s => s.ChangeSpaTaxDataOfPersonAgeTypeAsync(
                    It.IsAny<long>(),
                    It.IsAny<List<PersonAgeTypeSpaTaxData>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(([], [], []));

        var handler = new PersonAgeTypeUpdateCommandHandler(
            mockUnitOfWork.Object,
            mockLogger.Object,
            mockMapper.Object,
            mockService.Object,
            mockPersonAgeTypeSpaTaxDataService.Object
        );

        var payload = new PersonAgeTypeUpdateRequest(
            "NewName",
            7,
            12,
            false,
            new MetaOfBathingTaxAgeUpdateRequest("GroupTest", FoodBeds.Food, FoodBeds.Bed, 1),
            new List<SpaOfBathingTaxAgeUpdateRequest>()
        ) { Id = existing.Id };

        var command = new PersonAgeTypeUpdateCommand { Payload = payload };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existing.Id, result);
    }
}
