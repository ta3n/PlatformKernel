using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Area;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class AreaEnableCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldEnableAreaAndReturnAreaId_WhenAreaIsEnabled()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<AresEnableCommandHandler>>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var areaServiceMock = new Mock<IAreaService>();
        var areaId = 1;
        var request = new AreaEnableCommand { Payload = new AreaEnabledRequest(true) { Id = areaId } };

        var area = new Area
        {
            Id = areaId,
            Name = "Test Area",
            Description = "This is a fake area",
            ParentId = null,
            Parent = null,
            Children = []
        };

        areaServiceMock
            .Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(area);

        var handler = new AresEnableCommandHandler(
            loggerMock.Object,
            MockUnitOfWork,
            MockMapper,
            areaServiceMock.Object,
            facilityRepositoryMock.Object
        );

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(areaId, result);
    }
}
