using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using MediatR;
using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class FacilityUpdateCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidRequest_UpdatesFacilityAndSites()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<FacilityUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockCacheService = new Mock<ICacheService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockSiteService = new Mock<ISiteService>();
        var mockFacilitySiteService = new Mock<IFacilitySiteService>();

        var payload = new FacilityUpdateRequest(new long[] { 2, 3 }, true, "test@example.com", "Memo") { Id = 1 };

        var command = new FacilityUpdateCommand { Payload = payload };
        var cancellationToken = CancellationToken.None;

        mockSiteService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), cancellationToken))
            .ReturnsAsync(2);

        mockFacilityService.Setup(f => f.CountByIdsAsync(It.IsAny<long[]>(), cancellationToken))
            .ReturnsAsync(1);

        mockFacilitySiteService.Setup(f => f.FindAllByFacilityIdAsync(It.IsAny<long>(), cancellationToken))
            .ReturnsAsync(
                new List<FacilitySite>
                {
                    new()
                    {
                        FacilityId = 1,
                        SiteId = 2,
                        IsEnabled = true
                    }
                }
            );

        mockMapper.Setup(m => m.Map<Facility>(It.IsAny<FacilityUpdateRequest>()))
            .Returns(
                new Facility
                {
                    Id = 1,
                    Meta = new FacilityMeta()
                }
            );

        mockMapper.Setup(m => m.Map<FacilityResponse>(It.IsAny<Facility>()))
            .Returns(new FacilityResponse(1, "Code", 1, true, "Memo"));

        mockFacilityService
            .Setup(f => f.UpdateAsync(It.IsAny<Facility>(), false, It.IsAny<Func<Facility, Facility, Facility>?>(), cancellationToken))
            .ReturnsAsync(new Facility { Id = 1 });

        mockFacilitySiteService.Setup(f => f.DeleteRangeAsync(It.IsAny<List<FacilitySite>>(), false, cancellationToken))
            .ReturnsAsync(new List<FacilitySite>());

        mockFacilitySiteService.Setup(f => f.CreateRangeAsync(It.IsAny<List<FacilitySite>>(), false, cancellationToken))
            .ReturnsAsync(new List<FacilitySite>());

        mockCacheService.Setup(c => c.ResetAsync(It.IsAny<string>(), false, cancellationToken))
            .Returns(Task.CompletedTask);
        var commandHandler = new FacilityUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMediator.Object,
            mockCacheService.Object,
            mockFacilityService.Object,
            mockSiteService.Object,
            mockFacilitySiteService.Object
        );
        // Act
        var result = await commandHandler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task HandleAsync_InvalidSiteIds_ThrowsSiteNotFoundException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<FacilityUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockCacheService = new Mock<ICacheService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockSiteService = new Mock<ISiteService>();
        var mockFacilitySiteService = new Mock<IFacilitySiteService>();

        var commandHandler = new FacilityUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockMediator.Object,
            mockCacheService.Object,
            mockFacilityService.Object,
            mockSiteService.Object,
            mockFacilitySiteService.Object
        );

        var payload = new FacilityUpdateRequest(new long[] { 2, 3 }, true, "test@example.com", "Memo") { Id = 1 };

        var command = new FacilityUpdateCommand { Payload = payload };
        var cancellationToken = CancellationToken.None;

        mockSiteService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), cancellationToken))
            .ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<SiteNotfoundException>(
            () =>
                commandHandler.Handle(command, cancellationToken)
        );
    }
}
