using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class MealTypeEndpointUnitTest : BaseUnitTest
{
    private IMealTypeService MockMealTypeService { get; set; } = null!;

    protected override void InitData()
    {
        var mockMealTypeService = new Mock<IMealTypeService>();

        mockMealTypeService
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
                    var mealTypes = new List<MealType>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Meal type 1"
                        },
                        new()
                        {
                            Id = 2,
                            Name = "Meal type 2"
                        }
                    };

                    var mockPage = new Mock<IPage<MealType>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)mealTypes.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < mealTypes.Count);
                    mockPage.Setup(p => p.Content).Returns(mealTypes);

                    return mockPage.Object;
                }
            );

        MockMealTypeService = mockMealTypeService.Object;
    }

    [Fact]
    public async Task GetAllMealTypes_ReturnsCorrectResult()
    {
        // Arrange
        var mealTypes = new List<MealType>
        {
            new()
            {
                Id = 1,
                Name = "Meal type 1"
            },
            new()
            {
                Id = 2,
                Name = "Meal type 2"
            }
        };
        var mockMapper = MockServices.MockMapper(
            (mealTypes, new List<MealTypeResponse>
            {
                new(
                    1,
                    "Meal type 1"
                ),
                new(
                    2,
                    "Meal type 2"
                )
            })
        );
        var controller = new MealTypesEndpoint(mockMapper, MockMealTypeService);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();
        // Act
        var result = await controller.GetAllMealTypes(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<MealTypeResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("Meal type 1", response[0].Name);
        Assert.Equal("Meal type 2", response[1].Name);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
