using System.Net;
using System.Text.Json;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class AlertMessageEndpointIntNoMessageTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/alert-message";

    private async Task CreateAlertMessageAsync()
    {
        var alertMessageRepo = Factory.GetRequiredService<IAlertMessageRepository>()
            ?? throw new ArgumentException(nameof(IAlertMessageRepository));

        var mockData = new AlertMessage
        {
            Title = new MultilingualText { { TestUtil.DefaultAcceptLanguage, "Title" } },
            Content = new MultilingualText { { TestUtil.DefaultAcceptLanguage, "Content" } },
            IsEnabled = true
        };

        _ = await alertMessageRepo.AddAsync(mockData, true);
    }

    private async Task<List<AlertMessageResponse>> GetAlertMessages()
    {
        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        var alertMessageResponse = JsonSerializer.Deserialize<List<AlertMessageResponse>>(
            responseString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(alertMessageResponse);

        return alertMessageResponse;
    }

    [Fact]
    public async Task GetAlertMessages_NoAlertMessageActive()
    {
        var response = await GetAlertMessages();

        Assert.NotNull(response);
        Assert.Empty(response);
    }
}
