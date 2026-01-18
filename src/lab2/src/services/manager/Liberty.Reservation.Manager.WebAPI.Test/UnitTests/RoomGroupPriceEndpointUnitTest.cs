using System.Net;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class RoomGroupPriceEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceCreateSiteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceUpdateStandardPriceCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceUpdateChildrenPriceCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceUpdateSaleCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceUpdatePriceCalendarCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceUpdateDiscountCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceGetStandardPriceQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPriceGetStandardPriceQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new PlanRoomDetailStandardPriceResponse(
                        1,
                        1,
                        1,
                        1,
                        10,
                        []
                    );

                    return (new HeaderDictionary(), response);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceGetChildrenPriceQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPriceGetChildrenPriceQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new PlanRoomDetailChildrenPriceResponse(
                        1,
                        1,
                        1,
                        [
                            new RoomTypeChildrenPersonAgeTypeResponse
                            {
                                PersonAgeTypeId = 1,
                                Name = "Test 1",
                                IsEnabled = true,
                                IsRegardAdult = true,
                                PriceSettingType = Reservation.Application.Constants.PriceSettingTypes.Percent,
                                Value = 10
                            }
                        ]
                    );

                    return (new HeaderDictionary(), response);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceGetSaleQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPriceGetSaleQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new PlanRoomDetailSaleResponse
                    {
                        PlanId = 1,
                        SiteId = 1,
                        RomTypeId = 1,
                        AutoExtendEveryMonthDay = 1,
                        AutoExtendMonth = 1,
                        UseAutoExtend = true
                    };

                    return (new HeaderDictionary(), response);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceGetPriceCalendarQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPriceGetPriceCalendarQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new PlanRoomDetailPriceCalendarResponse(
                        1,
                        1,
                        1,
                        [
                            new RoomTypePriceDataCalendarResponse
                            {
                                DateCalendar = 20250101,
                                PersonMin = 1,
                                PersonMax = 10,
                                Price = 100,
                                UseAutoDiscount = true
                            }
                        ]
                    );

                    return (new HeaderDictionary(), response);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPriceGetDiscountQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPriceGetDiscountQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new PlanRoomDetailDiscountResponse(
                        1,
                        1,
                        1,
                        [
                            new RoomTypeDiscountDataResponse
                            {
                                PriceSettingType = Reservation.Application.Constants.PriceSettingTypes.None,
                                PersonMax = 10,
                                PersonMin = 1,
                                StartPrevDay = 1,
                                EndPrevDay = 10,
                                Value = 10
                            }
                        ]
                    );

                    return (new HeaderDictionary(), response);
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task CreateRoomTypeSite_ReturnCorrectResult()
    {
        var planForRoom = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        // Act
        var result = await controller.CreateRoomTypeSiteAsync(1, 1, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(planForRoom.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomTypeStandardPrice_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        var request = new RoomGroupPriceUpdateStandardPriceRequest(
            [
                new RomTypeUpdateStandardRequest(
                    1,
                    1,
                    2,
                    100
                ),
                new RomTypeUpdateStandardRequest(
                    2,
                    3,
                    4,
                    200
                )
            ]
        );

        // Act
        var result = await controller.UpdateStandardPriceOfRoomGroupPrice(1, 1, request, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomTypeChildrenPrice_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        var request = new RoomGroupPriceUpdateChildrenPriceRequest(
            [
                new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                    1,
                    true,
                    true,
                    Reservation.Application.Constants.PriceSettingTypes.Percent,
                    10
                ),
                new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                    2,
                    true,
                    true,
                    Reservation.Application.Constants.PriceSettingTypes.Percent,
                    10
                )
            ]
        );

        // Act
        var result = await controller.UpdateChildrenPriceOfRoomGroupPrice(1, 1, request, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomTypeSale_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        var request = new RoomGroupPriceUpdateSaleRequest
        {
            AutoExtendEveryMonthDay = 1,
            AutoExtendMonth = 1,
            UseAutoExtend = true
        };

        // Act
        var result = await controller.UpdateSaleOfRoomGroupPrice(1, 1, request, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomTypePriceCalendar_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        var request = new RoomGroupPriceUpdatePriceCalendarRequest(
            [
                new RomTypeUpdatePriceDataCalendarRequest(
                    20250101,
                    1,
                    10,
                    100,
                    true
                ),
                new RomTypeUpdatePriceDataCalendarRequest(
                    20250102,
                    1,
                    10,
                    100,
                    true
                )
            ]
        );

        // Act
        var result = await controller.UpdatePriceCalendarOfRoomGroupPrice(1, 1, request, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomTypeDiscount_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        var request = new RoomGroupPriceUpdateDiscountRequest(
            [
                new RomTypeUpdateDiscountDataRequest(
                    1,
                    10,
                    1,
                    10,
                    Reservation.Application.Constants.PriceSettingTypes.None,
                    100
                ),
                new RomTypeUpdateDiscountDataRequest(
                    1,
                    10,
                    1,
                    10,
                    Reservation.Application.Constants.PriceSettingTypes.None,
                    100
                )
            ]
        );

        // Act
        var result = await controller.UpdateDiscountOfRoomGroupPrice(1, 1, request, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetStandardPrice_ReturnCorrectResult()
    {
        var planForRoom = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        // Act
        var result = await controller.GetStandardPriceOfRoomGroupPrice(1, 1, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanRoomDetailStandardPriceResponse>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(planForRoom.Id, response.PlanId);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetChildrenPrice_ReturnCorrectResult()
    {
        var planForRoom = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        // Act
        var result = await controller.GetChildrenPriceOfRoomGroupPrice(1, 1, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanRoomDetailChildrenPriceResponse>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(planForRoom.Id, response.PlanId);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetSale_ReturnCorrectResult()
    {
        var planForRoom = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        // Act
        var result = await controller.GetSaleOfRoomGroupPrice(1, 1, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanRoomDetailSaleResponse>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(planForRoom.Id, response.PlanId);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPriceCalendar_ReturnCorrectResult()
    {
        var planForRoom = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        // Act
        var result = await controller.GetPriceCalendarOfRoomGroupPrice(1, 1, 20250101, 20250202, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanRoomDetailPriceCalendarResponse>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(planForRoom.Id, response.PlanId);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetDiscount_ReturnCorrectResult()
    {
        var planForRoom = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var controller = new RoomGroupPricesEndpoint(mockMapper, MockMediator);
        var planToken = CancellationToken.None;

        // Act
        var result = await controller.GetDiscountOfRoomGroupPrice(1, 1, planToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<PlanRoomDetailDiscountResponse>(okObjectResult.Value, false);

        Assert.NotNull(headers);
        Assert.Equal(planForRoom.Id, response.PlanId);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
