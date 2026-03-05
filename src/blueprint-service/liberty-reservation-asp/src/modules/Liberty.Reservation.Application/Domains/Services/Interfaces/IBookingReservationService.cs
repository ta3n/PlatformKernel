using System.Linq.Expressions;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Templates;
using OrderEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

using ReservationEntity = Contexts.DataContexts.Entities.Data.Reservation;

public interface IBookingReservationService : IBaseService<ReservationEntity>
{
    Task<ReservationEntity> AdjustHeaderDataOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> AdjustWhenChangePriceOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> CancelAsync(
        ReservationEntity entityToUpdate,
        decimal cancellationPrice,
        float rateFee,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<long> UpdateBookingSendMailStateAsync(
        long id,
        BookingSendMailState sendMailState,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> ConfirmedAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> NoShowAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<IPage<ReservationEntity>> GetPageReminderCancelCheckInReservationsAsync(
        long facilityId,
        DateTime reminderDate,
        IPageable pageable,
        Expression<Func<ReservationEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    Task<IPage<ReservationEntity>> GetPageReminderUpComingCheckInReservationsAsync(
        long facilityId,
        long reminderDate,
        IPageable pageable,
        Expression<Func<ReservationEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    long CalculateReminderDateId(
        BookingData? bookingData,
        DateTime reminderDate
    );

    Task<string> FindOderIdOfOnlinePaymentAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );

    decimal GetCancellationPrice(
        DateTime cancelledDateTime,
        ReservationEntity reservation
    );

    Task<TemplateFormatData?> FindMailTemplateAsync(
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> ChangeLocationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsBookingHasFacilitySetOnlyOnlinePaymentMethodAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );

    BookingCancellationFeeResponse GetBookingCancellationFee(
        DateTime cancelledDateTime,
        ReservationEntity reservation
    );

    Task<ReservationEntity> UpdateBookingCancellationStatusAsync(
        ReservationEntity entityToUpdate,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the order associated with an online payment for a specific reservation.
    /// </summary>
    /// <param name="reservationId">
    /// The unique identifier of the reservation for which the online payment order is to be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the order entity
    /// associated with the online payment.
    /// </returns>
    Task<OrderEntity> GetOrderOfOnlinePaymentAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );

    Task<OrderEntity> GetOrderByReservationIdAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );
}
