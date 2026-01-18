using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MasterCalendar;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class MasterCalendarEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(m => m.Send(It.IsAny<MasterCalendarUpdateDataCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateDataOfMasterCalendarResponse(20250101, "Test Name"));

        mockMediator.Setup(m => m.Send(It.IsAny<MasterCalendarUpdateTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DateTypeOfMasterCalendarResponse(
                    20250101,
                    "TYPE01",
                    "Test Type",
                    "TST",
                    "Red"
                )
            );

        mockMediator.Setup(m => m.Send(It.IsAny<MasterCalendarDeleteDataCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(20250101);

        mockMediator.Setup(m => m.Send(It.IsAny<MasterCalendarDeleteTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(20250101);

        mockMediator.Setup(m => m.Send(It.IsAny<MasterCalendarGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (new HeaderDictionary(), new List<DateOfMasterCalendarResponse>
                {
                    new(
                        20240101,
                        "Holiday",
                        "HLD",
                        "Red",
                        "New Year's Day"
                    ),
                    new(
                        20240125,
                        "Workday",
                        "WRK",
                        "Green",
                        "Workday"
                    )
                })
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task CreateDataOfDate_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new MasterCalendarEndpoint(MockMapper, MockMediator);
        var request = new MasterCalendarEditDataRequest(20250101, "Name");

        // Act
        var result = await controller.CreateDataOfDate(request, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateAppDateData.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal("20250101", headers["X-Liberty-params"]);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateTypeOfDate_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new MasterCalendarEndpoint(MockMapper, MockMediator);
        var request = new MasterCalendarEditTypeRequest(20250101, 1);

        // Act
        var result = await controller.CreateTypeOfDate(request, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateAppDateType.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task DeleteDataOfDate_ReturnsNoContentWithHeaders()
    {
        // Arrange
        var controller = new MasterCalendarEndpoint(MockMapper, MockMediator);
        var appDate = 20250101;

        // Act
        var result = await controller.DeleteDataOfDate(appDate, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateAppDateData.Deleted", headers["X-Liberty-alert"]);
        Assert.Equal("20250101", headers["X-Liberty-params"]);
        Assert.Equal(StatusCodes.Status204NoContent, okResult.StatusCode);
    }

    [Fact]
    public async Task DeleteTypeOfDate_ReturnsNoContentWithHeaders()
    {
        // Arrange
        var controller = new MasterCalendarEndpoint(MockMapper, MockMediator);
        var appDate = 20250101;

        // Act
        var result = await controller.DeleteTypeOfDate(appDate, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.AppDateAppDateType.Deleted", headers["X-Liberty-alert"]);
        Assert.Equal("20250101", headers["X-Liberty-params"]);
        Assert.Equal(StatusCodes.Status204NoContent, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAllDates_ReturnsOkResult()
    {
        // Arrange
        var controller = new MasterCalendarEndpoint(MockMapper, MockMediator);

        var pageable = PageableBinderConfig.DefaultPageable;
        const int startDate = 20230101;
        const int endDate = 20231231;

        // Act
        var result = await controller.GetAllDates(pageable, startDate, endDate, CancellationToken.None);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
