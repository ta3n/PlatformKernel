using Liberty.Reservation.Application.Domains.Services.Interfaces;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IReservationService : IBaseService<ReservationEntity>
{
    Task<ReservationEntity> AdjustHeaderDataOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> AdjustMetaDataOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> NoShowAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<ReservationEntity> CancelAsync(
        ReservationEntity entityToUpdate,
        decimal cancellationPrice,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<(long Id, ReservationStatus Status)> FindStatusByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<CancellationData>> GetCancellationDataAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ReservationRoomGroupAppDatePersonAgeType>?> GetAllPersonAgeTypesOfReservationAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ReservationRoomGroupAppDateOptionItem>> GetAllOptionItemsOfReservationAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    );
}
