using Liberty.Hangfire;
using Liberty.Hangfire.Models;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.HostedServices;

public class IntegrationEventOutboxHostedService(
    ILogger<IntegrationEventOutboxHostedService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<IntegrationEventSetting> options
)
    : HostedServiceBase(
        logger,
        TimeSpan.FromMinutes(options.Value.PeriodTime),
        TimeSpan.FromMinutes(options.Value.DelayBeforeTime)
    )
{
    private readonly int _pageSize = options.Value.PageSize;
    private static readonly SemaphoreSlim Semaphore = new(1);

    protected override async Task HandlerExecuteAsync(
        object? state
    )
    {
        logger.LogInformation("Start background job {JobName}", nameof(IntegrationEventOutboxHostedService));

        await Semaphore.WaitAsync();

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var eventOutboxService = scope.ServiceProvider.GetRequiredService<IIntegrationEventOutboxService>();

            var integrationEvents = await eventOutboxService.GetUnpublishedEventsAsync(
                DefaultValues.ServiceNameOfManager,
                Pageable.Of(1, _pageSize)
            );

            var eventsToRemove = new List<IntegrationEventOutbox>();
            foreach (var eventOutbox in integrationEvents)
            {
                var request = new RegisterScheduleJobDelayRequest(
                    eventOutbox.EventName,
                    eventOutbox.JobName,
                    eventOutbox.JsonData,
                    TimeSpan.FromSeconds(1)
                );

                var isSuccess = await RegisterScheduleJobAsync(request);

                if (!isSuccess)
                {
                    continue;
                }

                eventsToRemove.Add(eventOutbox);

                await eventOutboxService.DeletePhysicalAsync(
                    eventOutbox.Id
                );
            }

            logger.LogInformation(
                "End background job {JobName}, {Count} events processed: {EventIds}",
                nameof(IntegrationEventOutboxHostedService),
                eventsToRemove.Count,
                string.Join(", ", eventsToRemove.Select(x => x.Id))
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{JobName} error occurred", nameof(IntegrationEventOutboxHostedService));
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async Task<bool> RegisterScheduleJobAsync(
        RegisterScheduleJobDelayRequest request
    )
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var externalApiService =
                scope.ServiceProvider.GetRequiredService<IExternalApiService>();

            logger.LogInformation(
                "{Action} Send Batch Job {EventName} start",
                nameof(IntegrationEventOutboxHostedService),
                request.EventName
            );

            var (_, context) = await externalApiService.PostAsync(
                ExternalService.BatchSchedulerService,
                RegisterJobEndpoint.RegisterScheduleJobDelayEndpoint,
                JsonConvert.SerializeObject(request)
            );

            if (!string.IsNullOrEmpty(context))
            {
                logger.LogInformation(
                    "{Action} Send Batch Job {EventName} successful",
                    nameof(IntegrationEventOutboxHostedService),
                    request.EventName
                );
                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} Send Batch Job {EventName} error {Message}",
                nameof(IntegrationEventOutboxHostedService),
                request.EventName,
                ex.Message
            );
        }

        return false;
    }
}
