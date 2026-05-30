using SharedKernel.Grpc.Providers;

namespace SharedKernel.Grpc.HttpClient;

internal sealed class HeaderPropagationHandler(
    IHeaderPropagationProvider headerPropagationProvider
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var headers = headerPropagationProvider.GetHttpHeaders();

        foreach (var (key, value) in headers.Where(header => !request.Headers.Contains(header.Key)))
        {
            request.Headers.TryAddWithoutValidation(key, value);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
