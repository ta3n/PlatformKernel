using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Extensions;
using SharedKernel.BulkInsert.Test.Service.Api;
using SharedKernel.BulkInsert.Test.Service.Data;
using SharedKernel.BulkInsert.Test.Service.Domain;
using SharedKernel.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(
    options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

var connectionString = builder.Configuration.GetConnectionString(BulkInsertLabDatabase.ConnectionStringName)
    ?? throw new InvalidOperationException(
        $"Connection string '{BulkInsertLabDatabase.ConnectionStringName}' was not found."
    );

builder.Services.AddDbContextFactory<BulkInsertLabDbContext>(
    options => options.UseNpgsql(connectionString)
);
builder.Services.AddSingleton<BulkInsertLabDatabase>();
builder.Services.AddFluentBulkInsert()
    .AddEntity<LabOrder>(
        entity => entity
            .UseDbContextFactory<BulkInsertLabDbContext>()
            .UseRepoDb()
            .WithBatchSize(1_000)
            .WithTimeoutSeconds(60)
    );

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.MapBulkInsertLabEndpoints();

await app.RunAsync();
