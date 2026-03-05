using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Area;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class AreasEndpointUnitTest : BaseUnitTest
{
    private IAreaService MockAreaService { get; set; } = null!;

    protected override void InitData()
    {
        var mockAreaService = new Mock<IAreaService>();

        mockAreaService.Setup(
                x => x.CreateAsync(
                    It.IsAny<Area>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Area
                {
                    Id = 1,
                    Name = "Area1",
                    Description = "Description1",
                    ParentId = null
                }
            );

        mockAreaService.Setup(
                x => x.UpdateAsync(
                    It.IsAny<Area>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Area, Area, Area>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Area
                {
                    Id = 1,
                    Name = "UpdatedArea",
                    Description = "UpdatedDescription",
                    ParentId = null
                }
            );

        mockAreaService.Setup(
                x => x.DeleteAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Area
                {
                    Id = 1,
                    Name = "DeletedArea",
                    Description = "DeletedDescription",
                    ParentId = null
                }
            );

        mockAreaService.Setup(
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
                    var areas = new List<Area>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Area1",
                            Description = "Description1",
                            ParentId = null
                        },
                        new()
                        {
                            Id = 2,
                            Name = "Area2",
                            Description = "Description2",
                            ParentId = 1
                        }
                    };

                    var mockPage = new Mock<IPage<Area>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)areas.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < areas.Count);
                    mockPage.Setup(p => p.Content).Returns(areas);

                    return mockPage.Object;
                }
            );

        mockAreaService.Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Area
                {
                    Id = 1,
                    Name = "Area1",
                    Description = "Description1",
                    ParentId = null
                }
            );

        mockAreaService.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Area
                {
                    Id = 1,
                    Name = "Area1",
                    Description = "Description1",
                    ParentId = null
                }
            );

        MockAreaService = mockAreaService.Object;
    }

    [Fact]
    public async Task CreateArea_ReturnOkObjectResult()
    {
        // Arrange
        var area = new Area
        {
            Id = 1,
            Name = "AreaName",
            Description = "Description",
            ParentId = null
        };
        var areaCreateRequest = new AreaCreateRequest("AreaName");
        var mockMapper = MockServices.MockMapper((areaCreateRequest, area));
        var controller = new AreasEndpoint(mockMapper, MockMediator, MockAreaService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CreateArea(areaCreateRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(area.Id, okResult.Value);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Area.Created", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(area.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateArea_ReturnsCorrectResult()
    {
        // Arrange
        var area = new Area
        {
            Id = 1,
            Name = "AreaName",
            Description = "Description",
            ParentId = null
        };
        var areaUpdateRequest = new AreaUpdateRequest("UpdatedName");
        var mockMapper = MockServices.MockMapper((areaUpdateRequest, area));
        var controller = new AreasEndpoint(mockMapper, MockMediator, MockAreaService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.UpdateArea(area.Id, areaUpdateRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Area.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(area.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteArea_ReturnsCorrectResult()
    {
        // Arrange
        var mockMapper = MockServices.MockMapper();
        var controller = new AreasEndpoint(mockMapper, MockMediator, MockAreaService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteArea(1, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Area.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal("1", headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }

    [Fact]
    public async Task EnableArea_ReturnsCorrectResult()
    {
        // Arrange
        var area = new Area
        {
            Id = 1,
            Name = "AreaName",
            Description = "AreaDescription"
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<AreaEnableCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(area.Id);

        var areaEnabledRequest = new AreaEnabledRequest(true);

        var mockMapper = MockServices.MockMapper(
            (areaEnabledRequest, area)
        );
        var controller = new AreasEndpoint(mockMapper, mockMediator.Object, MockAreaService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.EnableArea(area.Id, areaEnabledRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var nocontentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Area.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(area.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, nocontentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllAreas_ReturnsCorrectResult()
    {
        // Arrange
        var areas = new List<Area>
        {
            new()
            {
                Id = 1,
                Name = "Area1",
                Description = "Desc1",
                ParentId = null
            },
            new()
            {
                Id = 2,
                Name = "Area2",
                Description = "Desc2",
                ParentId = 1
            }
        };
        var mockMapper = MockServices.MockMapper(
            (areas, new List<AreaResponse>
            {
                new(1, "Desc1", "Area1", true),
                new(2, "Desc2", "Area2", true)
            })
        );
        var controller = new AreasEndpoint(mockMapper, MockMediator, MockAreaService);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllAreas(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<AreaResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("Area1", response[0].Name);
        Assert.Equal("Area2", response[1].Name);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetArea_ReturnsCorrectResult()
    {
        // Arrange
        var area = new Area
        {
            Id = 1,
            Name = "Area1",
            Description = "Description1",
            ParentId = null
        };
        var areaResponse = new AreaResponse(1, "Area1", "Description1", true);
        var mockMapper = MockServices.MockMapper((area, areaResponse));
        var controller = new AreasEndpoint(mockMapper, MockMediator, MockAreaService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetArea(area.Id, cancellationToken);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
