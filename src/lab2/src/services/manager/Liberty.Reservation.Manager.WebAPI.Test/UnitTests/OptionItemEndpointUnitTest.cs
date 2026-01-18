using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class OptionItemEndpointUnitTest : BaseUnitTest
{
    private IOptionItemService MockOptionItemService { get; set; } = null!;
    private ISecurityContextAccessor MockSecurityContextAccessor { get; set; } = null!;

    protected override void InitData()
    {
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        MockSecurityContextAccessor = mockSecurityContextAccessor.Object;
        mockOptionItemService.Setup(
                x => x.ArrangeOrderAsync(
                    It.IsAny<List<long>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        mockOptionItemService.Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new OptionItem
                {
                    Id = 1,
                    Description = "Test",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } }
                }
            );

        MockOptionItemService = mockOptionItemService.Object;

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemDeleteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemUpdateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    OptionItemGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var optionItems = new List<OptionItemResponse>
                    {
                        new(1, "Test", "Description", 1, 1, true, true, []),
                        new(2, "Test", "Description", 1, 1, true, true, []),
                        new(3, "Test", "Description", 1, 1, true, true, [])
                    };

                    return (new HeaderDictionary(), optionItems);
                }
            );

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<OptionItemGetQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    OptionItemGetQuery _,
                    CancellationToken _
                ) =>
                {
                    var optionItem = new OptionItemDetailResponse(
                        1,
                        "Test",
                        "Description",
                        1,
                        2,
                        1,
                        [],
                        [],
                        [],
                        [],
                        true
                    );

                    return (new HeaderDictionary(), optionItem);
                }
            );

        MockMediator = mockMediator.Object;

        mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        mockSecurityContextAccessor.Setup(m => m.FacilityKey).Returns(1);
        MockSecurityContextAccessor = mockSecurityContextAccessor.Object;
    }

    [Fact]
    public async Task CreateOptionItemAsync()
    {
        var request = new OptionItemCreateRequest("Test", "Description", 1, 1);

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CreateOptionItem(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task EnableOptionItemAsync()
    {
        var optionItem = new OptionItem
        {
            Id = 1,
            Description = "Test",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } }
        };

        var request = new OptionItemEnabledRequest(true);
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.EnableOptionItem(1, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.OptionItem.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(optionItem.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateOptionItemAsync()
    {
        var optionItem = new OptionItem
        {
            Id = 1,
            Description = "Test",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } }
        };

        var request = new OptionItemUpdateRequest(
            "Option item",
            "Description",
            1,
            2,
            1,
            [1],
            [1],
            [1],
            []
        );

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.UpdateOptionItem(1, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.OptionItem.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(optionItem.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderAsync()
    {
        var request = new ItemUpdateOrderRequest(
            [
                1,
                2,
                3
            ]
        );
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ArrangeOrderOfOptionItems(request, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteOptionItem_ReturnsCorrectResult()
    {
        var option = new OptionItem
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = "Option Item Description"
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteOptionItem(option.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.OptionItem.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(option.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllOptions_ReturnsCorrectResult()
    {
        var options = new List<OptionItem>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
                Description = "Cancellation Description"
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 2" } },
                Description = "Cancellation Description"
            },
            new()
            {
                Id = 3,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 3" } },
                Description = "Cancellation Description"
            }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllOptionItems(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<OptionItemResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(options.Count, response.Count);
        Assert.Equal(options[0].Id, response[0].Id);
        Assert.Equal(options[1].Id, response[1].Id);
        Assert.Equal(options[2].Id, response[2].Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetOptionItem_ReturnsCorrectResult()
    {
        var option = new OptionItem
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = "Option Item Description"
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new OptionItemsEndpoint(
            mockMapper,
            MockMediator,
            MockOptionItemService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetOptionItem(option.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<OptionItemDetailResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(option.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
