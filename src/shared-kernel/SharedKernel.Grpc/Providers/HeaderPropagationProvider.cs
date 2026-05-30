using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace SharedKernel.Grpc.Providers;

internal sealed class HeaderPropagationProvider(
    IHttpContextAccessor httpContextAccessor
) : IHeaderPropagationProvider
{
    private static readonly string[] DefaultHeaders =
    [
        HeaderNames.Authorization,
        HeaderNames.AcceptLanguage,
        "X-Correlation-ID",
        "X-Request-ID"
    ];

    public Metadata GetGrpcMetadata()
    {
        Metadata metadata = [];
        var context = httpContextAccessor.HttpContext;

        if (context is null)
        {
            return metadata;
        }

        var headers = context.Request.Headers;
        foreach (var headerName in DefaultHeaders.Where(
                h => headers.TryGetValue(h, out var v) && !string.IsNullOrEmpty(v)
            ))
        {
            metadata.Add(headerName.ToLowerInvariant(), headers[headerName]!);
        }

        return metadata;
    }

    public IDictionary<string, string> GetHttpHeaders()
    {
        Dictionary<string, string> propagationHeaders = [];
        var context = httpContextAccessor.HttpContext;

        if (context is null)
        {
            return propagationHeaders;
        }

        var headers = context.Request.Headers;
        foreach (var headerName in DefaultHeaders.Where(
                h => headers.TryGetValue(h, out var v) && !string.IsNullOrEmpty(v)
            ))
        {
            propagationHeaders[headerName] = headers[headerName]!;
        }

        return propagationHeaders;
    }
}
