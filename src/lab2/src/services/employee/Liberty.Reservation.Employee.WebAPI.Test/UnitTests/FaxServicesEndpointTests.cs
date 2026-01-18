using System.Net;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class FaxServicesEndpointTests : BaseUnitTest
{
    private IFaxSrvService MockFaxSrvService { get; set; } = null!;

    protected override void InitData()
    {
        var mockFaxService = new Mock<IFaxSrvService>();

        mockFaxService.Setup(
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
                    var faxes = new List<FaxService>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Fax1",
                            IsMailFax = true,
                            MailFormat = "{0}@xxxx.xxx",
                            UnitPrice = 10.0f
                        },
                        new()
                        {
                            Id = 2,
                            Name = "Fax2",
                            IsMailFax = false,
                            MailFormat = "{0}@yyyy.yyy",
                            UnitPrice = 15.0f
                        }
                    };
                    var mockPage = new Mock<IPage<FaxService>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)faxes.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < faxes.Count);
                    mockPage.Setup(p => p.Content).Returns(faxes);

                    return mockPage.Object;
                }
            );

        MockFaxSrvService = mockFaxService.Object;
    }

    [Fact]
    public async Task GetAllFaxServices_ReturnsCorrectResult()
    {
        // Arrange
        var faxServices = new List<FaxService>
        {
            new()
            {
                Id = 1,
                Name = "Fax Service 1",
                IsMailFax = true,
                UnitPrice = 100.0f
            },
            new()
            {
                Id = 2,
                Name = "Fax Service 2",
                IsMailFax = false,
                UnitPrice = 150.0f
            }
        };

        var response = new List<FaxServiceResponse>
        {
            new(1, "Code1", "Fax Service 1", true),
            new(2, "Code2", "Fax Service 2", false)
        };
        var mockMapper = MockServices.MockMapper((faxServices, response));
        var mockPageable = new Mock<IPageable>();
        var controller = new FaxServicesEndpoint(mockMapper, MockMediator, MockFaxSrvService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAllFaxServices(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var actualResponse = Assert.IsType<List<FaxServiceResponse>>(okResult.Value, false);
        Assert.Equal(2, actualResponse.Count);

        // Kiểm tra các giá trị đúng
        Assert.Equal(response[0].Id, actualResponse[0].Id);
        Assert.Equal(response[0].Name, actualResponse[0].Name);

        Assert.Equal(response[1].Id, actualResponse[1].Id);
        Assert.Equal(response[1].Name, actualResponse[1].Name);

        // Kiểm tra headers
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
