using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class DestinationsEndpointUnitTest : BaseUnitTest
{
    private ISiteService MockSiteService { get; set; } = null!;
    private ICacheService MockCacheService { get; set; } = null!;

    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();
        var mockCacheService = new Mock<ICacheService>();

        mockCacheService
            .Setup(
                x => x.RemoveByPatternsAsync(
                    true,
                    It.Is<string>(pattern => pattern.Contains(nameof(FacilityGetAllDestinationsQueryHandler)))
                )
            )
            .Returns(Task.CompletedTask);

        MockCacheService = mockCacheService.Object;

        var destinationResponse = new List<DestinationResponse>
        {
            new()
            {
                Id = 1,
                Name = "Destination 1",
                DisplayOrder = 1,
                IsEnabled = true
            },
            new()
            {
                Id = 2,
                Name = "Destination 2",
                DisplayOrder = 2,
                IsEnabled = true
            }
        };

        var headers = new HeaderDictionary();

        mockMediator.Setup(i => i.Send(It.IsAny<DestinationCreateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DestinationResponse
                {
                    Id = 1,
                    Code = "DST001",
                    Name = "Destination 1",
                    ShortName = "Dest1",
                    Url = "https://example.com",
                    DisplayOrder = 1,
                    IsEnabled = true
                }
            );

        mockMediator.Setup(i => i.Send(It.IsAny<DestinationUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DestinationResponse
                {
                    Id = 1,
                    Code = "DST001",
                    Name = "Destination 1",
                    ShortName = "Dest1",
                    Url = "https://example.com",
                    DisplayOrder = 1,
                    IsEnabled = true
                }
            );

        mockMediator.Setup(m => m.Send(It.IsAny<DestinationGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((headers, destinationResponse));

        mockMediator.Setup(m => m.Send(It.IsAny<DestinationGetQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (new HeaderDictionary(), new DestinationDetailResponse(
                    1,
                    "DST001",
                    "Destination 1",
                    "Dest1",
                    "BB",
                    "https://example.com",
                    true
                ))
            );

        MockMediator = mockMediator.Object;

        var mockSiteService = new Mock<ISiteService>();

        mockSiteService.Setup(service => service.ArrangeOrderAsync(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSiteService.Setup(
                service => service
                    .EnableAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new Site
                {
                    Id = 1,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } },
                    ShortName = "TS",
                    Url = "https://testsite.com",
                    Description = "This is a test site",
                    Memo = "Test memo",
                    UseSitePoint = true,
                    IsMaster = true,
                    Tag = "TestTag"
                }
            );

        mockSiteService.Setup(service => service.DeleteAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Site
                {
                    Id = 1,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } },
                    ShortName = "TS",
                    Url = "https://testsite.com",
                    Description = "This is a test site",
                    Memo = "Test memo",
                    UseSitePoint = true,
                    IsMaster = true,
                    Tag = "TestTag"
                }
            );

        MockSiteService = mockSiteService.Object;
    }

    [Fact]
    public async Task CreateDestination_ReturnOkObjectResult()
    {
        // Arrange
        var request = new DestinationCreateRequest(
            "Destination 1",
            "Dest1",
            "CC",
            "https://example.com"
        );
        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.CreateDestination(request, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Site.Created", headers["X-Liberty-alert"]);
        Assert.IsType<long>(okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateDestination_ReturnsNoContentResultWithHeaders()
    {
        // Arrange
        var request = new DestinationUpdateRequest(
            "Code 1",
            "Destination 1",
            "Des 1",
            "AB",
            "https://example.com"
        );
        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.UpdateDestination(1, request, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Site.Updated", headers["X-Liberty-alert"]);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderOfDestinations_ReturnsNoContentResult()
    {
        // Arrange
        var request = new ItemUpdateOrderRequest(
            [
                1,
                2,
                3
            ]
        );
        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.ArrangeOrderOfDestinations(request, CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnableDestination_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var request = new DestinationEnabledRequest(true);
        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.EnableDestination(1, request, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Site.Updated", headers["X-Liberty-alert"]);
        Assert.IsType<long>(okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task DeleteDestination_ReturnsNoContentResultWithHeaders()
    {
        // Arrange
        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.DeleteDestination(1, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Site.Deleted", headers["X-Liberty-alert"]);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllDestinations_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var mockPageable = new Mock<IPageable>();

        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.GetAllDestinations(mockPageable.Object, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetDestination_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new DestinationsEndpoint(
            MockMapper,
            MockMediator,
            MockCacheService,
            MockSiteService
        );

        // Act
        var result = await controller.GetDestination(1, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
