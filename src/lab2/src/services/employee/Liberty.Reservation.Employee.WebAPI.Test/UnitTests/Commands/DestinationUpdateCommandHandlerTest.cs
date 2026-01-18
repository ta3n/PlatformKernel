using Liberty.Cache.Services;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class DestinationUpdateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateDestinationAndReturnResponse()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DestinationUpdateCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();
        var siteServiceMock = new Mock<ISiteService>();

        var handler = new DestinationUpdateCommandHandler(
            loggerMock.Object,
            MockUnitOfWork,
            mapperMock.Object,
            cacheServiceMock.Object,
            siteServiceMock.Object
        );

        var destinationId = 1;
        var request = new DestinationUpdateCommand
        {
            Payload = new DestinationUpdateRequest(
                "DEST123",
                "Name",
                "Short Name",
                "AB",
                "Test.test"
            ) { Id = destinationId }
        };

        var destination = new Site
        {
            Id = destinationId,
            Code = "DEST123",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } }
        };

        var expectedResponse = new DestinationResponse
        {
            Id = destinationId,
            Name = "Test Destination",
            Code = "DEST123"
        };

        mapperMock
            .Setup(x => x.Map<Site>(request.Payload))
            .Returns(destination);

        mapperMock
            .Setup(x => x.Map<DestinationResponse>(destination))
            .Returns(expectedResponse);

        siteServiceMock
            .Setup(x => x.CheckExistingCode(It.IsAny<string>(), It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        siteServiceMock
            .Setup(
                x => x.UpdateAsync(
                    It.IsAny<Site>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Site, Site, Site>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(destination);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(destinationId, result.Id);
        Assert.Equal("Test Destination", result.Name);
        Assert.Equal("DEST123", result.Code);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenCodeExists()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DestinationUpdateCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();
        var siteServiceMock = new Mock<ISiteService>();

        siteServiceMock
            .Setup(x => x.CheckExistingCode(It.IsAny<string>(), It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DestinationUpdateCommandHandler(
            loggerMock.Object,
            MockUnitOfWork,
            mapperMock.Object,
            cacheServiceMock.Object,
            siteServiceMock.Object
        );

        var request = new DestinationUpdateCommand
        {
            Payload = new DestinationUpdateRequest(
                "DEST123",
                "Name",
                "Short Name",
                "AB",
                "Test.test"
            ) { Id = 1 }
        };

        // Act & Assert
        await Assert.ThrowsAsync<SiteDuplicatedCodeException>(
            () => handler.Handle(request, CancellationToken.None)
        );
    }
}
