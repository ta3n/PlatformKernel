using System.Net.Http.Json;
using SharedKernel.Hangfire.Test.Integration;
using SharedKernel.Hangfire.Test.Service.Models;
using Xunit.Sdk;

namespace SharedKernel.Hangfire.Test;

[Collection(HangfireRedisCollection.Name)]
public sealed class HangfireScaleOutIntegrationTests
{
    private readonly HangfireRedisContainerFixture _fixture;

    public HangfireScaleOutIntegrationTests(
        HangfireRedisContainerFixture fixture
    )
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ScheduledJob_ExecutesOnceAcrossScaledInstances()
    {
        await _fixture.ResetAsync();

        var testId = CreateTestId("scheduled");
        await using var instanceA = _fixture.CreateFactory(
            instanceId: "instance-a",
            hangfirePrefix: $"{testId}:hangfire:",
            statePrefix: $"{testId}:state:"
        );
        await using var instanceB = _fixture.CreateFactory(
            instanceId: "instance-b",
            hangfirePrefix: $"{testId}:hangfire:",
            statePrefix: $"{testId}:state:"
        );
        using var clientA = instanceA.CreateClient();
        using var clientB = instanceB.CreateClient();

        await WaitForHealthyAsync(clientA);
        await WaitForHealthyAsync(clientB);

        var scenarioId = $"{testId}:scenario";
        using var response = await clientA.PostAsJsonAsync(
            $"/tests/scenarios/{scenarioId}/jobs/scheduled",
            new ScheduleJobRequest(DelayMilliseconds: 1500)
        );
        response.EnsureSuccessStatusCode();

        var state = await WaitForStateAsync(
            clientB,
            scenarioId,
            timeout: TimeSpan.FromSeconds(45),
            isComplete: snapshot => snapshot.ScheduledExecutions == 1,
            isFailure: snapshot => snapshot.ScheduledExecutions > 1,
            failureMessage: snapshot => $"Scheduled job executed {snapshot.ScheduledExecutions} times."
        );

        Assert.Equal(1, state.ScheduledExecutions);
        Assert.Single(state.ScheduledInstances);

        await Task.Delay(TimeSpan.FromSeconds(3));

        var finalState = await GetStateAsync(clientA, scenarioId);
        Assert.Equal(1, finalState.ScheduledExecutions);
    }

    [Fact]
    public async Task RecurringJob_ExecutesOnceAcrossScaledInstances()
    {
        await _fixture.ResetAsync();

        var testId = CreateTestId("recurring");
        await using var instanceA = _fixture.CreateFactory(
            instanceId: "instance-a",
            hangfirePrefix: $"{testId}:hangfire:",
            statePrefix: $"{testId}:state:"
        );
        await using var instanceB = _fixture.CreateFactory(
            instanceId: "instance-b",
            hangfirePrefix: $"{testId}:hangfire:",
            statePrefix: $"{testId}:state:"
        );
        using var clientA = instanceA.CreateClient();
        using var clientB = instanceB.CreateClient();

        await WaitForHealthyAsync(clientA);
        await WaitForHealthyAsync(clientB);

        var scenarioId = $"{testId}:scenario";
        using var response = await clientA.PostAsync(
            $"/tests/scenarios/{scenarioId}/jobs/recurring/next-minute",
            content: null
        );
        response.EnsureSuccessStatusCode();

        var registration = await response.Content.ReadFromJsonAsync<RecurringJobRegistrationResponse>();
        Assert.NotNull(registration);

        var timeout = registration!.ScheduledForUtc - DateTimeOffset.UtcNow + TimeSpan.FromSeconds(75);
        if (timeout < TimeSpan.FromSeconds(30))
        {
            timeout = TimeSpan.FromSeconds(30);
        }

        var state = await WaitForStateAsync(
            clientB,
            scenarioId,
            timeout,
            isComplete: snapshot => snapshot.RecurringExecutions == 1,
            isFailure: snapshot => snapshot.RecurringExecutions > 1,
            failureMessage: snapshot => $"Recurring job executed {snapshot.RecurringExecutions} times."
        );

        Assert.Equal(1, state.RecurringExecutions);
        Assert.Single(state.RecurringInstances);

        await Task.Delay(TimeSpan.FromSeconds(5));

        var finalState = await GetStateAsync(clientA, scenarioId);
        Assert.Equal(1, finalState.RecurringExecutions);
    }

