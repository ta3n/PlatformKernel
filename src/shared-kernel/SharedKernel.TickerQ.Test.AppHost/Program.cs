var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("tickerqdb");

var redis = builder.AddRedis("redis");

builder
    .AddProject<Projects.SharedKernel_TickerQ_Test_Service>("tickerq-test-service")
    .WithReference(postgres)
    .WithReference(redis);

builder.Build().Run();
