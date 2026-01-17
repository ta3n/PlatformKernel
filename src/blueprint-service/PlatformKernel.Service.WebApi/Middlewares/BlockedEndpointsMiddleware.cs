using SharedKernel.SysException.Exceptions;

namespace PlatformKernel.Service.WebApi.Middlewares;

public class BlockedEndpointsMiddleware(
    RequestDelegate next
)
{
    private const string DefaultGroupsEndpoint = "DefaultEndpoint";

    private readonly List<(string Controller, string Action)> _blockedEndpoints =
    [
        (DefaultGroupsEndpoint, "Create")
    ];

    public async Task InvokeAsync(
        HttpContext context
    )
    {
        var controller = context.Request.RouteValues["controller"]?.ToString();
        var action = context.Request.RouteValues["action"]?.ToString();

        if (controller != null && action != null)
        {
            var endpoint = (controller, action);
            var endpointBlockedAll = (controller, "*");

            if (_blockedEndpoints.Contains(endpoint) || _blockedEndpoints.Contains(endpointBlockedAll))
            {
                throw new AppBaseException("This endpoint is blocked.");
            }
        }

        await next(context);
    }
}
