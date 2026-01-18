using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides methods for managing and retrieving integration event outbox entities.
/// This service is responsible for handling operations related to publishing and retrieving
/// integration events that are stored within the outbox mechanism.
/// </summary>
public interface IIntegrationEventOutboxService : IBaseService<IntegrationEventOutbox>
{
    /// <summary>
    /// Retrieves unpublished integration events from the outbox for a specified service and pageable configuration.
    /// </summary>
    /// <param name="serviceName">
    /// Name of the service for which unpublished integration events are being retrieved.
    /// </param>
    /// <param name="pageable">
    /// An instance of <see cref="IPageable"/> that specifies the pagination details for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete. The default value is an uninitialized token.
    /// </param>
    /// <returns>
    /// An asynchronous operation that, when completed, yields a collection of <see cref="IntegrationEventOutbox"/> representing the unpublished events.
    /// </returns>
    Task<IEnumerable<IntegrationEventOutbox>> GetUnpublishedEventsAsync(
        string serviceName,
        IPageable pageable,
        CancellationToken cancellationToken = default
    );
}
