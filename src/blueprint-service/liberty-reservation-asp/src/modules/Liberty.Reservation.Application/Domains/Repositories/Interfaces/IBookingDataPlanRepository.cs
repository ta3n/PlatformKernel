using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Interface for accessing booking data related to plans.
/// Provides methods to retrieve and paginate booking plans.
/// </summary>
public interface IBookingDataPlanRepository : IRepositoryBase<Plan>
{
    /// <summary>
    /// Retrieves a plan by its facility ID and plan ID.
    /// </summary>
    /// <param name="facilityId">The ID of the facility to filter the results.</param>
    /// <param name="id">The ID of the plan to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the <see cref="Plan"/> if found, or <c>null</c> if no matching plan exists.
    /// </returns>
    Task<Plan?> GetByIdAsync(
        long facilityId,
        long id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a paginated list of booking plans based on the provided filter and pagination parameters.
    /// </summary>
    /// <param name="filter">The filter criteria for booking plans.</param>
    /// <param name="pageable">The pagination parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a paginated collection of <see cref="BookingPlanModel"/>.
    /// </returns>
    Task<IPage<BookingPlanModel>> GetPageBookingPlansAsync(
        BookingDataPlanFilterParameter filter,
        IPageable pageable,
        CancellationToken cancellationToken
    );
}
