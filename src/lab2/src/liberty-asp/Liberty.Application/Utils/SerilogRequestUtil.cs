using Liberty.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System.Text;

namespace Liberty.Application.Utils;

public static class SerilogRequestUtil
{
    public const string HttpMessageTemplate =
        "HTTP {RequestMethod} {RequestPath} QueryString:{QueryString} Body:{Body}  responded {StatusCode} in {Elapsed:0.0000} ms";

    private static readonly List<string> _ignoreUrl = new()
    {
        "/job",
    };

    private static LogEventLevel DefaultGetLevel(HttpContext ctx,
        double _,
        Exception? ex)
    {
        return ex is null && ctx.Response.StatusCode <= 499 ? LogEventLevel.Information : LogEventLevel.Error;
    }

    public static LogEventLevel GetRequestLevel(HttpContext ctx, double _, Exception? ex) =>
        ex is null && ctx.Response.StatusCode <= 499 ? IgnoreRequest(ctx) : LogEventLevel.Error;

    private static LogEventLevel IgnoreRequest(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value;
        if (path.IsNullOrEmpty())
        {
            return LogEventLevel.Information;
        }

        return _ignoreUrl.Any(s => path.StartsWith(s)) ? LogEventLevel.Verbose : LogEventLevel.Information;
    }

    /// <summary>
    /// Add additional attributes from the Request
    /// </summary>
    /// <param name="diagnosticContext"></param>
    /// <param name="httpContext"></param>
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        diagnosticContext.Set("RequestHost", request.Host);
        diagnosticContext.Set("RequestScheme", request.Scheme);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("RequestIp", httpContext.GetRequestIp());

        if (request.Method == HttpMethods.Get)
        {
            diagnosticContext.Set("QueryString", request.QueryString.HasValue ? request.QueryString.Value : string.Empty);
            diagnosticContext.Set("Body", string.Empty);
        }
        else
        {
            diagnosticContext.Set("QueryString", request.QueryString.HasValue ? request.QueryString.Value : string.Empty);
            diagnosticContext.Set("Body", request.ContentLength > 0 ? request.GetRequestBody() : string.Empty);
        }

        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        var endpoint = httpContext.GetEndpoint();
        if (endpoint != null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }
    public static string GetRequestIp(this HttpContext context)
    {
        string ip = SplitCsv(GetHeaderValueAs<string>(context, "X-Forwarded-For")).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(ip))
            ip = SplitCsv(GetHeaderValueAs<string>(context, "X-Real-IP")).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(ip) && context.Connection?.RemoteIpAddress != null)
            ip = context.Connection.RemoteIpAddress.ToString();

        if (string.IsNullOrWhiteSpace(ip))
            ip = GetHeaderValueAs<string>(context, "REMOTE_ADDR");

        return ip;
    }
    public static string GetRequestBody(this HttpRequest request)
    {
        if (!request.Body.CanRead)
        {
            return default;
        }

        if (!request.Body.CanSeek)
        {
            return default;
        }

        if (request.Body.Length < 1)
        {
            return default;
        }

        var bodyStr = "";
        request.Body.Seek(0, SeekOrigin.Begin);
        using (StreamReader reader
               = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            bodyStr = reader.ReadToEnd();
        }

        request.Body.Position = 0;
        return bodyStr;
    }
    private static T GetHeaderValueAs<T>(HttpContext context, string headerName)
    {
        if (context.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
        {
            string rawValues = values.ToString();

            if (!string.IsNullOrWhiteSpace(rawValues))
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
        }

        return default;
    }

    private static List<string> SplitCsv(string csvList)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return new List<string>();

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable<string>()
            .Select(s => s.Trim())
            .ToList();
    }
}
