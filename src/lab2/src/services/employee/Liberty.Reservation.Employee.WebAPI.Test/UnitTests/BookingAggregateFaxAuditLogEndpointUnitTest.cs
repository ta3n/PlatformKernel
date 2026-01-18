using AutoMapper;
using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingAggregateFaxAuditLog;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class BookingAggregateFaxAuditLogEndpointUnitTest : BaseUnitTest
{
    [Fact]
    public async Task GetBookingAggregateFaxAuditLogs_Success()
    {
        var mediatorMock = new Mock<IMediator>();
        var mapperMock = new Mock<IMapper>();
        var pageableMock = new Mock<IPageable>();
        var cancellationToken = CancellationToken.None;

        var bookingAggregateFaxAuditLogs = new List<BookingAggregateFaxAuditLogSearchResponse>
        {
            new(
                1,
                "faxNumber_123",
                "test subject",
                "test result",
                true,
                "",
                ""
            ) { CreatedAtId = 20250101 }
        };

        mediatorMock
            .Setup(
                x =>
                    x.Send(It.IsAny<SearchBookingAggregateFaxAuditLogQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((new HeaderDictionary(), bookingAggregateFaxAuditLogs));

        var controller = new BookingAggregateFaxAuditLogEndpoint(
            mapperMock.Object,
            mediatorMock.Object
        );

        var result = await controller.SearchBookingAggregateFaxAuditLog(
            pageableMock.Object,
            cancellationToken
        );

        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.IsType<List<BookingAggregateFaxAuditLogSearchResponse>>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        Assert.NotNull(headers);
    }
}
