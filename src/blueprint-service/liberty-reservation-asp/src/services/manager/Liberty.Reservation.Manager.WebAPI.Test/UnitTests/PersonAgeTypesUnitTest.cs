using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PersonAgeTypesUnitTest : BaseUnitTest
{
    private IPersonAgeTypeService MockPersonAgeTypeService { get; set; } = null!;

    protected override void InitData()
    {
        var mockPeronAgeTypeService = new Mock<IPersonAgeTypeService>();

        mockPeronAgeTypeService
            .Setup(
                x => x.GetAllPersonAgeTypes(
                    It.IsAny<IPageable>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    IPageable pageable,
                    bool _,
                    CancellationToken _
                ) =>
                {
                    var personAgeTypes = new List<PersonAgeType>
                    {
                        new()
                        {
                            Id = 1,
                            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
                        },
                        new()
                        {
                            Id = 2,
                            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 2" } }
                        }
                    };

                    var mockPage = new Mock<IPage<PersonAgeType>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)personAgeTypes.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < personAgeTypes.Count);
                    mockPage.Setup(p => p.Content).Returns(personAgeTypes);

                    return mockPage.Object;
                }
            );

        MockPersonAgeTypeService = mockPeronAgeTypeService.Object;
    }

    [Fact]
    public async Task GetAllPersonAgeTypes_ReturnsCorrectResult()
    {
        // Arrange
        var personAgeTypes = new List<PersonAgeType>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 2" } }
            }
        };
        var mockMapper = MockServices.MockMapper(
            (personAgeTypes, new List<PersonAgeTypeResponse>
            {
                new(
                    1,
                    "Person age type 1",
                    true,
                    0,
                    18
                ),
                new(
                    2,
                    "Person age type 2",
                    true,
                    19,
                    30
                )
            })
        );
        var controller = new PersonAgeTypesEndpoint(mockMapper, MockMediator, MockPersonAgeTypeService);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();
        // Act
        var result = await controller.GetAllPersonAgeTypes(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<PersonAgeTypeResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("Person age type 1", response[0].Name);
        Assert.Equal("Person age type 2", response[1].Name);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
