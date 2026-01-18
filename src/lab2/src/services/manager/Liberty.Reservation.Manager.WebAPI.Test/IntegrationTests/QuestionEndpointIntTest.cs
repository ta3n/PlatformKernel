using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class QuestionEndpointIntTest : BaseIntegrationTest
{
    private string BaseUrl { get; set; } = "/api/questions";

    private async Task<string> CreateQuestionAsync()
    {
        var createReq = new QuestionCreateRequest(
            "Question 1",
            "Question 1 Description"
        );

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToJsonContent(createReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }

    [Fact]
    public async Task CreateQuestion_ReturnsOk_WithQuestion()
    {
        var responseString = await CreateQuestionAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderQuestion_Return_NoContent()
    {
        var newQuestionJson1 = await CreateQuestionAsync();
        _ = long.TryParse(newQuestionJson1, out var newQuestionId1);

        var newQuestionJson2 = await CreateQuestionAsync();
        _ = long.TryParse(newQuestionJson2, out var newQuestionId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newQuestionId1, newQuestionId2
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
    public async Task UpdateQuestion_ReturnsOk_WithQuestion()
    {
        var newQuestionResponse = await CreateQuestionAsync();
        var newQuestionId = long.TryParse(newQuestionResponse, out var id) ? id : 0;

        var updateReq = new QuestionUpdateRequest(
            "Option Item 2",
            "Option Item 2 Description",
            "{\"type\":5,\"data\":[\"aaa\"],\"selected\":\"aaa\",\"isRequired\":false,\"selectionMin\":null,\"selectionMax\":null,\"placeholder\":\"\",\"valueLabel\":\"aaa\"}",
            QuestionTypes.Text,
            true
        ) { Id = newQuestionId };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{newQuestionId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableQuestion_ReturnsNoContent_WithQuestion()
    {
        var newQuestionResponse = await CreateQuestionAsync();
        var newQuestionId = long.TryParse(newQuestionResponse, out var id) ? id : 0;

        await Client.PutAsync(
            $"{BaseUrl}/{newQuestionId}",
            TestUtil.ToJsonContent(
                new QuestionUpdateRequest(
                    "Option Item 2",
                    "Option Item 2 Description",
                    "{\"type\":5,\"data\":[\"aaa\"],\"selected\":\"aaa\",\"isRequired\":false,\"selectionMin\":null,\"selectionMax\":null,\"placeholder\":\"\",\"valueLabel\":\"aaa\"}",
                    QuestionTypes.Text,
                    true
                ) { Id = newQuestionId }
            )
        );

        var updateReq = new QuestionEnabledRequest(true) { Id = newQuestionId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newQuestionId}/enable",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestion_ReturnsOk_WithQuestion()
    {
        var newQuestionResponse = await CreateQuestionAsync();
        var newQuestionId = long.TryParse(newQuestionResponse, out var id) ? id : 0;

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{newQuestionId}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllQuestions_ReturnsOk_WithQuestionList()
    {
        await CreateQuestionAsync();

        var response = await Client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllEnableQuestions_ReturnsOk_WithQuestionList()
    {
        await CreateQuestionAsync();

        var response = await Client.GetAsync($"{BaseUrl}/?enable=true");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetQuestion_ReturnsOk_WithQuestion()
    {
        var newQuestionResponse = await CreateQuestionAsync();
        var newQuestionId = long.TryParse(newQuestionResponse, out var id) ? id : 0;

        var response = await Client.GetAsync($"{BaseUrl}/{newQuestionId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
