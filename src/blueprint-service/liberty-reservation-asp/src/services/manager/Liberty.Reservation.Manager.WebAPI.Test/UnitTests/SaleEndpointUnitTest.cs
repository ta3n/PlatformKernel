using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class SaleEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<SaleGetAllReservationsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    SaleGetAllReservationsQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new List<SaleDetailResponse>
                    {
                        new()
                        {
                            Id = 1,
                            Code = "SALE123",
                            State = "Confirmed",
                            IsReserved = true,
                            BookingDateTime = DateTime.Now,
                            CheckInDate = AppDate.GetId(DateTime.UtcNow),
                            CheckOutDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                            NumberOfNights = 4,
                            NumberOfPeople = 2,
                            ReserverName = "John Doe",
                            PaymentType = "Credit Card",
                            TotalPrice = 500.00m
                        },
                        new()
                        {
                            Id = 2,
                            Code = "SALE123",
                            State = "Confirmed",
                            IsReserved = true,
                            BookingDateTime = DateTime.Now,
                            CheckInDate = AppDate.GetId(DateTime.UtcNow),
                            CheckOutDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                            NumberOfNights = 4,
                            NumberOfPeople = 2,
                            ReserverName = "John Doe",
                            PaymentType = "Credit Card",
                            TotalPrice = 500.00m
                        }
                    };

                    return (new HeaderDictionary(), response);
                }
            );

        mockMediator
            .Setup(
                x => x.Send(
                    It.IsAny<SaleGetReservationOverviewQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    SaleGetReservationOverviewQuery _,
                    CancellationToken _
                ) =>
                {
                    var response = new SaleOverviewResponse(
                        50,
                        30,
                        10,
                        5,
                        10000.00m,
                        5000.00m,
                        15000.00m,
                        200.00m,
                        100.00m,
                        300.00m
                    );

                    return (new HeaderDictionary(), response);
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task GetAllSaleDetails_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new SalesEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllSaleDetails(mockPageable.Object, 20250101, 20250202, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<IEnumerable<SaleDetailResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(2, response.Count());
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetSaleOverview_ReturnCorrectResult()
    {
        var mockMapper = MockServices.MockMapper();
        var controller = new SalesEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetSalesOverview(20250101, 20250202, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<SaleOverviewResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }
}
