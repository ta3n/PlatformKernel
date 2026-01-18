using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class FaxServicesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/fax-service-categories";

    [Fact]
    public async Task GetAllFaxService_ReturnOK_WithFaxServices()
    {
        var faxServiceRepo = Factory.GetRequiredService<IFaxServiceRepository>();
        var faxService = new FaxService
        {
            Code = EntityUtil.CreateCode(),
            Name = "Name"
        };
        _ = await faxServiceRepo!.AddAsync(faxService, true);

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
