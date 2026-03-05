using System.Net;
using System.Text.Json;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class AlertMessageEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/alert-message";

    private async Task CreateAlertMessageAsync()
    {
        var alertMessageRepo = Factory.GetRequiredService<IAlertMessageRepository>();
        var mockData = new AlertMessage
        {
            Title = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Title" } },
            Content = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Content" } },
            IsEnabled = true
        };

        _ = await alertMessageRepo!.AddAsync(mockData, true);
    }

    private async Task<AlertMessageResponse> GetAlertMessage()
    {
        await CreateAlertMessageAsync();

        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        var alertMessageResponse = JsonSerializer.Deserialize<AlertMessageResponse>(
            responseString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(alertMessageResponse);

        return alertMessageResponse;
    }

    private async Task<HttpResponseMessage> UpdateAlertMessage(
        AlertMessageUpdateRequest request,
        long id
    )
    {
        var response = await Client.PutAsync(
            $"{BaseUrl}/{id}",
            TestUtil.ToJsonContent(request)
        );

        return response;
    }

    [Fact]
    public async Task GetAlertMessage_ReturnsOk()
    {
        var response = await GetAlertMessage();
        Assert.NotNull(response);
        Assert.True(response.Id > 0);
    }

    [Fact]
    public async Task UpdateAlertMessage_ReturnsNoContent()
    {
        var alertMessage = await GetAlertMessage();
        Assert.NotNull(alertMessage);
        Assert.True(alertMessage.Id > 0);

        var updateRequest = new AlertMessageUpdateRequest(
            "title test",
            "content test update",
            "",
            "",
            false
        ) { Id = alertMessage.Id };

        var response = await UpdateAlertMessage(updateRequest, alertMessage.Id);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAlertMessage_InvalidRequest()
    {
        var alertMessage = await GetAlertMessage();
        Assert.NotNull(alertMessage);
        Assert.True(alertMessage.Id > 0);

        var updateRequest = new AlertMessageUpdateRequest(
            "title test",
            new string('a', 251),
            "",
            "",
            false
        ) { Id = alertMessage.Id };

        var response = await UpdateAlertMessage(updateRequest, alertMessage.Id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
