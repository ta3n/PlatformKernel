using Microsoft.Extensions.Options;
using SharedKernel.Hangfire.Test.Service.Models;
using SharedKernel.Hangfire.Test.Service.Options;
using StackExchange.Redis;

namespace SharedKernel.Hangfire.Test.Service.Services;

public sealed class ScenarioStateTracker
{
    private const string BootstrapEnterScript = """
        local current = redis.call('INCR', KEYS[1])
        local max = tonumber(redis.call('GET', KEYS[2]) or '0')
        if current > max then
            redis.call('SET', KEYS[2], current)
        end
        redis.call('INCR', KEYS[3])
        return current
        """;

    private readonly IConnectionMultiplexer _redis;
    private readonly int _databaseIndex;
    private readonly string _instanceId;
    private readonly string _statePrefix;

    public ScenarioStateTracker(
        IConnectionMultiplexer redis,
        IOptions<TestHangfireServiceOptions> options
    )
    {
        _redis = redis;
        _databaseIndex = options.Value.StateDatabase;
        _instanceId = InstanceIdentity.Resolve(options.Value.InstanceId);
        _statePrefix = options.Value.StatePrefix;
    }

    public Task RecordScheduledExecutionAsync(
        string scenarioId
    )
    {
        return RecordExecutionAsync(scenarioId, "scheduled");
    }

    public Task RecordRecurringExecutionAsync(
        string scenarioId
    )
    {
        return RecordExecutionAsync(scenarioId, "recurring");
    }

    public async Task RecordBootstrapEntryAsync(
        string scenarioId,
        TimeSpan holdDuration,
        CancellationToken cancellationToken
    )
    {
        var database = GetDatabase();

        await database.ScriptEvaluateAsync(
            BootstrapEnterScript,
            [
                BootstrapCurrentKey(scenarioId),
                BootstrapMaxKey(scenarioId),
                BootstrapEntryKey(scenarioId)
            ],
            []
        );
        await database.SetAddAsync(BootstrapInstancesKey(scenarioId), _instanceId);
        await ApplyKeyExpirationAsync(scenarioId);

        try
        {
            await Task.Delay(holdDuration, cancellationToken);
        }
        finally
        {
            await database.StringDecrementAsync(BootstrapCurrentKey(scenarioId));
        }
    }

    public async Task<ScenarioStateResponse> GetStateAsync(
        string scenarioId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var database = GetDatabase();
        var scheduledExecutionsTask = database.StringGetAsync(ExecutionCountKey(scenarioId, "scheduled"));
        var recurringExecutionsTask = database.StringGetAsync(ExecutionCountKey(scenarioId, "recurring"));
        var scheduledInstancesTask = database.SetMembersAsync(ExecutionInstancesKey(scenarioId, "scheduled"));
        var recurringInstancesTask = database.SetMembersAsync(ExecutionInstancesKey(scenarioId, "recurring"));
        var bootstrapEntryCountTask = database.StringGetAsync(BootstrapEntryKey(scenarioId));
        var bootstrapMaxConcurrencyTask = database.StringGetAsync(BootstrapMaxKey(scenarioId));
        var bootstrapInstancesTask = database.SetMembersAsync(BootstrapInstancesKey(scenarioId));

        await Task.WhenAll(
            scheduledExecutionsTask,
            recurringExecutionsTask,
            scheduledInstancesTask,
            recurringInstancesTask,
            bootstrapEntryCountTask,
            bootstrapMaxConcurrencyTask,
            bootstrapInstancesTask
        );

        return new ScenarioStateResponse(
            ScenarioId: scenarioId,
            ScheduledExecutions: ToInt32(scheduledExecutionsTask.Result),
            ScheduledInstances: ToStrings(scheduledInstancesTask.Result),
            RecurringExecutions: ToInt32(recurringExecutionsTask.Result),
            RecurringInstances: ToStrings(recurringInstancesTask.Result),
            BootstrapEntryCount: ToInt32(bootstrapEntryCountTask.Result),
            BootstrapMaxConcurrency: ToInt32(bootstrapMaxConcurrencyTask.Result),
            BootstrapInstances: ToStrings(bootstrapInstancesTask.Result)
        );
    }

    private async Task RecordExecutionAsync(
        string scenarioId,
        string jobType
    )
    {
        var database = GetDatabase();

        await database.StringIncrementAsync(ExecutionCountKey(scenarioId, jobType));
        await database.SetAddAsync(ExecutionInstancesKey(scenarioId, jobType), _instanceId);
        await ApplyKeyExpirationAsync(scenarioId);
    }

    private async Task ApplyKeyExpirationAsync(
        string scenarioId
    )
    {
        var database = GetDatabase();
        var expiration = TimeSpan.FromHours(24);

        await Task.WhenAll(
            database.KeyExpireAsync(ExecutionCountKey(scenarioId, "scheduled"), expiration),
            database.KeyExpireAsync(ExecutionCountKey(scenarioId, "recurring"), expiration),
            database.KeyExpireAsync(ExecutionInstancesKey(scenarioId, "scheduled"), expiration),
            database.KeyExpireAsync(ExecutionInstancesKey(scenarioId, "recurring"), expiration),
            database.KeyExpireAsync(BootstrapEntryKey(scenarioId), expiration),
            database.KeyExpireAsync(BootstrapCurrentKey(scenarioId), expiration),
            database.KeyExpireAsync(BootstrapMaxKey(scenarioId), expiration),
            database.KeyExpireAsync(BootstrapInstancesKey(scenarioId), expiration)
        );
    }

    private IDatabase GetDatabase()
    {
        return _redis.GetDatabase(_databaseIndex);
    }

    private RedisKey ExecutionCountKey(
        string scenarioId,
        string jobType
    )
    {
        return $"{GetScenarioPrefix(scenarioId)}execution:{jobType}:count";
    }

    private RedisKey ExecutionInstancesKey(
        string scenarioId,
        string jobType
    )
    {
        return $"{GetScenarioPrefix(scenarioId)}execution:{jobType}:instances";
    }

    private RedisKey BootstrapEntryKey(
        string scenarioId
    )
    {
        return $"{GetScenarioPrefix(scenarioId)}bootstrap:entries";
    }

    private RedisKey BootstrapCurrentKey(
        string scenarioId
    )
    {
        return $"{GetScenarioPrefix(scenarioId)}bootstrap:current";
    }

    private RedisKey BootstrapMaxKey(
        string scenarioId
    )
    {
        return $"{GetScenarioPrefix(scenarioId)}bootstrap:max";
    }

    private RedisKey BootstrapInstancesKey(
        string scenarioId
    )
    {
        return $"{GetScenarioPrefix(scenarioId)}bootstrap:instances";
    }

    private string GetScenarioPrefix(
        string scenarioId
    )
    {
        return $"{_statePrefix.TrimEnd(':')}:{scenarioId}:";
    }

    private static int ToInt32(
        RedisValue value
    )
    {
        return value.HasValue && int.TryParse(value.ToString(), out var parsedValue)
            ? parsedValue
            : 0;
    }

    private static string[] ToStrings(
        RedisValue[] values
    )
    {
        return values.Select(value => value.ToString())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
    }
}
