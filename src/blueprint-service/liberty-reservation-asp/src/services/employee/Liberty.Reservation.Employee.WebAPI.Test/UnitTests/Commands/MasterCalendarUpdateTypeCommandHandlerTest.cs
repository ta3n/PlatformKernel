using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.UnitOfWork.Abstractions;
using Liberty.Cache.Services;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class MasterCalendarUpdateTypeCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidRequest_CreatesNewDateTypeOfAppDate_WhenNoExistingTypes()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockAppDateService = new Mock<IAppDateService>();
        var mockAppDateTypeService = new Mock<IAppDateTypeService>();
        var mockDateTypeOfAppDateService = new Mock<IDateTypeOfAppDateService>();
        var mockLogger = new Mock<ILogger<MasterCalendarUpdateTypeCommandHandler>>();

        var payload = new MasterCalendarUpdateTypeCommand { Payload = new MasterCalendarEditTypeRequest(AppDate.GetId(DateTime.Now), 1) };

        var existingAppDateType = new AppDateType
        {
            Id = 1,
            Code = "Type1",
            Name = "Existing Type",
            ShortName = "T1",
            Color = "Red"
        };
        var existingAppDate = new AppDate
        {
            Id = 123,
            DateTime = DateTime.Now
        };

        mockAppDateTypeService.Setup(service => service.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAppDateType);
        mockAppDateService.Setup(service => service.FindByAppDateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAppDate);
        mockDateTypeOfAppDateService.Setup(service => service.FindAllByAppDateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<AppDateAppDateType>().ToList());

        mockDateTypeOfAppDateService
            .Setup(service => service.CreateAsync(It.IsAny<AppDateAppDateType>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppDateAppDateType());

        var commandHandler = new MasterCalendarUpdateTypeCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockAppDateService.Object,
            mockAppDateTypeService.Object,
            mockDateTypeOfAppDateService.Object
        );

        // Act
        var result = await commandHandler.Handle(payload, CancellationToken.None);

        // Assert
        Assert.Equal("Type1", result.Code);
        Assert.Equal("Existing Type", result.Name);
        Assert.Equal("T1", result.ShortName);
        Assert.Equal("Red", result.Color);

        mockDateTypeOfAppDateService.Verify(
            service => service.CreateAsync(It.IsAny<AppDateAppDateType>(), false, It.IsAny<CancellationToken>()),
            Times.Once
        );
        mockUnitOfWork.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
