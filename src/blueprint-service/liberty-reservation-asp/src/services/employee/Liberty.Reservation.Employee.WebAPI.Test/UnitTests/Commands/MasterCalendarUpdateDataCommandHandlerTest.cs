using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;
using Liberty.Cache.Services;
using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class MasterCalendarUpdateDataCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesNewDateDataIfNotExists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockAppDateService = new Mock<IAppDateService>();
        var mockDateDataOfAppDateService = new Mock<IDateDataOfAppDateService>();

        var payload = new MasterCalendarUpdateDataCommand
        {
            Payload = new MasterCalendarEditDataRequest(AppDate.GetId(DateTime.Now), "Test Event")
        };

        var existingAppDate = new AppDate();
        mockAppDateService.Setup(service => service.FindByAppDateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAppDate);

        var existingDateDataOfAppDate = new AppDateAppDateData();
        mockDateDataOfAppDateService.Setup(service => service.FindByAppDateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDateDataOfAppDate);

        mockDateDataOfAppDateService.Setup(
                service => service.CreateAsync(
                    It.IsAny<AppDateAppDateData>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new AppDateAppDateData());

        var handler = new MasterCalendarUpdateDataCommandHandler(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockAppDateService.Object,
            mockDateDataOfAppDateService.Object
        );

        // Act
        var result = await handler.Handle(payload, CancellationToken.None);

        // Assert
        Assert.Equal(payload.Payload.AppDate, result.AppDate);
        Assert.Equal(payload.Payload.Name, result.Name);
    }
}
