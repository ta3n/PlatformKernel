using SharedKernel.MassTransit.Test.Api;
using SharedKernel.MassTransit.Test.Configuration;
using SharedKernel.MassTransit.Test.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

var appOptions = AppOptions.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(appOptions);
builder.Services.AddMassTransitTestApplication(builder.Configuration, appOptions);

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet(
    "/",
    () => Results.Ok(
        new
        {
            Service = "SharedKernel.MassTransit.Test",
            appOptions.Role,
            QueueType = app.Configuration["MessageQueueSettings:QueueType"]
        }
    )
);

app.MapGet(
    "/health",
    () => Results.Ok(
        new
        {
            Status = "Healthy",
            appOptions.Role
        }
    )
);

if (AppRoles.IsApi(appOptions.Role))
{
    app.MapOrderEndpoints();
}

await app.InitializeDatabaseAsync();
await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
