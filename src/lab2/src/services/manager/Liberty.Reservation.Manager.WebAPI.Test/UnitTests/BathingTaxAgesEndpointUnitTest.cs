using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class BathingTaxAgesEndpointUnitTest : BaseUnitTest
{
    private IPersonAgeTypeService MockPersonAgeTypeService { get; set; } = null!;

    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<BathingTaxAgeCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<BathingTaxAgeUpdateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<BathingTaxAgeChangeStatusCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<BathingTaxAgeGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    BathingTaxAgeGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var cancellations = new BathingTaxAgeResponse(
                        true,
                        "Spa Tax Comment",
                        null
                    )
                    {
                        BathingTaxAges =
                        [
                            new()
                            {
                                Id = 1,
                                Name = "Test 1",
                                AgeMax = 10,
                                AgeMin = 1,
                                IsMain = true,
                                IsEnabled = true,
                                DisplayOrder = 1
                            }
                        ]
                    };

                    return (new HeaderDictionary(), cancellations);
                }
            );

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<BathingTaxAgeChangeSettingFacilityCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        MockMediator = mockMediator.Object;

        var mockPersonAgeTypeService = new Mock<IPersonAgeTypeService>();

        mockPersonAgeTypeService.Setup(
                x => x.ArrangeOrderAsync(
                    It.IsAny<List<long>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        MockPersonAgeTypeService = mockPersonAgeTypeService.Object;
    }

    [Fact]
    public async Task CreateCancellation_ReturnsCorrectResult()
    {
        var personAgeType = new PersonAgeType
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new BathingTaxAgesEndpoint(
            mockMapper,
            MockMediator,
            MockPersonAgeTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new BathingTaxAgeCreateRequest(
            "Test 1",
            10,
            1,
            1,
            false,
            new MetaOfBathingTaxAgeUpdateRequest(
                "Test 1",
                FoodBeds.None,
                FoodBeds.None,
                PersonAgeGroups.Adult
            ),
            []
        );

        // Act
        var result = await controller.CreateBathingTaxAgeAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(personAgeType.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateCancellation_ReturnsCorrectResult()
    {
        var personAgeType = new PersonAgeType
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new BathingTaxAgesEndpoint(
            mockMapper,
            MockMediator,
            MockPersonAgeTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new BathingTaxAgeUpdateRequest(
            1,
            "Test 1",
            10,
            1,
            false,
            new MetaOfBathingTaxAgeUpdateRequest(
                "Test 1",
                FoodBeds.None,
                FoodBeds.None,
                PersonAgeGroups.Adult
            ),
            []
        );

        // Act
        var result = await controller.UpdateBathingTaxAgeAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(personAgeType.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ChangeStatusCancellation_ReturnsCorrectResult()
    {
        var personAgeType = new PersonAgeType
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new BathingTaxAgesEndpoint(
            mockMapper,
            MockMediator,
            MockPersonAgeTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new BathingTaxAgeChangeStatusRequest(
            1,
            true
        );

        // Act
        var result = await controller.ChangeStatusBathingTaxAgeAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(personAgeType.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ChangeFacilitySettingCancellation_ReturnsCorrectResult()
    {
        var personAgeType = new PersonAgeType
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new BathingTaxAgesEndpoint(
            mockMapper,
            MockMediator,
            MockPersonAgeTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new BathingTaxAgeChangeSettingFacilityRequest(
            true,
            "Comment",
            null
        );

        // Act
        var result = await controller.AdjustFacilitySettingAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(personAgeType.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAllCancellation_ReturnsCorrectResult()
    {
        var personAgeTypes = new List<PersonAgeType>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
            }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new BathingTaxAgesEndpoint(
            mockMapper,
            MockMediator,
            MockPersonAgeTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllBathingTaxAgesAsync(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<BathingTaxAgeResponse>(okResult.Value, false);
        var bathingTaxAges = response.BathingTaxAges.ToList();
        Assert.Equal(personAgeTypes.Count, response.BathingTaxAges.Count());
        for (var i = 0; i < personAgeTypes.Count; i++)
        {
            Assert.Equal(personAgeTypes[i].Id, bathingTaxAges[i].Id);
            Assert.Equal(personAgeTypes[i].Name!.GetValueByHeader(), bathingTaxAges[i].Name);
        }

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
