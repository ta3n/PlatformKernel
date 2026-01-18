using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Liberty.Fax.Models.Requests;
using Liberty.Fax.Models.Responses;
using Liberty.Fax.Options;
using Liberty.Fax.Services.Interfaces;
using Liberty.Fax.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Liberty.Fax.Services;

public class FaxService(
    HttpClient httpClient,
    IOptions<FaxOptions> faxSetting,
    ILogger<FaxService> logger
) : IFaxService
{
    private readonly FaxOptions _faxOptions = faxSetting.Value;

    public async Task<FaxResponse?> SendFaxAsync(
        long processKey,
        string faxNumber,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        var requestData = new FaxRequest
        {
            SendTo = [new FaxRecipient { FaxNo = faxNumber }],
            Subject = subject.TruncateWithEllipsis(),
            Body = body
        };
        logger.LogWarning(
            "Request has subject '{Subject}', process key '{ProcessKey}'",
            requestData.Subject,
            processKey
        );

        if (string.IsNullOrEmpty(faxNumber) || string.IsNullOrEmpty(_faxOptions.Password))
        {
            return null;
        }

        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var authorValue = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_faxOptions.UserName}:{_faxOptions.Password}")
        );
        var processKeyString = $"{processKey}{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        logger.LogInformation(
            "FAX send request: X-ProcessKey: {ProcessKeyString}, Authen: {AuthorValue}",
            processKeyString,
            authorValue
        );

        httpClient.DefaultRequestHeaders.Add("X-Auth", authorValue);
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        httpClient.DefaultRequestHeaders.Add("X-Processkey", processKeyString);

        var response = await httpClient.PostAsync(_faxOptions.ApiUrl, content, cancellationToken);

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(result) || !response.IsSuccessStatusCode)
        {
            logger.LogError(
                "FAX send failed: HttpStatus: {ResponseStatusCode}",
                response.StatusCode
            );
            return null;
        }

        var responseData = JsonSerializer.Deserialize<FaxResponse>(result);

        logger.LogInformation(
            "FAX send result: {ResponseDataResult}, Booking: {ResponseDataProcessKey}, Time: {ResponseDataAcceptTime}",
            responseData?.Result,
            responseData?.ProcessKey,
            responseData?.AcceptTime
        );

        if (!(responseData?.IsSuccess ?? false))
        {
            logger.LogError(
                "FAX send failed: {ResponseDataResult}, Booking: {ResponseDataProcessKey}, Time: {ResponseDataAcceptTime}",
                responseData?.Result,
                responseData?.ProcessKey,
                responseData?.AcceptTime
            );
        }

        return responseData;
    }
}
