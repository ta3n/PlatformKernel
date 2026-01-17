using Microsoft.AspNetCore.Http;

namespace SharedKernel.ServiceDefaults.Middlewares;

public abstract class BaseMiddleware : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await next(context);
            return;
        }

        await HandleAsync(context, next);
    }

    protected abstract Task HandleAsync(
        HttpContext context,
        RequestDelegate next
    );
}
