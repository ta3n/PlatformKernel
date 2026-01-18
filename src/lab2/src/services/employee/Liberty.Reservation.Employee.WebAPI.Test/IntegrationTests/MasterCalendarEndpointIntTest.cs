using System.Net;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class MasterCalendarEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/master-calendar";

    private async Task<string> CreateDataOfDateAsync()
    {
        var createReq = new MasterCalendarEditDataRequest(20240819, "Name");

        var response = await Client.PostAsync(
            $"{BaseUrl}/data",
            TestUtil.ToJsonContent(createReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    private async Task<string> CreateTypeOfDateAsync()
    {
        var appDateTypeRepo = Factory.GetRequiredService<IAppDateTypeRepository>();
        var appDateType = new AppDateType { Name = "Name" };
        var newAppDateType = await appDateTypeRepo!.AddAsync(appDateType, true);

        var rand = new Random();
        var createReq = new MasterCalendarEditTypeRequest(
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(99))),
            newAppDateType.Id
        );

        var response = await Client.PostAsync(
            $"{BaseUrl}/type",
            TestUtil.ToJsonContent(createReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    [Fact]
    public async Task CreateDataOfDate_ReturnOK()
    {
        var appDate = await CreateDataOfDateAsync();
        Assert.NotEmpty(appDate);
    }

    [Fact]
    public async Task CreateTypeOfDate_ReturnOK()
    {
        var responseString = await CreateTypeOfDateAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task DeleteDataOfDate_ReturnNoContent()
    {
        var appDate = await CreateDataOfDateAsync();

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{appDate}/data"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTypeOfDate_ReturnNoContent()
    {
        var appDate = await CreateTypeOfDateAsync();

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{appDate}/type"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllDates_ReturnOK()
    {
        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();

        var appDate = new AppDate { DateTime = DateTime.UtcNow };
        _ = await appDateRepo!.AddAsync(appDate);

        var response = await Client.GetAsync($"{BaseUrl}?startDate=20240809&endDate=20240819");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllEnableDates_ReturnOK()
    {
        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();

        var appDate = new AppDate { DateTime = DateTime.UtcNow };
        _ = await appDateRepo!.AddAsync(appDate);

        var response = await Client.GetAsync($"{BaseUrl}?startDate=20240809&endDate=20240819&enable=true");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
