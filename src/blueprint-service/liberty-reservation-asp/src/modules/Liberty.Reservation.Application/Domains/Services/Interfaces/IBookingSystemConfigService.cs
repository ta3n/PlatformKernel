using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines the contract for managing booking system configuration settings.
/// Provides methods to interact with <see cref="SystemConfig"/> entities,
/// supporting operations like retrieval, creation, and updates of configurations.
/// </summary>
public interface IBookingSystemConfigService : IBaseService<SystemConfig>
{
    /// <summary>
    /// Retrieves the system configuration asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="SystemConfig"/> object containing the system configuration if available;
    /// otherwise, <c>null</c>.
    /// </returns>
    Task<SystemConfig?> GetSystemConfigAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> GetSystemConfigOnlinePaymenAsync(
        CancellationToken cancellationToken = default
    );
}
