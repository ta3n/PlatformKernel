var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddRabbitMQ("rebus-rabbitmq")
    .WithManagementPlugin();

await builder.Build().RunAsync();
