using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;
using Liberty.SysException.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class AppDateTypesEndpointUnitTest : BaseUnitTest
{
    private IAppDateTypeService MockAppDateTypeService { get; set; } = null!;

    protected override void InitData()
    {
        var appDateTypeService = new Mock<IAppDateTypeService>();

        appDateTypeService.Setup(
                x => x.CreateAsync(
                    It.IsAny<AppDateType>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new AppDateType
                {
                    Id = 1,
                    Name = "AppName",
                    ShortName = "AppShortName",
                    Color = "Red",
                    Description = "Description"
                }
            );

        appDateTypeService.Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new AppDateType
                {
                    Id = 1,
                    Name = "AppName",
                    ShortName = "AppShortName",
                    Color = "Red",
                    Description = "Description"
                }
            );

        appDateTypeService.Setup(
                x => x.UpdateAsync(
                    It.IsAny<AppDateType>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<AppDateType, AppDateType, AppDateType>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new AppDateType
                {
                    Id = 1,
                    Name = "AppName",
                    ShortName = "AppShortName",
                    Color = "Red",
                    Description = "Description"
                }
            );

        appDateTypeService.Setup(
                x => x.DeleteAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new AppDateType
                {
                    Id = 1,
                    Name = "AppName",
                    ShortName = "AppShortName",
                    Color = "Red",
                    Description = "Description"
                }
            );

        appDateTypeService.Setup(
                x => x.FindAllAsync(
                    It.IsAny<IPageable>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    IPageable pageable,
                    CancellationToken _
                ) =>
                {
                    var appDateTypes = new List<AppDateType>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "AppName1",
                            ShortName = "Short1",
                            Color = "Red"
                        },
                        new()
                        {
                            Id = 2,
                            Name = "AppName2",
                            ShortName = "Short2",
                            Color = "Blue"
                        }
                    };

                    var mockPage = new Mock<IPage<AppDateType>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)appDateTypes.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < appDateTypes.Count);
                    mockPage.Setup(p => p.Content).Returns(appDateTypes);

                    return mockPage.Object;
                }
            );

        appDateTypeService.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new AppDateType
                {
                    Id = 1,
                    Name = "AppName",
                    ShortName = "AppShortName",
                    Color = "Red",
                    Description = "Description"
                }
            );

        MockAppDateTypeService = appDateTypeService.Object;
    }

    [Fact]
    public async Task CreateAppDateType_ReturnOkObjectResult()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };
        var appDateTypeCreateRequest = new AppDateTypeCreateRequest(
            "AppName",
            "AppShortName",
            "Red",
            "Description"
        );
        var mockMapper = MockServices.MockMapper(
            (appDateTypeCreateRequest, appDateType)
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CreateAppDateType(appDateTypeCreateRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(appDateType.Id, okResult.Value);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateType.Created", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(appDateType.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateAppDateType_ReturnsCorrectResult()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            "AppName",
            "AppShortName",
            "Red",
            "Description"
        );
        var mockMapper = MockServices.MockMapper(
            (appDateTypeUpdateRequest, appDateType)
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.UpdateAppDateType(appDateType.Id, appDateTypeUpdateRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateType.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(appDateType.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateAppDateType_ReturnAppRequestInvalidException()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            string.Empty,
            "AppShortName",
            "Red",
            "Description"
        );
        var mockMapper = MockServices.MockMapper(
            (appDateTypeUpdateRequest, appDateType)
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.UpdateAppDateType(appDateType.Id, appDateTypeUpdateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0001, exception.ErrorCode);
        Assert.Equal("Name", exception.ErrorField);
    }

    [Fact]
    public async Task EnableAppDateType_ReturnsCorrectResult()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };
        var appDateTypeEnabledRequest = new AppDateTypeEnabledRequest(true);

        var mockMapper = MockServices.MockMapper(
            (appDateTypeEnabledRequest, appDateType)
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.EnableAppDateType(appDateType.Id, appDateTypeEnabledRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateType.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(appDateType.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }

    [Fact]
    public async Task EnableAppDateType_ReturnAppRequestInvalidException()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };
        var appDateTypeEnabledRequest = new AppDateTypeEnabledRequest(true);

        var mockMapper = MockServices.MockMapper(
            (appDateTypeEnabledRequest, appDateType)
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.EnableAppDateType(-1, appDateTypeEnabledRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0001, exception.ErrorCode);
        Assert.Equal("Id", exception.ErrorField);
    }

    [Fact]
    public async Task DeleteAppDateType_ReturnsCorrectResult()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };
        _ = new AppDateTypeEnabledRequest(true);

        var mockMapper = MockServices.MockMapper();
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteAppDateType(appDateType.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateType.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(appDateType.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllAppDateTypes_ReturnsCorrectResult()
    {
        // Arrange
        var appDateTypes = new List<AppDateType>
        {
            new()
            {
                Id = 1,
                Name = "AppName1",
                ShortName = "Short1",
                Color = "Red",
                Description = "Description1"
            },
            new()
            {
                Id = 2,
                Name = "AppName2",
                ShortName = "Short2",
                Color = "Blue",
                Description = "Description2"
            }
        };
        var mockMapper = MockServices.MockMapper(
            (appDateTypes, new List<AppDateTypeResponse>
            {
                new(1, "AppName1", "Short1", "Red", "Description1", true),
                new(2, "AppName2", "Short2", "Blue", "Description2", true)
            })
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllAppDateTypes(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<AppDateTypeResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("AppName1", response[0].Name);
        Assert.Equal("AppName2", response[1].Name);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAppDateType_ReturnOkObjectResult()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };

        var appDateTypeResponse = new AppDateTypeResponse(
            1,
            "AppName1",
            "Short1",
            "Red",
            "Description1",
            true
        );
        var mockMapper = MockServices.MockMapper(
            (appDateType, appDateTypeResponse)
        );
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAppDateType(appDateType.Id, cancellationToken);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetAppDateType_ReturnNotFoundResult()
    {
        // Arrange
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "AppName",
            ShortName = "AppShortName",
            Color = "Red",
            Description = "Description"
        };

        _ = new AppDateTypeResponse(
            1,
            "AppName1",
            "Short1",
            "Red",
            "Description1",
            true
        );
        var mockMapper = MockServices.MockMapper();
        var controller = new AppDateTypesEndpoint(mockMapper, MockAppDateTypeService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAppDateType(appDateType.Id, cancellationToken);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
    }
}
