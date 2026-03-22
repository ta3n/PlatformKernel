using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SharedKernel.Grpc;
using SharedKernel.Grpc.HttpClient;
using SharedKernel.Grpc.Interceptors;
using SharedKernel.Grpc.Options;

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
    public async Task ClientInterceptor_AddsDeadlineFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("GrpcClientPolicy:Timeout", "5")
            ])
            .Build();
        var interceptor = new ClientInterceptor(configuration, NullLogger<ClientInterceptor>.Instance);

        DateTime? deadline = null;
        var call = interceptor.AsyncUnaryCall(
            "request",
            CreateClientContext(),
            (request, context) =>
            {
                deadline = context.Options.Deadline;
                return CreateSuccessfulCall("ok");
            });
        var response = await call.ResponseAsync;

        Assert.Equal("ok", response);
        Assert.NotNull(deadline);
        Assert.InRange((deadline!.Value - DateTime.UtcNow).TotalSeconds, 0, 6);
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
