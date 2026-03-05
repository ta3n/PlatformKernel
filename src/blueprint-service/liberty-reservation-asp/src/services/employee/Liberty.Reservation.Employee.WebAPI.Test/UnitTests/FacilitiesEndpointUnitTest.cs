using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class FacilitiesEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new FacilityResponse(
                    123,
                    "F001",
                    1,
                    true,
                    "Facility Update Memo"
                )
            );

        mockMediator.Setup(mediator => mediator.Send(It.IsAny<FacilityGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var facilities = new List<FacilityResponse>
                    {
                        new(
                            1,
                            "F001",
                            1,
                            true,
                            "Facility 1"
                        )
                        {
                            Name = "Facility A",
                            Kana = "A",
                            PostCode = "12345"
                        },
                        new(
                            2,
                            "F002",
                            2,
                            false,
                            "Facility 2"
                        )
                        {
                            Name = "Facility B",
                            Kana = "B",
                            PostCode = "67890"
                        }
                    };

                    return (new HeaderDictionary(), facilities);
                }
            );

        mockMediator.Setup(mediator => mediator.Send(It.IsAny<FacilityGetQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetQuery _,
                    CancellationToken _
                ) =>
                {
                    var facilityResponse = new FacilityDetailResponse(
                        1,
                        "F001",
                        "Facility A",
                        "A",
                        "12345",
                        "123 Street",
                        "City Name",
                        "State Name",
                        "Country Name",
                        "123-456-789",
                        "facility@example.com",
                        "Facility Memo",
                        "Record Code",
                        true,
                        null,
                        null
                    );

                    return (new HeaderDictionary(), facilityResponse);
                }
            );
        mockMediator.Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateFaxServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new FacilityResponse(
                    123,
                    "F001",
                    1,
                    true,
                    "Fax services updated"
                )
            );

        mockMediator
            .Setup(
                mediator => mediator.Send(
                    It.IsAny<FacilityUpdateFaxServiceCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    FacilityUpdateFaxServiceCommand _,
                    CancellationToken _
                ) => new FacilityResponse(
                    123,
                    "F001",
                    1,
                    true,
                    "Updated Memo"
                )
            );

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetAllDestinationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetAllDestinationsQuery query,
                    CancellationToken _
                ) =>
                {
                    if (query.FacilityId != 1)
                    {
                        return (new HeaderDictionary(), null!);
                    }

                    var destinationResponses = new List<DestinationResponse>
                    {
                        new()
                        {
                            Id = 1,
                            Code = "DEST001",
                            Name = "Destination A",
                            ShortName = "Dest A",
                            Url = "https://example.com/destination-a",
                            DisplayOrder = 1,
                            IsEnabled = true
                        },
                        new()
                        {
                            Id = 2,
                            Code = "DEST002",
                            Name = "Destination B",
                            ShortName = "Dest B",
                            Url = "https://example.com/destination-b",
                            DisplayOrder = 2,
                            IsEnabled = true
                        }
                    };

                    return (new HeaderDictionary(), destinationResponses);
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task UpdateFacility_ReturnsNoContentResultWithHeaders()
    {
        // Arrange
        var request = new FacilityUpdateRequest([1, 2, 3], true, "Facility 1", "Description 1");
        var controller = new FacilitiesEndpoint(MockMapper, MockMediator);

        // Act
        var result = await controller.UpdateFacility(1, request, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnableFacility_ReturnsNoContentResultWithHeaders()
    {
        // Arrange
        var request = new FacilityEnabledRequest(true);
        var controller = new FacilitiesEndpoint(MockMapper, MockMediator);

        // Act
        var result = await controller.EnableFacility(1, request, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllFacilities_ReturnsOkResult()
    {
        // Arrange
        var mockPageable = new Mock<IPageable>();
        var controller = new FacilitiesEndpoint(MockMapper, MockMediator);

        // Act
        var result = await controller.GetAllFacilities(mockPageable.Object, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        _ = Assert.IsType<IEnumerable<FacilityResponse>>(okResult.Value, false);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetFacility_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new FacilitiesEndpoint(MockMapper, MockMediator);

        // Act
        var result = await controller.GetFacility(1, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        _ = Assert.IsType<FacilityDetailResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAllDestinationsOfFacility_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var mockPageable = new Mock<IPageable>();
        var controller = new FacilitiesEndpoint(MockMapper, MockMediator);

        // Act
        var result = await controller.GetAllDestinationsOfFacility(1, mockPageable.Object, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        _ = Assert.IsType<List<DestinationResponse>>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
