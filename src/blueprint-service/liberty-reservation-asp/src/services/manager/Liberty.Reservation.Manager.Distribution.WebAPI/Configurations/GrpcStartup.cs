using Liberty.Grpc;
using Liberty.Grpc.Options;
using Liberty.Protobuf.Site.V1;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

public static class GrpcStartup
{
    public static IServiceCollection AddGrpcModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var serviceOptions = configuration.GetSection("Services").Get<ServiceSetting>();
        var grpcClientPolicyOptions = configuration.GetSection(GrpcClientPolicyOptions.Name).Get<GrpcClientPolicyOptions>();

        ArgumentNullException.ThrowIfNull(grpcClientPolicyOptions);

        services
            .AddCustomGrpcServer()
            .AddCustomGrpcClient<SiteProtoEndpoint.SiteProtoEndpointClient>(
                serviceOptions?.SiteService?.GrpcUrl ?? string.Empty,
                grpcClientPolicyOptions
            );

        return services;
    }

    public static WebApplicationBuilder AddGrpcHost(
        this WebApplicationBuilder webApplicationBuilder,
        IConfiguration configuration
    )
    {
        var servicePort = Environment.GetEnvironmentVariable("Service_Port");
        var serviceGrpcPort = Environment.GetEnvironmentVariable("Service_GRPC_Port");

        webApplicationBuilder.WebHost.ConfigureKestrel(
            delegate(
                KestrelServerOptions options
            )
            {
                options.ListenAnyIP(
                    Convert.ToInt32(serviceGrpcPort),
                    delegate(
                        ListenOptions listenOptions
                    )
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    }
                );
                options.ListenAnyIP(
                    Convert.ToInt32(servicePort),
                    delegate(
                        ListenOptions listenOptions
                    )
                    {
                        listenOptions.Protocols = HttpProtocols.Http1;
                    }
                );
            }
        );
        return webApplicationBuilder;
    }
}
