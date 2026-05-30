using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsertPipeline.Extensions;
using SharedKernel.BulkInsertPipeline.Options;
using SharedKernel.BulkInsertPipeline.Test.Service.Api;
using SharedKernel.BulkInsertPipeline.Test.Service.Configuration;
using SharedKernel.BulkInsertPipeline.Test.Service.Data;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;
using SharedKernel.BulkInsertPipeline.Test.Service.Services;
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

var connectionString = builder.Configuration.GetConnectionString(BulkInsertPipelineLabDatabase.ConnectionStringName)
    ?? throw new InvalidOperationException(
        $"Connection string '{BulkInsertPipelineLabDatabase.ConnectionStringName}' was not found."
    );
var labOptions = builder.Configuration.GetSection(BulkInsertPipelineLabOptions.SectionName)
        .Get<BulkInsertPipelineLabOptions>()
    ?? new BulkInsertPipelineLabOptions();

builder.Services.AddSingleton(labOptions);
builder.Services.AddDbContextFactory<BulkInsertPipelineLabDbContext>(
    options => options.UseNpgsql(connectionString)
);
builder.Services.AddSingleton<BulkInsertPipelineLabDatabase>();
builder.Services.AddSingleton<PipelineDirectInsertRunner>();

builder.Services.AddBulkInsert(
        options => ConfigureHostedBulkInsert(options, labOptions)
    )
    .AddEntity<MetricReading>(
        entity => MetricReadingRegistration.ConfigureEntity(entity, connectionString)
    );

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.MapBulkInsertPipelineLabEndpoints();

await app.RunAsync();

static void ConfigureHostedBulkInsert(
    BulkInsertOptions options,
    BulkInsertPipelineLabOptions labOptions
)
{
    MetricReadingRegistration.ConfigureOptions(
        options,
        labOptions.HostedProvider,
        labOptions.FallbackProvider,
        labOptions.WorkerCount,
        labOptions.ChannelCapacity
    );
}

public partial class Program
{
    protected Program()
    {
    }
}
