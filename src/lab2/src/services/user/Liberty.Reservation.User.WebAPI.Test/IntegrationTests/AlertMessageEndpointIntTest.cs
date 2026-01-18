using System.Net;
using System.Text.Json;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class AlertMessageEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/alert-message";

    private async Task CreateAlertMessageAsync()
    {
        var alertMessageRepo = Factory.GetRequiredService<IAlertMessageRepository>();
        var mockData = new AlertMessage
        {
            Title = new MultilingualText { { TestUtil.DefaultAccpectLanguage, "Title" } },
            Content = new MultilingualText { { TestUtil.DefaultAccpectLanguage, "Content" } },
            IsEnabled = true
        };

        _ = await alertMessageRepo!.AddAsync(mockData, true);

        var acb = await alertMessageRepo.GetAllAsync();
        Console.WriteLine(acb);
    }

    private async Task<List<AlertMessageResponse>> GetAlertMessages()
    {
        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(response.StatusCode.ToString(), HttpStatusCode.OK.ToString());

        var responseString = await response.Content.ReadAsStringAsync();

        var alertMessageResponse = JsonSerializer.Deserialize<List<AlertMessageResponse>>(
            responseString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(alertMessageResponse);

        return alertMessageResponse;
    }

    [Fact]
    public async Task GetAlertMessages_ReturnsOk()
    {
        await CreateAlertMessageAsync();
        var response = await GetAlertMessages();
        Assert.NotNull(response);
        Assert.True(response.Count > 0);
    }

    [Fact]
    public async Task GetAlertMessages_NoAlertMessageActive()
    {
        var response = await GetAlertMessages();
        Assert.NotNull(response);
        Assert.True(response.Count == 0);
    }
}
