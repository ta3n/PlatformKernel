using DbUp;
using DbUp.Engine;
using DbUp.Support;
using Microsoft.Extensions.Configuration;
using Serilog;
using Spectre.Console;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.ApplicationShared.Constants;
using SharedKernel.ApplicationShared.Extensions;
using SharedKernel.ApplicationShared.Settings;

var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(
        (
            hostingContext,
            config
        ) =>
        {
            // Configure application settings based on the environment
            var env = hostingContext.HostingEnvironment.EnvironmentName;

            config.AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();
        }
    )
    .ConfigureServices(
        (
            context,
            services
        ) =>
        {
            // Register configuration as a singleton service
            services.AddSingleton(context.Configuration);
        }
    )
    .Build();

try
{
    // Retrieve configuration and application information
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    var appInfo = configuration.GetOptionsExt<AppInfo>("App");
    var connectionString = configuration.GetConnectionString("DataContextConnection");

    // Log the connection string (for debugging purposes)
    Log.Information("Connecting to database: {ConnectionString}", connectionString);

    // Display application name and version using ASCII art
    AnsiConsole.Write(
        new FigletText($"{appInfo.AppName} v{appInfo.AppVersion}")
            .Color(Color.MediumPurple)
    );

    Log.Information("Starting migration");

    // Ensure the database exists if running in a local environment
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
    }

    // Configure the DbUp upgrade builder
    var upgradeBuilder = DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .WithVariablesDisabled()
        .LogToConsole()
        .LogScriptOutput();

    var scriptFolderPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "SqlScripts"
    );

    upgradeBuilder.WithScriptsFromFileSystem(
        scriptFolderPath,
        new SqlScriptOptions
        {
            RunGroupOrder = 1,
            ScriptType = ScriptType.RunOnce
        }
    );

    upgradeBuilder.WithScriptsFromFileSystem(
        Path.Combine(scriptFolderPath, "Views"),
        new SqlScriptOptions
        {
            RunGroupOrder = 2,
            ScriptType = ScriptType.RunOnce
        }
    );

    upgradeBuilder.WithScriptsFromFileSystem(
        Path.Combine(scriptFolderPath, "StoreProduces"),
        new SqlScriptOptions
        {
            RunGroupOrder = 3,
            ScriptType = ScriptType.RunOnce
        }
    );

    var customFolder = Path.Combine(scriptFolderPath, "Custom");
    if (Directory.Exists(customFolder))
    {
        upgradeBuilder.WithScriptsFromFileSystem(
            customFolder,
            new SqlScriptOptions
            {
                RunGroupOrder = 4,
                ScriptType = ScriptType.RunOnce
            }
        );
    }

    // Build the upgrade engine and perform the migration
    var upgradeEngine = upgradeBuilder.LogToAutodetectedLog().Build();
    var result = upgradeEngine.PerformUpgrade();

    if (!result.Scripts.Any())
    {
        Log.Information("No migration scripts have been executed on the current database.");
    }

    foreach (var script in result.Scripts)
    {
        var scriptName = $"Script run is : {script}";
        Log.Information(scriptName);
    }

    // Handle migration results
    if (!result.Successful)
    {
        // Log and display error if migration fails
        AnsiConsole.MarkupLine($"[red]Database migration error: {result.Error}[/]");
        Environment.Exit(ServiceErrorCode.Failed);
    }

    // Log and display success message if migration completes successfully
    AnsiConsole.MarkupLine("[bold green]Database migration completed successfully![/]");
    Environment.Exit(ServiceErrorCode.Success);
}
catch (Exception ex)
{
    // Log fatal errors and exit with failure code
    Log.Fatal(ex, "Host terminated unexpectedly");
    Environment.Exit(ServiceErrorCode.Failed);
}
finally
{
    // Ensure Serilog flushes all logs before exiting
    await Log.CloseAndFlushAsync();
}
