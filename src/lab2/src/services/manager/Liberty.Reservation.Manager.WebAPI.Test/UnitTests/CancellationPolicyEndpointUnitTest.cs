using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class CancellationPolicyEndpointUnitTest : BaseUnitTest
{
    private ICancellationService MockCancellationService { get; set; } = null!;

    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<CancellationPolicyCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<CancellationPolicyUpdateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<CancellationPolicyDeleteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<CancellationPolicyGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    CancellationPolicyGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var cancellations = new List<CancellationPolicyResponse>
                    {
                        new(1, "Test 1", true, string.Empty, "a"),
                        new(2, "Test 2", true, string.Empty, "a"),
                        new(3, "Test 3", true, string.Empty, "a")
                    };

                    return (new HeaderDictionary(), cancellations);
                }
            );

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<CancellationPolicyGetQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    CancellationPolicyGetQuery _,
                    CancellationToken _
                ) =>
                {
                    var cancellations = new CancellationPolicyDetailResponse(
                        1,
                        "Test 1",
                        "Cancellation Description",
                        "Rule Detail",
                        new CancellationMeta("TableSource"),
                        []
                    );

                    return (new HeaderDictionary(), cancellations);
                }
            );

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<CancellationPolicyEnableCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        MockMediator = mockMediator.Object;

        var mockCancellationService = new Mock<ICancellationService>();

        mockCancellationService
            .Setup(
                x => x.ArrangeOrderAsync(
                    It.IsAny<List<long>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        MockCancellationService = mockCancellationService.Object;
    }

    [Fact]
    public async Task CreateCancellation_ReturnsCorrectResult()
    {
        var cancellation = new Cancellation
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new CancellationPolicyCreateRequest(
            "Test 1",
            "Cancellation Description"
        );

        // Act
        var result = await controller.CreateCancellationPolicy(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(cancellation.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateCancellation_ReturnsCorrectResult()
    {
        var cancellation = new Cancellation
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new CancellationPolicyUpdateRequest(
            "Test 1",
            "Cancellation Description",
            "Rule Detail",
            []
        );

        // Act
        var result = await controller.UpdateCancellationPolicy(cancellation.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Cancellation.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(cancellation.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteCancellation_ReturnsCorrectResult()
    {
        var cancellation = new Cancellation
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteCancellationPolicy(cancellation.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Cancellation.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(cancellation.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllCancellation_ReturnsCorrectResult()
    {
        var cancellations = new List<Cancellation>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 2" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
            },
            new()
            {
                Id = 3,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 3" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
            }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllCancellationPolicies(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<CancellationPolicyResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(cancellations.Count, response.Count);
        Assert.Equal(cancellations[0].Id, response[0].Id);
        Assert.Equal(cancellations[1].Id, response[1].Id);
        Assert.Equal(cancellations[2].Id, response[2].Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetCancellation_ReturnsCorrectResult()
    {
        var cancellation = new Cancellation
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetCancellationPolicy(cancellation.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<CancellationPolicyDetailResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(cancellation.Id, response.Id);
        Assert.Equal(cancellation.Name.GetValueByHeader(), response.Name);
        Assert.Equal(cancellation.Description.GetValueByHeader(), response.Description);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderOfCancellations_ReturnsCorrectResult()
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
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ArrangeOrderOfCancellationPolicies(request, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnableCancellation_ReturnsCorrectResult()
    {
        var cancellation = new Cancellation
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation Description" } }
        };

        var cancellationEnabledRequest = new CancellationPolicyEnabledRequest(true);
        var mockMapper = MockServices.MockMapper(
            (cancellationEnabledRequest, cancellation)
        );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new CancellationPoliciesEndpoint(
            mockMapper,
            MockMediator,
            MockCancellationService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.EnableCancellationPolicy(cancellation.Id, cancellationEnabledRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Cancellation.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(cancellation.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }
}
