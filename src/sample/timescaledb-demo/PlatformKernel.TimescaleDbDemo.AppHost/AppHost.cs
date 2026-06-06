var builder = DistributedApplication.CreateBuilder(args);

var databaseName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "timescale_demo";
var defaultDatabaseUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
var defaultDatabasePassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
var timescaleTuneMemory = Environment.GetEnvironmentVariable("TIMESCALE_TUNE_MEMORY") ?? "16GB";
var timescaleTuneCpuCount = Environment.GetEnvironmentVariable("TIMESCALE_TUNE_NUM_CPUS") ?? "4";
var timescaleTuneMaxBackgroundWorkers = Environment.GetEnvironmentVariable("TIMESCALE_TUNE_MAX_BG_WORKERS") ?? "16";
var timescaleTuneMaxConnections = Environment.GetEnvironmentVariable("TIMESCALE_TUNE_MAX_CONNS") ?? "300";
var timescaleSharedMemorySize = Environment.GetEnvironmentVariable("TIMESCALE_SHM_SIZE") ?? "4g";
var timescaleNofileLimit = Environment.GetEnvironmentVariable("TIMESCALE_NOFILE_HARD") ?? "65535";
var pgbouncerMaxClientConnections = Environment.GetEnvironmentVariable("PGBOUNCER_MAX_CLIENT_CONN") ?? "20000";
var pgbouncerDefaultPoolSize = Environment.GetEnvironmentVariable("PGBOUNCER_DEFAULT_POOL_SIZE") ?? "200";
var pgbouncerMinPoolSize = Environment.GetEnvironmentVariable("PGBOUNCER_MIN_POOL_SIZE") ?? "50";
var pgbouncerReservePoolSize = Environment.GetEnvironmentVariable("PGBOUNCER_RESERVE_POOL_SIZE") ?? "50";
var pgbouncerReservePoolTimeout = Environment.GetEnvironmentVariable("PGBOUNCER_RESERVE_POOL_TIMEOUT") ?? "2";
var pgbouncerMaxDatabaseConnections = Environment.GetEnvironmentVariable("PGBOUNCER_MAX_DB_CONNECTIONS") ?? "250";
var pgbouncerMaxDatabaseClientConnections = Environment.GetEnvironmentVariable("PGBOUNCER_MAX_DB_CLIENT_CONNECTIONS") ?? "20000";
var pgbouncerQueryWaitTimeout = Environment.GetEnvironmentVariable("PGBOUNCER_QUERY_WAIT_TIMEOUT") ?? "30";
var pgbouncerServerIdleTimeout = Environment.GetEnvironmentVariable("PGBOUNCER_SERVER_IDLE_TIMEOUT") ?? "30";
var pgbouncerServerLifetime = Environment.GetEnvironmentVariable("PGBOUNCER_SERVER_LIFETIME") ?? "1800";
var pgbouncerListenBacklog = Environment.GetEnvironmentVariable("PGBOUNCER_LISTEN_BACKLOG") ?? "8192";
var pgbouncerNofileLimit = Environment.GetEnvironmentVariable("PGBOUNCER_NOFILE_HARD") ?? "262144";
var pgbouncerSomaxconn = Environment.GetEnvironmentVariable("PGBOUNCER_SOMAXCONN") ?? "8192";
var pgbouncerAuthType = Environment.GetEnvironmentVariable("PGBOUNCER_AUTH_TYPE") ?? "scram-sha-256";

var databaseUser = builder.AddParameter("timescaledb-user", defaultDatabaseUser, true);
var databasePassword = builder.AddParameter("timescaledb-password", defaultDatabasePassword, true);

var timescaledb = builder
    .AddPostgres("timescaledb", databaseUser, databasePassword, 55432)
    .WithImage("timescale/timescaledb", "latest-pg17")
    .WithDataVolume("timescaledb-demo-data")
    .WithEnvironment("TIMESCALEDB_TELEMETRY", "off")
    .WithEnvironment("TS_TUNE_MEMORY", timescaleTuneMemory)
    .WithEnvironment("TS_TUNE_NUM_CPUS", timescaleTuneCpuCount)
    .WithEnvironment("TS_TUNE_MAX_BG_WORKERS", timescaleTuneMaxBackgroundWorkers)
    .WithEnvironment("TS_TUNE_MAX_CONNS", timescaleTuneMaxConnections)
    .WithContainerRuntimeArgs(
        $"--shm-size={timescaleSharedMemorySize}",
        $"--ulimit=nofile={timescaleNofileLimit}:{timescaleNofileLimit}"
    );

