using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class ConsumptionTaxesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/consumption-taxes";

    private async Task<ConsumptionTax> CreateConsumptionTaxAsync()
    {
        var consumptionTaxRepo = Factory.GetRequiredService<IConsumptionTaxRepository>();

        var consumptionTax = new ConsumptionTax
        {
            Code = EntityUtil.CreateCode(),
            Name = "Name",
            Rate = 100,
            EnabledStart = 1,
            EnabledEnd = 2
        };

        var newConsumptionTax = await consumptionTaxRepo!.AddAsync(consumptionTax, true);

        return newConsumptionTax;
    }

    [Fact]
    public async Task OrderConsumptionTax_Return_NoContent()
    {
        var newTax1 = await CreateConsumptionTaxAsync();

        var newTax2 = await CreateConsumptionTaxAsync();

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newTax1.Id, newTax2.Id
            ]
        );

        var response = await Client.PutAsync(
            $"{BaseUrl}/order",
            TestUtil.ToJsonContent(orderRequest)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllConsumptionTax_ReturnOK_WithConsumptionTaxes()
    {
        _ = await CreateConsumptionTaxAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetConsumptionTax_ReturnOK_WithConsumptionTax()
    {
        var consumptionTax = await CreateConsumptionTaxAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{consumptionTax.Id}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
