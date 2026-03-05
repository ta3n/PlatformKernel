using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Allergen;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class AllergensEndpointUnitTest : BaseUnitTest
{
    private IAllergenService MockAllergenService { get; set; } = null!;

    protected override void InitData()
    {
        var mockAllergenService = new Mock<IAllergenService>();

        mockAllergenService.Setup(
                x => x.CreateAsync(
                    It.IsAny<Allergen>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Allergen
                {
                    Id = 1,
                    Name = "Allergen1"
                }
            );

        mockAllergenService.Setup(
                x => x.UpdateAsync(
                    It.IsAny<Allergen>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Allergen, Allergen, Allergen>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Allergen
                {
                    Id = 1,
                    Name = "UpdatedAllergen"
                }
            );

        mockAllergenService.Setup(
                x => x.DeleteAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Allergen
                {
                    Id = 1,
                    Name = "DeletedAllergen"
                }
            );

        mockAllergenService.Setup(
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
                    var Allergens = new List<Allergen>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Allergen1"
                        },
                        new()
                        {
                            Id = 2,
                            Name = "Allergen2"
                        }
                    };

                    var mockPage = new Mock<IPage<Allergen>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)Allergens.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < Allergens.Count);
                    mockPage.Setup(p => p.Content).Returns(Allergens);

                    return mockPage.Object;
                }
            );

        mockAllergenService.Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Allergen
                {
                    Id = 1,
                    Name = "Allergen1"
                }
            );

        mockAllergenService.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Allergen
                {
                    Id = 1,
                    Name = "Allergen1"
                }
            );

        MockAllergenService = mockAllergenService.Object;
    }

    [Fact]
    public async Task GetAllAllergens_ReturnsCorrectResult()
    {
        // Arrange
        var Allergens = new List<Allergen>
        {
            new()
            {
                Id = 1,
                Name = "Allergen1"
            },
            new()
            {
                Id = 2,
                Name = "Allergen2"
            }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<AllergenGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    AllergenGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var allergens = new List<AllergenResponse>
                    {
                        new(
                            1,
                            "Allergen 1",
                            true
                        ),
                        new(
                            2,
                            "Allergen 2",
                            false
                        )
                    };

                    return (new HeaderDictionary(), allergens);
                }
            );

        var mockMapper = MockServices.MockMapper(
            (Allergens, new List<AllergenResponse>
            {
                new(1, "Allergen 1", true),
                new(2, "Allergen 2", true)
            })
        );
        var controller = new AllergensEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllAllergens(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<AllergenResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("Allergen 1", response[0].Name);
        Assert.Equal("Allergen 2", response[1].Name);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
