using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class BedTypeEndpointUnitTest : BaseUnitTest
{
    private IBedTypeService MockBedTypeService { get; set; } = null!;

    protected override void InitData()
    {
        var bedTypeService = new Mock<IBedTypeService>();

        bedTypeService
            .Setup(
                x => x.FindAllAsync(
                    It.IsAny<IPageable>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    IPageable pageable,
                    CancellationToken _
                ) =>
                {
                    var bedTypes = new List<BedType>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "BedType 1",
                            BedTypeUnitType = BedTypeUnitTypes.Pair
                        },
                        new()
                        {
                            Id = 2,
                            Name = "BedType 2",
                            BedTypeUnitType = BedTypeUnitTypes.Pair
                        }
                    };

                    var mockPage = new Mock<IPage<BedType>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)bedTypes.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < bedTypes.Count);
                    mockPage.Setup(p => p.Content).Returns(bedTypes);

                    return mockPage.Object;
                }
            );

        MockBedTypeService = bedTypeService.Object;
    }

    [Fact]
    public async Task GetAllBedTypes_ReturnsCorrectResult()
    {
        // Arrange
        var bedTypes = new List<BedType>
        {
            new()
            {
                Id = 1,
                Name = "BedType 1"
            },
            new()
            {
                Id = 2,
                Name = "BedType 2"
            }
        };

        var mockMapper = MockServices.MockMapper(
            (
                bedTypes,
                new List<BedTypeResponse>
                {
                    new(1, "Code 1", "BedType 1", BedTypeUnitTypes.Pair, true),
                    new(2, "Code 2", "BedType 2", BedTypeUnitTypes.Pair, true)
                }
            )
        );
        var controller = new BedTypesEndpoint(mockMapper, MockBedTypeService);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllBedTypes(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<BedTypeResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("BedType 1", response[0].Name);
        Assert.Equal("BedType 2", response[1].Name);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