    [Fact]
    public async Task BootstrapLock_SerializesAcrossScaledInstances()
    {
        await _fixture.ResetAsync();

        var testId = CreateTestId("bootstrap");
        await using var instanceA = _fixture.CreateFactory(
            instanceId: "instance-a",
            hangfirePrefix: $"{testId}:hangfire:",
            statePrefix: $"{testId}:state:"
        );
        await using var instanceB = _fixture.CreateFactory(
            instanceId: "instance-b",
            hangfirePrefix: $"{testId}:hangfire:",
            statePrefix: $"{testId}:state:"
        );
        using var clientA = instanceA.CreateClient();
        using var clientB = instanceB.CreateClient();

        await WaitForHealthyAsync(clientA);
        await WaitForHealthyAsync(clientB);

        var scenarioId = $"{testId}:scenario";
        var request = new BootstrapLockRequest(HoldMilliseconds: 1500);

        await Task.WhenAll(
            clientA.PostAsJsonAsync($"/tests/scenarios/{scenarioId}/bootstrap/run", request),
            clientB.PostAsJsonAsync($"/tests/scenarios/{scenarioId}/bootstrap/run", request)
        );

        var state = await WaitForStateAsync(
            clientA,
            scenarioId,
            timeout: TimeSpan.FromSeconds(15),
            isComplete: snapshot => snapshot.BootstrapEntryCount == 2,
            isFailure: snapshot => snapshot.BootstrapMaxConcurrency > 1,
            failureMessage: snapshot => $"Bootstrap lock allowed max concurrency {snapshot.BootstrapMaxConcurrency}."
        );

        Assert.Equal(2, state.BootstrapEntryCount);
        Assert.Equal(1, state.BootstrapMaxConcurrency);
        Assert.Equal(["instance-a", "instance-b"], state.BootstrapInstances);
    }

    private static async Task WaitForHealthyAsync(
        HttpClient client
    )
    {
        var timeoutAt = DateTimeOffset.UtcNow.AddSeconds(15);

        while (DateTimeOffset.UtcNow < timeoutAt)
        {
            using var response = await client.GetAsync("/health");
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250));
        }

        throw new XunitException("Service did not become healthy within 15 seconds.");
    }

    private static async Task<ScenarioStateResponse> WaitForStateAsync(
        HttpClient client,
        string scenarioId,
        TimeSpan timeout,
        Func<ScenarioStateResponse, bool> isComplete,
        Func<ScenarioStateResponse, bool> isFailure,
        Func<ScenarioStateResponse, string> failureMessage
    )
    {
        var timeoutAt = DateTimeOffset.UtcNow.Add(timeout);

        while (DateTimeOffset.UtcNow < timeoutAt)
        {
            var state = await GetStateAsync(client, scenarioId);

            if (isFailure(state))
            {
                throw new XunitException(failureMessage(state));
            }

            if (isComplete(state))
            {
                return state;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        var finalState = await GetStateAsync(client, scenarioId);
        throw new XunitException(
            $"Timed out waiting for scenario '{scenarioId}'. Final snapshot: {System.Text.Json.JsonSerializer.Serialize(finalState)}"
        );
    }

    private static async Task<ScenarioStateResponse> GetStateAsync(
        HttpClient client,
        string scenarioId
    )
    {
        var state = await client.GetFromJsonAsync<ScenarioStateResponse>(
            $"/tests/scenarios/{scenarioId}"
        );

        return state ?? throw new XunitException("Scenario state response was null.");
    }

    private static string CreateTestId(
        string prefix
    )
    {
        return $"{prefix}:{Guid.NewGuid():N}";
    }
}
