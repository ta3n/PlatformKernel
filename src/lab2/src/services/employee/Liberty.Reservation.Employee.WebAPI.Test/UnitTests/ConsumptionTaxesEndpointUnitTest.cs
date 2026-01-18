using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class ConsumptionTaxEndpointUnitTest : BaseUnitTest
{
    private IConsumptionTaxService MockConsumptionTaxService { get; set; } = null!;

    protected override void InitData()
    {
        var mockConsumptionTaxService = new Mock<IConsumptionTaxService>();

        mockConsumptionTaxService.Setup(
                x => x.CreateAsync(
                    It.IsAny<ConsumptionTax>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ConsumptionTax
                {
                    Id = 1,
                    Name = "Standard Tax",
                    Rate = 10.0f,
                    EnabledStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    EnabledEnd = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds()
                }
            );

        mockConsumptionTaxService.Setup(
                x => x.UpdateAsync(
                    It.IsAny<ConsumptionTax>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<ConsumptionTax, ConsumptionTax, ConsumptionTax>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ConsumptionTax
                {
                    Id = 1,
                    Name = "Updated Tax",
                    Rate = 12.0f,
                    EnabledStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    EnabledEnd = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds()
                }
            );

        mockConsumptionTaxService.Setup(
                x => x.DeleteAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ConsumptionTax
                {
                    Id = 1,
                    Name = "Deleted Tax",
                    Rate = 0.0f,
                    EnabledStart = 0,
                    EnabledEnd = 0
                }
            );

        mockConsumptionTaxService.Setup(
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
                    var taxes = new List<ConsumptionTax>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Standard Tax",
                            Rate = 10.0f,
                            EnabledStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            EnabledEnd = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds()
                        },
                        new()
                        {
                            Id = 2,
                            Name = "Reduced Tax",
                            Rate = 5.0f,
                            EnabledStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            EnabledEnd = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds()
                        }
                    };

                    var mockPage = new Mock<IPage<ConsumptionTax>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)taxes.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < taxes.Count);
                    mockPage.Setup(p => p.Content).Returns(taxes);

                    return mockPage.Object;
                }
            );

        mockConsumptionTaxService.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ConsumptionTax
                {
                    Id = 1,
                    Name = "Standard Tax",
                    Rate = 10.0f,
                    EnabledStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    EnabledEnd = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds()
                }
            );

        MockConsumptionTaxService = mockConsumptionTaxService.Object;
    }

    [Fact]
    public async Task GetConsumptionTax_ReturnOkObjectResult()
    {
        // Arrange
        var tax = new ConsumptionTax
        {
            Id = 1,
            Rate = 4,
            EnabledStart = 20200930,
            EnabledEnd = 20200930
        };
        var taxResponse = new ConsumptionTaxResponse(1, "Code", "Name", 4, 20200930, 20200930);
        var mockMapper = MockServices.MockMapper((tax, taxResponse));
        var controller = new ConsumptionTaxesEndpoint(mockMapper, MockMediator, MockConsumptionTaxService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetConsumptionTax(tax.Id, cancellationToken);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetConsumptionTax_ReturnNotFoundResult()
    {
        // Arrange
        var mockMapper = MockServices.MockMapper();
        var controller = new ConsumptionTaxesEndpoint(mockMapper, MockMediator, MockConsumptionTaxService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetConsumptionTax(999, cancellationToken);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetAllConsumptionTaxes_ReturnsCorrectResult()
    {
        // Arrange
        var consumptionTaxes = new List<ConsumptionTax>
        {
            new()
            {
                Id = 1,
                Name = "Tax 1",
                Rate = 10.0f,
                EnabledStart = 1234567890,
                EnabledEnd = 2345678901
            },
            new()
            {
                Id = 2,
                Name = "Tax 2",
                Rate = 5.0f,
                EnabledStart = 2234567890,
                EnabledEnd = 3345678901
            }
        };

        var response = new List<ConsumptionTaxResponse>
        {
            new(1, "CODE1", "Tax 1", 10.0f, 1234567890, 2345678901),
            new(2, "CODE2", "Tax 2", 5.0f, 2234567890, 3345678901)
        };
        var mockMapper = MockServices.MockMapper((consumptionTaxes, response));
        var mockPageable = new Mock<IPageable>();
        var controller = new ConsumptionTaxesEndpoint(mockMapper, MockMediator, MockConsumptionTaxService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAllConsumptionTaxes(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var actualResponse = Assert.IsType<List<ConsumptionTaxResponse>>(okResult.Value, false);
        Assert.Equal(2, actualResponse.Count);

        Assert.Equal(response[0].Id, actualResponse[0].Id);
        Assert.Equal(response[0].Name, actualResponse[0].Name);
        Assert.Equal(response[0].Rate, actualResponse[0].Rate);

        Assert.Equal(response[1].Id, actualResponse[1].Id);
        Assert.Equal(response[1].Name, actualResponse[1].Name);
        Assert.Equal(response[1].Rate, actualResponse[1].Rate);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderOfConsumptionTaxes_ReturnsCorrectResult()
    {
        // Arrange
        var request = new ItemUpdateOrderRequest(
            [
                1,
                2,
                3
            ]
        );
        var mockMapper = MockServices.MockMapper();
        var controller = new ConsumptionTaxesEndpoint(mockMapper, MockMediator, MockConsumptionTaxService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ArrangeOrderOfConsumptionTaxes(request, cancellationToken);
        _ = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }
}
