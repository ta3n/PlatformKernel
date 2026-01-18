using System.Net;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

public abstract class BaseMailTemplatesBaseEndpointIntTest(
    string type
) : BaseIntegrationTest
{
    private const string BaseUrl = "api/mail-templates";

    protected virtual string GetFormat()
    {
        var systemConfigRepo = Factory.GetRequiredService<ISystemConfigRepository>();
        var mailTemplates = systemConfigRepo!
                .GetQueryableWithAsNoTracking()
                .FirstOrDefault()
            ?? throw new SystemConfigNotfoundException();
        var templateFormatData = mailTemplates.TemplateFormatData ?? throw new SystemConfigNotfoundException();
        var template = templateFormatData.GetTemplate(
                type
            )
            ?? throw new SystemConfigNotfoundException();

        var response = JsonConvert.SerializeObject(template);

        return response;
    }

    private async Task<string> PreviewMailTemplateAsync()
    {
        var format = GetFormat();
        var previewMailRequest = new MailTemplatePreviewRequest(format) { IoType = type };
        var response = await Client.PostAsync(
            $"{BaseUrl}/{type}/preview",
            TestUtil.ToJsonContent(previewMailRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }

    [Fact]
    public async Task GetMailTemplate_Return200OK_WithMailTemplateResponse()
    {
        var response = await Client.GetAsync($"{BaseUrl}/{type}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task UpdateMailTemplate_ReturnNoContent()
    {
        var format = GetFormat();
        var updateMailRequest = new MailTemplateUpdateRequest(format) { IoType = type };
        var response = await Client.PutAsync(
            $"{BaseUrl}/{type}",
            TestUtil.ToJsonContent(updateMailRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PreviewMailTemplate_ReturnOK_WithMailTemplatePreviewResponse()
    {
        var responseString = await PreviewMailTemplateAsync();

        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task SendMail_ReturnOK()
    {
        var responseString = await PreviewMailTemplateAsync();

        var previewResponse = JsonConvert.DeserializeObject<MailTemplatePreviewResponse>(responseString);
        Assert.NotNull(previewResponse);

        var sendMailRequest = new MailTemplateSendRequest(
            previewResponse.Subject ?? "",
            previewResponse.Body ?? "",
            "test@gmail.com"
        );

        var sendMailRequestJson = TestUtil.ToJsonContent(sendMailRequest);

        var response = await Client.PostAsync(
            BaseUrl,
            sendMailRequestJson
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
