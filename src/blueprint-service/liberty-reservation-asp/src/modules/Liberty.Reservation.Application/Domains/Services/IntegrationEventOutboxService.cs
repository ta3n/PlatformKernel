using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class IntegrationEventOutboxService(
    ILogger<IntegrationEventOutboxService> logger,
    IIntegrationEventOutboxRepository integrationEventOutboxRepository
) : BaseService<IntegrationEventOutbox>(logger, integrationEventOutboxRepository, new EventOutboxNotfoundException()),
    IIntegrationEventOutboxService
{
    public async Task<IEnumerable<IntegrationEventOutbox>> GetUnpublishedEventsAsync(
        string serviceName,
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = integrationEventOutboxRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.ServiceName == serviceName)
            .OrderByDescending(x => x.CreatedAt);

        var page = await queryable.UsePageableAsync(
            pageable,
            cancellationToken: cancellationToken
        );

        return page.Content;
    }
}
