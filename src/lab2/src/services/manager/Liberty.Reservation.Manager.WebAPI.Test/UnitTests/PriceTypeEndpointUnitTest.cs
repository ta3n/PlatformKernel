using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PriceTypeEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceTypeCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceTypeUpdateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceTypeDeleteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceTypeGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PriceTypeGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new List<PriceTypeResponse>
                    {
                        new(1, "PriceType1", "PT1", "#F00", 1, true),
                        new(2, "PriceType2", "PT2", "#F00", 2, true)
                    };

                    return (new HeaderDictionary(), response);
                }
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<PriceTypeGetQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    PriceTypeGetQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new PriceTypeResponse(1, "PriceType1", "PT1", "#F00", 1, true);

                    return (new HeaderDictionary(), response);
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task CreatePriceType_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var mockAppDateTypeService = new Mock<IAppDateTypeService>().Object;
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PriceTypesEndpoint(
            mockMapper,
            MockMediator,
            mockAppDateTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PriceTypeCreateRequest("PT1", "PriceType1", "#F00");

        var result = await controller.CreatePriceType(
            request,
            cancellationToken
        );

        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<long>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(1, response);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePriceType_ReturnCorrectResult()
    {
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "PriceType1",
            ShortName = "PT1",
            Color = "#F00"
        };

        var mockMapper = MockServices.MockMapper();
        var mockAppDateTypeService = new Mock<IAppDateTypeService>().Object;
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PriceTypesEndpoint(
            mockMapper,
            MockMediator,
            mockAppDateTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new PriceTypeUpdateRequest("PT1", "PriceType1", "#F00");

        var result = await controller.UpdatePriceType(
            appDateType.Id,
            request,
            cancellationToken
        );

        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.PriceType.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(appDateType.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeletePriceType_ReturnCorrectResult()
    {
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "PriceType1",
            ShortName = "PT1",
            Color = "#F00"
        };

        var mockMapper = MockServices.MockMapper();
        var mockAppDateTypeService = new Mock<IAppDateTypeService>().Object;
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PriceTypesEndpoint(
            mockMapper,
            MockMediator,
            mockAppDateTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var result = await controller.DeletePriceType(
            appDateType.Id,
            cancellationToken
        );

        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.PriceType.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(appDateType.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllPriceTypes_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var mockAppDateTypeService = new Mock<IAppDateTypeService>().Object;
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PriceTypesEndpoint(
            mockMapper,
            MockMediator,
            mockAppDateTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var mockPageable = new Mock<IPageable>();

        var result = await controller.GetAllPriceTypes(
            mockPageable.Object,
            cancellationToken
        );

        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<IEnumerable<PriceTypeResponse>>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(2, response.Count());
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPriceType_ReturnCorrectResult()
    {
        var appDateType = new AppDateType
        {
            Id = 1,
            Name = "PriceType1",
            ShortName = "PT1",
            Color = "#F00"
        };

        var mockMapper = MockServices.MockMapper();
        var mockAppDateTypeService = new Mock<IAppDateTypeService>().Object;
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new PriceTypesEndpoint(
            mockMapper,
            MockMediator,
            mockAppDateTypeService,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var result = await controller.GetPriceType(
            1,
            cancellationToken
        );

        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PriceTypeResponse>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(appDateType.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
