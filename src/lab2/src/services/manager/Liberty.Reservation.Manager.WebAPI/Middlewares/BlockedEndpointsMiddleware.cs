using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Manager.WebAPI.Middlewares;

public class BlockedEndpointsMiddleware(
    RequestDelegate next
)
{
    private const string RoomGroupsEndpoint = "RoomGroupsEndpoint";
    private const string RoomGroupPricesEndpoint = "RoomGroupPricesEndpoint";
    private const string BathingTaxAgesEndpoint = "BathingTaxAgesEndpoint";

    private readonly List<(string Controller, string Action)> _blockedEndpoints =
    [
        (RoomGroupsEndpoint, "UpdatePlanPaymentMethod"),
        (RoomGroupsEndpoint, "GetPlanPaymentMethod"),
        (RoomGroupsEndpoint, "UpdatePlanImportantNote"),
        (RoomGroupsEndpoint, "GetPlanImportantNote"),
        (RoomGroupsEndpoint, "UpdatePlanSpecial"),
        (RoomGroupsEndpoint, "GetPlanSpecial"),
        (RoomGroupsEndpoint, "UpdatePlanCancel"),
        (RoomGroupsEndpoint, "GetPlanCancel"),
        (RoomGroupsEndpoint, "UpdatePlanQuestion"),
        (RoomGroupsEndpoint, "GetPlanQuestion"),
        (RoomGroupsEndpoint, "UpdatePlanOption"),
        (RoomGroupsEndpoint, "GetPlanOption"),
        (RoomGroupsEndpoint, "UpdatePlanMeal"),
        (RoomGroupsEndpoint, "GetPlanMeal"),
        (RoomGroupsEndpoint, "UpdatePlanSale"),
        (RoomGroupsEndpoint, "GetPlanSale"),
        (RoomGroupPricesEndpoint, "*"),
        (BathingTaxAgesEndpoint, "CreateBathingTaxAgeAsync"),
        (BathingTaxAgesEndpoint, "ChangeVisibleTaxAgeAsync"),
        (BathingTaxAgesEndpoint, "ArrangeOrderAsync")
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
                throw new AppLibertyException("This endpoint is blocked.");
            }
        }

        await next(context);
    }
}
