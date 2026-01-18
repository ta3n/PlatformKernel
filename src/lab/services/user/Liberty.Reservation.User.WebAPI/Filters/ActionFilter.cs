using Microsoft.AspNetCore.Mvc.Filters;

namespace Liberty.Reservation.Employee.WebAPI.Filters;

public interface IActionFilter : IAsyncActionFilter, IAsyncResultFilter
{
}

public class ActionFilter : IActionFilter
{
    public ActionFilter()
    {
        ;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        await next();
    }

    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next
    )
    {
        await next();
    }
}
