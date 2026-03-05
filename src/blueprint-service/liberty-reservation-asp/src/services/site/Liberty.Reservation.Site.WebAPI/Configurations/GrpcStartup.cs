using Liberty.Grpc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

public static class GrpcStartup
{
    public static IServiceCollection AddGrpcModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        services.AddCustomGrpcServer();

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
