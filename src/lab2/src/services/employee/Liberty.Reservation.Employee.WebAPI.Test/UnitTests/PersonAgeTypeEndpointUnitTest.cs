using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.PersonAgeType;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static Liberty.Reservation.Employee.WebAPI.Application.Models.Responses.PersonAgeTypeResponse;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class PersonAgeTypeEndpointUnitTest : BaseUnitTest
{
    [Fact]
    public async Task GetAllPersonAgeType_ShouldReturnListPersonAgeType()
    {
        var pageMock = new Mock<IPageable>();
        var mediatorMock = new Mock<IMediator>();

        var dataMock = new List<PersonAgeTypeResponse>
        {
            new()
            {
                Id = 1,
                Name = "Child",
                AgeMin = 0,
                AgeMax = 12,
                IsMain = true,
                IsEnabled = true,
                IsVisible = true,
                Spas =
                [
                    new SpaOfPersonAgeTypeResponse
                    {
                        Id = 1,
                        PriceMin = 0,
                        PriceMax = 9999,
                        Tax = 10
                    }
                ],
                Meta = new MetaOfPersonAgeTypeResponse
                {
                    GroupName = "ChildGroup",
                    Bed = FoodBeds.Bed,
                    Food = FoodBeds.Food,
                    PersonAgeGroup = (long)PersonAgeGroups.Child
                }
            }
        };

        mediatorMock.Setup(x => x.Send(It.IsAny<PersonAgeTypeGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), dataMock));

        var controller = new PersonAgeTypeEndpoint(
            MockMapper,
            mediatorMock.Object
        );

        var result = await controller.GetListPersonAgeType(pageMock.Object, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CreatePersonAgeType_ShouldReturnCreated_WhenRequestValid()
    {
        // Arrange
        long testId = 4;
        var mediatorMock = new Mock<IMediator>();
        var controller = new PersonAgeTypeEndpoint(
            MockMapper,
            mediatorMock.Object
        );
        mediatorMock
            .Setup(m => m.Send(It.IsAny<PersonAgeTypeCreateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testId);

        var createRequest = new PersonAgeTypeCreateRequest(
            "Test",
            0,
            12,
            true,
            new MetaOfBathingTaxAgeUpdateRequest("Group", FoodBeds.Food, FoodBeds.Bed, 8),
            new List<SpaOfBathingTaxAgeUpdateRequest>()
        );

        // Act
        var result = await controller.CreatePersonAgeType(createRequest, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(testId, okResult.Value);
    }

    [Fact]
    public async Task UpdatePersonAgeType_ShouldReturnOk_WhenMediatorReturnsId()
    {
        // Arrange
        long testId = 1;
        var mediatorMock = new Mock<IMediator>();
        var controller = new PersonAgeTypeEndpoint(
            MockMapper,
            mediatorMock.Object
        );
        mediatorMock
            .Setup(m => m.Send(It.IsAny<PersonAgeTypeUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testId);

        var updateRequest = new PersonAgeTypeUpdateRequest(
            "Test",
            0,
            12,
            true,
            new MetaOfBathingTaxAgeUpdateRequest("Group", FoodBeds.Food, FoodBeds.Bed, 8),
            new List<SpaOfBathingTaxAgeUpdateRequest> { new(0, 9999, 10) }
        ) { Id = testId };

        // Act
        var result = await controller.UpdatePersonAgeType(updateRequest, testId, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(testId, okResult.Value);
    }

    [Fact]
    public async Task DeletePersonAgeType_ShouldReturnNoContent_WhenMediatorReturnsId()
    {
        // Arrange
        long testId = 1;
        var mediatorMock = new Mock<IMediator>();
        var controller = new PersonAgeTypeEndpoint(
            MockMapper,
            mediatorMock.Object
        );

        mediatorMock
            .Setup(m => m.Send(It.IsAny<PersonAgeTypeDeleteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testId);

        // Act
        var result = await controller.DeletePersonAgeType(testId, CancellationToken.None);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(204, noContentResult.StatusCode);
    }
}
