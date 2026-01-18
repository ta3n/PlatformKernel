using System.Net;
using System.Text;
using Liberty.Reservation.Site.Public.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Liberty.Reservation.Site.Public.WebAPI.Web.ApiService;
using Liberty.Reservation.Site.Public.WebAPI.Web.Extensions;
using Newtonsoft.Json.Linq;

namespace Liberty.Reservation.Site.Public.WebAPI.Boundaries.Restful.Base;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
public abstract class BaseEndpoint(
    IExternalPublicApiService externalApiService
) : ControllerBase
{
    private string[] _ignorePropertiesForResponse =
    [
        "id"
    ];

    protected void AdditionalIgnorePropertiesForResponse(
        params string[] properties
    )
    {
        _ignorePropertiesForResponse = [.. _ignorePropertiesForResponse, .. properties];
    }

    protected async Task<IActionResult> CallApiAsync(
        string payload,
        CancellationToken cancellationToken = default
    )
    {
        var method = HttpContext.Request.Method;
        var route = HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;
        var query = HttpContext.Request.QueryString.Value ?? string.Empty;

        var fullPath = $"{route}{query}";

        HttpResponseModel result;

        switch (method)
        {
            case var _ when HttpMethods.IsGet(method):
                result = await externalApiService.GetAsync(
                    ExternalService.SiteService,
                    fullPath,
                    cancellationToken
                );
                break;

            case var _ when HttpMethods.IsPost(method):
                result = await externalApiService.PostAsync(
                    ExternalService.SiteService,
                    fullPath,
                    payload,
                    cancellationToken
                );
                break;

            case var _ when HttpMethods.IsPut(method):
                result = await externalApiService.PutAsync(
                    ExternalService.SiteService,
                    fullPath,
                    payload,
                    cancellationToken
                );
                break;

            case var _ when HttpMethods.IsPatch(method):
                result = await externalApiService.PatchAsync(
                    ExternalService.SiteService,
                    fullPath,
                    payload,
                    cancellationToken
                );
                break;

            case var _ when HttpMethods.IsDelete(method):
                result = await externalApiService.DeleteAsync(
                    ExternalService.SiteService,
                    fullPath,
                    cancellationToken
                );
                break;

            default:
                return BadRequest($"Unsupported HTTP method: {method}");
        }

        var content = Encoding.UTF8.GetString(result.Content);
        var sanitized = content;

        if (result.StatusCode is HttpStatusCode.OK)
        {
            sanitized = RemoveIdProperties(content);
        }

        return new ContentResult
        {
            StatusCode = (int)result.StatusCode,
            Content = sanitized
        }.WithHeaders(result.Headers);
    }

    /// <summary>
    /// Removes all properties named "id" (case-insensitive) from a JSON string.
    /// </summary>
    /// <param name="json">
    /// The JSON string to process. If the input is null, empty, or not a valid JSON string,
    /// the original input is returned.
    /// </param>
    /// <returns>
    /// A JSON string with all properties named "id" removed. If the input is not a valid JSON string,
    /// the original input is returned.
    /// </returns>
    private string RemoveIdProperties(
        string json
    )
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        // If the input does not appear to be JSON, return it as-is to avoid parsing errors.
        // Valid JSON typically starts with '{' or '[' (ignoring BOM/whitespace).
        var span = json.AsSpan().Trim();
        if (span.Length == 0 || (span[0] != '{' && span[0] != '['))
        {
            return json;
        }

        // Parse the JSON string into a JToken for manipulation.
        JToken token;
        try
        {
            token = JToken.Parse(json);
        }
        catch
        {
            // If the input is not valid JSON, return it as-is.
            return json;
        }

        // Recursively remove "id" properties from the JSON structure.
        StripIds(token);

        // Convert the modified JToken back to a compact JSON string.
        return token.ToString(Newtonsoft.Json.Formatting.None);
    }

    /// <summary>
    /// Recursively removes properties from a JSON token that match the names specified
    /// in the <see cref="_ignorePropertiesForResponse"/> collection.
    /// Supports both JSON objects and arrays.
    /// </summary>
    private void StripIds(
        JToken token
    )
    {
        switch (token)
        {
            case JObject obj:
                ProcessJObject(obj);
                break;
            case JArray array:
                ProcessJArray(array);
                break;
        }
    }

    /// <summary>
    /// Removes matching properties and recursively processes child tokens within a JObject.
    /// </summary>
    private void ProcessJObject(
        JObject obj
    )
    {
        var toRemove = obj.Properties()
            .Where(p => ShouldIgnoreProperty(p.Name))
            .ToList();

        foreach (var p in toRemove)
        {
            p.Remove();
        }

        foreach (var child in obj.Properties().Select(p => p.Value))
        {
            StripIds(child);
        }

        bool ShouldIgnoreProperty(
            string propertyName
        )
        {
            return _ignorePropertiesForResponse
                .Contains(propertyName, StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Recursively processes all items within a JArray.
    /// </summary>
    private void ProcessJArray(
        JArray array
    )
    {
        foreach (var item in array)
        {
            StripIds(item);
        }
    }
}
