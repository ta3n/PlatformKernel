using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SharedKernel.Grpc;
using SharedKernel.Grpc.HttpClient;
using SharedKernel.Grpc.Interceptors;
using SharedKernel.Grpc.Options;
using SharedKernel.Grpc.Providers;
using Moq;

namespace SharedKernel.Grpc.Test;

public class UnitTest1
{
    [Fact]
    public void AddCustomHttpClient_RegistersNamedClientWithBaseAddress()
    {
        var services = new ServiceCollection();
        services.AddCustomHttpClient("kernel", new Uri("https://example.com"), new HttpClientPolicyOptions { Enabled = false });

        var provider = services.BuildServiceProvider();
        var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var client = clientFactory.CreateClient("kernel");

        Assert.Equal(new Uri("https://example.com"), client.BaseAddress);
    }

    [Fact]
    public void AddCustomGrpcServer_RegistersServerInterceptor()
    {
        var services = new ServiceCollection();

        services.AddCustomGrpcServer();

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ServerInterceptor));
    }

    [Fact]
    public async Task ClientInterceptor_AddsDeadlineAndMetadataFromProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("GrpcClientPolicy:Timeout", "5")
            ])
            .Build();

        var metadataProviderMock = new Mock<IHeaderPropagationProvider>();
        var expectedMetadata = new Metadata { { "x-custom", "value" } };
        metadataProviderMock.Setup(x => x.GetGrpcMetadata()).Returns(expectedMetadata);

        var interceptor = new ClientInterceptor(configuration, NullLogger<ClientInterceptor>.Instance, metadataProviderMock.Object);

        Metadata? capturedMetadata = null;
        var call = interceptor.AsyncUnaryCall(
            "request",
            CreateClientContext(),
            (request, context) =>
            {
                capturedMetadata = context.Options.Headers;
                return CreateSuccessfulCall("ok");
            });
        await call.ResponseAsync;

        Assert.NotNull(capturedMetadata);
        var entry = capturedMetadata.FirstOrDefault(m => m.Key == "x-custom");
        Assert.NotNull(entry);
        Assert.Equal("value", entry.Value);
    }

    [Fact]
    public async Task ErrorHandlerInterceptor_WrapsInnerException()
    {
        var interceptor = new ErrorHandlerInterceptor();
        var call = interceptor.AsyncUnaryCall(
            "request",
            CreateClientContext(),
            (_, _) =>
            {
                var task = Task.FromException<string>(new InvalidOperationException("boom"));
                return new AsyncUnaryCall<string>(
                    task,
                    Task.FromResult(new Metadata()),
                    () => new Status(StatusCode.Unknown, string.Empty),
                    () => new Metadata(),
                    () => { });
            });
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => call.ResponseAsync);

        Assert.Equal("Custom error", exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public async Task HeaderPropagationHandler_AddsHeadersFromProvider()
    {
        var metadataProviderMock = new Mock<IHeaderPropagationProvider>();
        var expectedHeaders = new Dictionary<string, string> { { "X-Test", "Value" } };
        metadataProviderMock.Setup(x => x.GetHttpHeaders()).Returns(expectedHeaders);

        var handler = new HeaderPropagationHandler(metadataProviderMock.Object)
        {
            InnerHandler = new TestHandler()
        };

        var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        await invoker.SendAsync(request, CancellationToken.None);

        Assert.True(request.Headers.Contains("X-Test"));
        Assert.Equal("Value", request.Headers.GetValues("X-Test").First());
    }

    private class TestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }

    private static ClientInterceptorContext<string, string> CreateClientContext()
    {
        return new ClientInterceptorContext<string, string>(
            new Method<string, string>(
                MethodType.Unary,
                "kernel.Service",
                "Ping",
                Marshallers.StringMarshaller,
                Marshallers.StringMarshaller),
            "localhost",
            new CallOptions());
    }

    private static AsyncUnaryCall<string> CreateSuccessfulCall(string response)
    {
        return new AsyncUnaryCall<string>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, string.Empty),
            () => new Metadata(),
            () => { });
    }
}
