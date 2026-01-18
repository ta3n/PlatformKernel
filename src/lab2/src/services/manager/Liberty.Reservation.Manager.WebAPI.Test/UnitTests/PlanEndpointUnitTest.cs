using System.Net;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PlanEndpointUnitTest : BaseUnitTest
{
    private IPlanService MockPlanService { get; set; } = null!;

    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateBasicSettingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateRoomTypeCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateDisplayCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdatePublishAcceptCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateSaleCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdatePaymentMethodCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateMealCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateOptionCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateCancelCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateSpecialCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateQuestionCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanUpdateImportantNoteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanEnabledCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanEnabledRoomTypeCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanDeleteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var plans = new List<PlanResponse>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Plan1",
                            UseDisplayDate = true,
                            DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                            DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                            UseAcceptDate = true,
                            AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                            AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                            IsEnabled = true,
                            IsOnSidePayment = true,
                            IsOnLinePayment = true,
                            HasWarning = true
                        },
                        new()
                        {
                            Id = 2,
                            Name = "Plan2",
                            UseDisplayDate = true,
                            DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                            DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                            UseAcceptDate = true,
                            AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                            AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                            IsEnabled = true,
                            IsOnSidePayment = true,
                            IsOnLinePayment = true,
                            HasWarning = true
                        },
                        new()
                        {
                            Id = 3,
                            Name = "Plan3",
                            UseDisplayDate = true,
                            DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                            DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                            UseAcceptDate = true,
                            AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                            AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                            IsEnabled = true,
                            IsOnSidePayment = true,
                            IsOnLinePayment = true,
                            HasWarning = true
                        }
                    };

                    return (new HeaderDictionary(), plans);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetAllDestinationsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetAllDestinationsQuery _,
                    CancellationToken _
                ) =>
                {
                    var plans = new List<SiteResponse>
                    {
                        new(1, "Site 1", "Site 1", "site1.com", "Desciption", true),
                        new(2, "Site 2", "Site 2", "site2.com", "Desciption", true),
                        new(3, "Site 3", "Site 3", "site3.com", "Desciption", true)
                    };

                    return (new HeaderDictionary(), plans);
                }
            );

        MockMediator = mockMediator.Object;

        var mockPlanService = new Mock<IPlanService>();

        mockPlanService.Setup(
                x => x.ArrangeOrderAsync(
                    It.IsAny<List<long>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        MockPlanService = mockPlanService.Object;
    }

    [Fact]
    public async Task CreatePlan_ReturnCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var planToken = CancellationToken.None;

        var request = new PlanCreateRequest(
            "Test 1",
            "Plan Description",
            false,
            PlanTypes.Combo
        );

        // Act
        var result = await controller.CreatePlanAsync(request, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(plan.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBasicSettingPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateBasicSettingRequest(
            "Test 1",
            "Test 1",
            "Test 1",
            "Plan Description",
            []
        );

        // Act
        var result = await controller.UpdatePlanBasicSetting(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomTypePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateRoomTypeRequest
        (
            []
        );

        // Act
        var result = await controller.UpdatePlanRoomType(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateDisplayPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateDisplayRequest(
            ["Tag"],
            [1, 2],
            [3, 4]
        );

        // Act
        var result = await controller.UpdatePlanDisplay(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePublicAcceptPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            false,
            1,
            TimeSpan.Zero,
            []
        );

        // Act
        var result = await controller.UpdatePlanPublishAccept(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateSalePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateSaleRequest(
            new TimeSpan(0, 10, 0),
            new TimeSpan(0, 10, 0),
            new TimeSpan(0, 10, 0),
            0,
            1,
            1,
            PlanDaySaleLimitTypes.Pair,
            true,
            1,
            1,
            true,
            1,
            1,
            1,
            1
        );

        // Act
        var result = await controller.UpdatePlanSale(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePaymentMethodPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdatePaymentMethodRequest(
            true,
            true
        );

        // Act
        var result = await controller.UpdatePlanPaymentMethod(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatemealPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateMealRequest(
            [
                new PlanMealTypeRequest(1, MealTypeEatTypes.Box),
                new PlanMealTypeRequest(2, MealTypeEatTypes.Room)
            ]
        );

        // Act
        var result = await controller.UpdatePlanMeal(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateOptionPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateOptionRequest(
            true,
            [1, 2]
        );

        // Act
        var result = await controller.UpdatePlanOption(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateCancelPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateCancelRequest(
            true,
            1,
            new TimeSpan(0, 10, 0),
            1
        );

        // Act
        var result = await controller.UpdatePlanCancel(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateSpecialPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateSpecialRequest(
            true,
            "Secret"
        );

        // Act
        var result = await controller.UpdatePlanSpecial(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestionPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateQuestionRequest(
            [1]
        );

        // Act
        var result = await controller.UpdatePlanQuestion(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateImportantNotePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanUpdateSpecialRequest(
            true,
            "Secret"
        );

        // Act
        var result = await controller.UpdatePlanSpecial(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnablePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanEnabledRequest(
            true
        );

        // Act
        var result = await controller.PlanEnabled(plan.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnableRoomTypePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };

        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room Group 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            IsEnabled = true
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PlanEnabledRoomTypeRequest(
            true
        );

        // Act
        var result = await controller.EnabledPlanRoomType(plan.Id, roomGroup.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(roomGroup.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeletePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeletePlan(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Plan.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(plan.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllPlan_ReturnsCorrectResult()
    {
        var plans = new List<Plan>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan1" } },
                UseDisplayDate = true,
                DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                UseAcceptDate = true,
                AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                IsEnabled = true,
                IsOnSidePayment = true,
                IsOnLinePayment = true
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan2" } },
                UseDisplayDate = true,
                DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                UseAcceptDate = true,
                AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                IsEnabled = true,
                IsOnSidePayment = true,
                IsOnLinePayment = true
            },
            new()
            {
                Id = 3,
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan3" } },
                UseDisplayDate = true,
                DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                UseAcceptDate = true,
                AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                IsEnabled = true,
                IsOnSidePayment = true,
                IsOnLinePayment = true
            }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var mockPagable = new Mock<IPageable>();

        // Act
        var result = await controller.GetPlans(mockPagable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<PlanResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plans.Count, response.Count);
        Assert.Equal(plans[0].Id, response[0].Id);
        Assert.Equal(plans[1].Id, response[1].Id);
        Assert.Equal(plans[2].Id, response[2].Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetAllDestinationsPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan1" } },
            UseDisplayDate = true,
            DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
            DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
            UseAcceptDate = true,
            AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
            AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
            IsEnabled = true,
            IsOnSidePayment = true,
            IsOnLinePayment = true
        };

        var sites = new List<Site>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site 1" } },
                Description = "Site 1",
                Url = "site1.com"
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site 2" } },
                Description = "Site 2",
                Url = "site2.com"
            },
            new()
            {
                Id = 3,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site 3" } },
                Description = "Site 3",
                Url = "site3.com"
            }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var mockPagable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllDestinationOfPlan(plan.Id, mockPagable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<SiteResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(sites.Count, response.Count);
        Assert.Equal(sites[0].Id, response[0].Id);
        Assert.Equal(sites[1].Id, response[1].Id);
        Assert.Equal(sites[2].Id, response[2].Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderOfPlans_ReturnsCorrectResult()
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
        var controller = new PlansEndpoint(
            mockMapper,
            MockMediator,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ArrangeOrderOfPlans(request, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetBasicSettingPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanBasicSettingResponse
                    {
                        Id = 1,
                        Name = "Test 1",
                        NameForImport = "Test 1",
                        Summary = "Plan Summary",
                        Description = "Plan Description",
                        Files = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(mockMapper, mockMediator.Object, MockPlanService, mockCacheManagementService.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.PlanGetBasicSetting(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanBasicSettingResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetDisplayPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanDisplayResponse
                    {
                        Id = 1,
                        Tags = ["Tag"],
                        MasterCategories = [],
                        PlanCategories = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanDisplay(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanDisplayResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetRoomTypePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanRoomTypeResponse
                    {
                        Id = 1,
                        RoomGroups = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanRoomType(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanRoomTypeResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPublishAcceptPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanPublishAcceptResponse
                    {
                        Id = 1,
                        UseDisplayDate = true,
                        DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                        DisplayDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                        UseAcceptDate = true,
                        AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                        AcceptDateEnd = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                        AcceptDays = 2,
                        AcceptMonths = 2,
                        AcceptEndLimitType = PlanAcceptEndLimitTypes.AfterDays,
                        ReceptionDayLimit = 1,
                        ReceptionLimit = new TimeSpan(0, 10, 0),
                        Sites = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanPublishAccept(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanPublishAcceptResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetSalePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanSaleResponse
                    {
                        Id = 1,
                        CheckInStart = new TimeSpan(14, 0, 0), // 2:00 PM
                        CheckInEnd = new TimeSpan(18, 0, 0), // 6:00 PM
                        CheckOut = "11:00 AM",
                        RoomNumberDaySaleLimit = 10,
                        GroupNumberDaySaleLimit = 5,
                        PlanDaySaleLimitType = PlanDaySaleLimitTypes.Pair, // Replace with actual enum value
                        UseDaySaleLimit = true,
                        UseAcceptPersonNumber = true,
                        AcceptPersonNumberMin = 1,
                        AcceptPersonNumberMax = 4,
                        NumberOfStayLimitMin = 1,
                        NumberOfStayLimitMax = 7,
                        PointRate = 0.05f,
                        PointExpire = 365,
                        MaxRoomNumberDaySaleLimit = 20
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanSale(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanSaleResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPaymentMethodPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanPaymentMethodResponse
                    {
                        Id = 1,
                        IsOnLinePayment = true,
                        IsOnSidePayment = true
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanPaymentMethod(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanPaymentMethodResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetMealPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanMealResponse
                    {
                        Id = 1,
                        MealTypes =
                        [
                            new(1, MealTypeEatTypes.Box),
                            new(2, MealTypeEatTypes.Room)
                        ]
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanPaymentMethod(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanMealResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetOptionPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanOptionResponse
                    {
                        Id = 1,
                        UseFixedOptionItem = true,
                        OptionItems = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanOption(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanOptionResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetCancelPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanCancelResponse
                    {
                        Id = 1,
                        IsCancelSameAccept = true,
                        CancelDayLimit = 1,
                        CancelLimit = new TimeSpan(0, 10, 0),
                        CancellationId = 1
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanCancel(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanCancelResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetQuestionPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanQuestionResponse
                    {
                        Id = 1,
                        Questions = [new(1, "Test 1", true), new(2, "Test 2", true)]
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanQuestion(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanQuestionResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetSpecialPlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanSpecialResponse
                    {
                        Id = 1,
                        IsSecret = true,
                        SecretWord = "Secret"
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanSpecial(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanSpecialResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetImportantNotePlan_ReturnsCorrectResult()
    {
        var plan = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };
        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new PlanImportantNoteResponse
                    {
                        Id = 1,
                        Payment = "Payment",
                        Meal = "Meal",
                        Other = "Other"
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PlansEndpoint(
            mockMapper,
            mockMediator.Object,
            MockPlanService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPlanImportantNote(plan.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanImportantNoteResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(plan.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