var timescaledbDatabase = timescaledb.AddDatabase("timescaledb-direct", databaseName);
var timescaledbEndpoint = timescaledb.GetEndpoint("tcp");
var pgbouncerDatabaseUrls = databaseName.Equals("postgres", StringComparison.OrdinalIgnoreCase)
    ? ReferenceExpression.Create(
        $"postgres://{databaseUser}:{databasePassword}@{timescaledbEndpoint.Host}:{timescaledbEndpoint.Property(EndpointProperty.Port)}/{databaseName}"
    )
    : ReferenceExpression.Create(
        $"postgres://{databaseUser}:{databasePassword}@{timescaledbEndpoint.Host}:{timescaledbEndpoint.Property(EndpointProperty.Port)}/{databaseName},postgres://{databaseUser}:{databasePassword}@{timescaledbEndpoint.Host}:{timescaledbEndpoint.Property(EndpointProperty.Port)}/postgres"
    );

var pgbouncer = builder
    .AddContainer("pgbouncer", "edoburu/pgbouncer", "v1.25.1-p0")
    .WithEnvironment("DATABASE_URLS", pgbouncerDatabaseUrls)
    .WithEnvironment("AUTH_TYPE", pgbouncerAuthType)
    .WithEnvironment("POOL_MODE", "transaction")
    .WithEnvironment("MAX_CLIENT_CONN", pgbouncerMaxClientConnections)
    .WithEnvironment("DEFAULT_POOL_SIZE", pgbouncerDefaultPoolSize)
    .WithEnvironment("MIN_POOL_SIZE", pgbouncerMinPoolSize)
    .WithEnvironment("RESERVE_POOL_SIZE", pgbouncerReservePoolSize)
    .WithEnvironment("RESERVE_POOL_TIMEOUT", pgbouncerReservePoolTimeout)
    .WithEnvironment("MAX_DB_CONNECTIONS", pgbouncerMaxDatabaseConnections)
    .WithEnvironment("MAX_DB_CLIENT_CONNECTIONS", pgbouncerMaxDatabaseClientConnections)
    .WithEnvironment("QUERY_WAIT_TIMEOUT", pgbouncerQueryWaitTimeout)
    .WithEnvironment("SERVER_IDLE_TIMEOUT", pgbouncerServerIdleTimeout)
    .WithEnvironment("SERVER_LIFETIME", pgbouncerServerLifetime)
    .WithEnvironment("LISTEN_BACKLOG", pgbouncerListenBacklog)
    .WithEnvironment("SERVER_RESET_QUERY", "DISCARD ALL")
    .WithEnvironment("IGNORE_STARTUP_PARAMETERS", "extra_float_digits")
    .WithEnvironment("ADMIN_USERS", databaseUser)
    .WithEnvironment("STATS_USERS", databaseUser)
    .WithEndpoint(targetPort: 5432, port: 56432)
    .WithContainerRuntimeArgs(
        $"--ulimit=nofile={pgbouncerNofileLimit}:{pgbouncerNofileLimit}",
        $"--sysctl=net.core.somaxconn={pgbouncerSomaxconn}"
    )
    .WaitFor(timescaledbDatabase);

var pgbouncerEndpoint = pgbouncer.GetEndpoint("tcp");

var pooledConnectionString = builder.AddConnectionString(
    "timescaledb-pool",
    ReferenceExpression.Create(
        $"Host={pgbouncerEndpoint.Host};Port={pgbouncerEndpoint.Property(EndpointProperty.Port)};Database={databaseName};Username={databaseUser};Password={databasePassword};Pooling=true;Minimum Pool Size=20;Maximum Pool Size=200;Timeout=15;Command Timeout=30"
    )
);

builder
    .AddProject(
        "timescaledb-demo",
        "../PlatformKernel.TimescaleDbDemo/PlatformKernel.TimescaleDbDemo.csproj"
    )
    .WithEnvironment("TIMESCALE_CONNECTION_STRING", pooledConnectionString)
    .WithExplicitStart()
    .WaitFor(pgbouncer);

await builder.Build().RunAsync();
