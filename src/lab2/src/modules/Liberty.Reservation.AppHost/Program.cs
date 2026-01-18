// This code sets up a distributed application using .NET Aspire
// It configures multiple microservices and workers that make up the Liberty Reservation system:

using Liberty.Reservation.AppHost;

// Creates a builder for configuring the distributed application
var builder = DistributedApplication.CreateBuilder(args);

// Adds support for forwarded headers (useful when behind a proxy/load balancer)
builder.AddForwardedHeaders();

var rootDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../.."));

const int dbPort = 5435;
var postgresUser = builder.AddParameter("db-user", "postgres", secret: true);
var postgresPassword = builder.AddParameter("db-password", "", secret: true);
var dbDumpsPath = Path.Combine(rootDir, "db", "dumps");
var postgres = builder
    .AddPostgres("db", postgresUser, postgresPassword, dbPort)
    .WithImage("postgres", "16.4") // Ensure we use the same image as docker-compose
    .WithContainerName("liberty-db") // Use a fixed container name
    .WithPgAdmin(
        config => config
            .WithContainerName("liberty-pgadmin")
            .WithLifetime(ContainerLifetime.Persistent)
    ) // Adds pgAdmin for easy database management
    .WithLifetime(ContainerLifetime.Persistent) // Don't tear-down the container when we stop Aspire
    .WithDataVolume("liberty-db-data") // Wire up the PostgreSQL data volume
    .WithBindMount(dbDumpsPath, "/docker-entrypoint-initdb.d")
    .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust");
var facilityDb = postgres.AddDatabase("facility", "liberty_facility");
var reservationDb = postgres.AddDatabase("reservation", "liberty_reservation");

var redisPassword = builder.AddParameter("redis-password", "01j6efde8q4", secret: true);
var redis = builder.AddRedis("redis")
    .WithRedisCommander(
        config => config
            .WithContainerName("liberty-redis-commander")
            .WithLifetime(ContainerLifetime.Persistent)
    ) // Adds Redis Commander for easy management
    .WithContainerName("liberty-redis") // Use a fixed container name
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPassword(redisPassword);

var rabbitMqUser = builder.AddParameter("rabbitmq-user", "admin", true);
var rabbitMqPassword = builder.AddParameter("rabbitmq-password", "01jErQ4kB7D11e78TyX92WqBN8", secret: true);
var rabbitMq = builder
    .AddRabbitMQ("eventbus", rabbitMqUser, rabbitMqPassword)
    .WithContainerName("liberty-eventbus") // Use a fixed container name
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("RABBITMQ_DEFAULT_USER", rabbitMqUser)
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", rabbitMqPassword);

var seqApiKey = builder.AddParameter("seq-api-key", "01jErQ4kB7D11e78TyX92WqBN8", secret: true);
var seq = builder.AddSeq("seq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("datalust/seq:latest")
    .WithContainerName("liberty-seq")
    .WithDataVolume("liberty-seq-data")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SEQ_FLEET", "true")
    .WithEnvironment("SEQ_API_KEY", seqApiKey)
    .WithEnvironment("SEQ_FIRSTRUN_NOAUTHENTICATION", "true");

var launchProfileName = ShouldUseHttpsForEndpoints() ? "https" : "http";
// var runDbMigration = RunDbMigration();

const string envNameContextConnection = "ConnectionStrings__DataContextConnection";
const string envNameMembershipContextConnection = "Services__MembershipFacilityService__DataContextConnection";
const string envNameRedisUrlConfiguration = "Redis__UrlConfiguration";

builder.AddProject<Projects.Liberty_Reservation_DbMigration>(
        "db-migration",
        launchProfileName
    )
    .WaitFor(postgres)
    .WithEnvironment("ConnectionStrings__MigrationsAssembly", "Liberty.Reservation.DbMigration")
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    );

// Adds the Employee API service
builder.AddProject<Projects.Liberty_Reservation_Employee_WebAPI>(
        "employee-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Adds Manager-related services:
// - Main Manager API
// - Manager File API for file operations
// - Manager background worker service
builder.AddProject<Projects.Liberty_Reservation_Manager_WebAPI>(
        "manager-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .AddWriteToSeqEnv(seq, seqApiKey);

builder.AddProject<Projects.Liberty_Reservation_Manager_File_WebAPI>(
        "manager-file-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .AddRabbitMqEnv(rabbitMq, rabbitMqUser, rabbitMqPassword)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Adds Site-related services:
// - Main Site API
// - Site Public API for public-facing operations
// - Site File API for file operations
builder.AddProject<Projects.Liberty_Reservation_Site_WebAPI>(
        "site-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .AddWriteToSeqEnv(seq, seqApiKey);

builder.AddProject<Projects.Liberty_Reservation_Site_Public_WebAPI>(
        "site-public-api",
        launchProfileName
    )
    .AddWriteToSeqEnv(seq, seqApiKey);

builder.AddProject<Projects.Liberty_Reservation_Site_File_WebAPI>(
        "site-file-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .AddRabbitMqEnv(rabbitMq, rabbitMqUser, rabbitMqPassword)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Adds User-related services:
// - Main User API
// - User File API for file operations
builder.AddProject<Projects.Liberty_Reservation_User_WebAPI>(
        "user-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .AddWriteToSeqEnv(seq, seqApiKey);

builder.AddProject<Projects.Liberty_Reservation_User_File_WebAPI>(
        "user-file-api",
        launchProfileName
    )
    .WaitFor(postgres)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .AddRabbitMqEnv(rabbitMq, rabbitMqUser, rabbitMqPassword)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Adds a background worker for handling mail operations
builder.AddProject<Projects.Liberty_Reservation_Mail_Worker>(
        "mail-worker",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .AddRabbitMqEnv(rabbitMq, rabbitMqUser, rabbitMqPassword)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Adds a background worker for handling booking operations
builder.AddProject<Projects.Liberty_Reservation_Booking_Worker>(
        "booking-worker",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .AddRabbitMqEnv(rabbitMq, rabbitMqUser, rabbitMqPassword)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Builds and runs the distributed application
builder.AddProject<Projects.Liberty_Reservation_Manager_Distribution_WebAPI>(
        "distribution-webapi",
        launchProfileName
    )
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithEnvironment(
        envNameContextConnection,
        $"{reservationDb};Pooling=true;Minimum Pool Size=5;Maximum Pool Size=25;Connection Idle Lifetime=300;"
    )
    .WithEnvironment(envNameMembershipContextConnection, facilityDb)
    .WithEnvironment(envNameRedisUrlConfiguration, redis)
    .AddRabbitMqEnv(rabbitMq, rabbitMqUser, rabbitMqPassword)
    .AddWriteToSeqEnv(seq, seqApiKey);

// Builds and runs the distributed application
await builder.Build().RunAsync();

return;

// For test use only.
// Looks for an environment variable that forces the use of HTTP for all the endpoints. We
// are doing this for ease of running the Playwright tests in CI.
static bool ShouldUseHttpsForEndpoints()
{
    const string envVarName = "LIBERTY_USE_HTTPS_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(envVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out var result) && result == 1;
}

// static bool RunDbMigration()
// {
//     const string envVarName = "LIBERTY_RUN_DB_MIGRATION";
//     var envValue = Environment.GetEnvironmentVariable(envVarName);
//     return int.TryParse(envValue, out var result) && result == 1;
// }
