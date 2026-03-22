using Microsoft.AspNetCore.Http;
using SharedKernel.ServiceDefaults;
using SharedKernel.ServiceDefaults.Middlewares;

namespace SharedKernel.ServiceDefaults.Test;

public class UnitTest1
{
    [Fact]
    public async Task BaseMiddleware_ForOptionsRequestCallsNextDirectly()
    {
        var middleware = new TrackingMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Options;
        var nextCalled = false;

        await middleware.InvokeAsync(
            context,
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        Assert.True(nextCalled);
        Assert.False(middleware.HandleCalled);
    }

    [Fact]
    public async Task BaseMiddleware_ForNonOptionsRequestInvokesHandleAsync()
    {
        var middleware = new TrackingMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        Assert.True(middleware.HandleCalled);
    }

    [Fact]
    public void GetRuntimeInfoAsText_ContainsExpectedHeader()
    {
        var runtimeInfo = LogRuntimeInfo.GetRuntimeInfoAsText();

        Assert.Contains(".NET RUNTIME CONFIGURATION", runtimeInfo, StringComparison.Ordinal);
        Assert.Contains("Runtime Information:", runtimeInfo, StringComparison.Ordinal);
    }

    private sealed class TrackingMiddleware : BaseMiddleware
    {
        public bool HandleCalled { get; private set; }

        protected override async Task HandleAsync(HttpContext context, RequestDelegate next)
        {
            HandleCalled = true;
            await next(context);
        }
    }
}
