using SharedKernel.ServiceDefaults.Middlewares;

namespace PlatformKernel.Service.WebApi.Middlewares;

public class CheckTenantAvailableMiddleware(
    ILogger<CheckTenantAvailableMiddleware> logger
) : BaseMiddleware
{
    protected override async Task HandleAsync(
        HttpContext context,
        RequestDelegate next
    )
    {

        logger.LogInformation("CheckTenantAvailableMiddleware invoked.");

        await next(context);
    }

}
