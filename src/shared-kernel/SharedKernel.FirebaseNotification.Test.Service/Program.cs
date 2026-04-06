using SharedKernel.FirebaseNotification;
using SharedKernel.FirebaseNotification.Test.Service.Api;
using SharedKernel.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddFirebaseNotification(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.MapFirebaseNotificationEndpoints();

await app.RunAsync();

public partial class Program
{
    protected Program()
    {
    }
}
