using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class ReservationService(
    ILogger<ReservationService> logger,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : BaseService<ReservationEntity>(logger, reservationRepository, new ReservationNotfoundException()),
    IReservationService
{
    protected override IQueryable<ReservationEntity> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.Facility!.Id == facilityId
            );
    }

    public Task<ReservationEntity> AdjustHeaderDataOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.ModifiedDateTime = updateEntity.ModifiedDateTime;
                existingEntity.RestNumber = updateEntity.RestNumber;
                existingEntity.RoomNumber = updateEntity.RoomNumber;
                existingEntity.CheckInTime = updateEntity.CheckInTime;
                existingEntity.IsSameMainUser = updateEntity.IsSameMainUser;
                existingEntity.UseRoomUser = updateEntity.UseRoomUser;
                existingEntity.Memo = updateEntity.Memo;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public Task<ReservationEntity> AdjustMetaDataOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                var updateMetaData = updateEntity.ReservationData!;
                var existingMetaData = existingEntity.ReservationData!;

                existingMetaData.CheckInTime = updateMetaData.CheckInTime;
                existingMetaData.Reserver = updateMetaData.Reserver;
                existingMetaData.IsSameMainUser = updateMetaData.IsSameMainUser;
                existingMetaData.MainUser = updateMetaData.MainUser;
                existingMetaData.Memo = updateMetaData.Memo;

                existingEntity.ReservationData = existingMetaData;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public Task<ReservationEntity> AdjustWhenChangePriceOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.ReservationState = ReservationStatus.Modifying;
                existingEntity.ModifiedDateTime = updateEntity.ModifiedDateTime;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public Task<ReservationEntity> NoShowAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.ReservationState = ReservationStatus.NoShow;
                existingEntity.NoShowReason = updateEntity.NoShowReason;
                existingEntity.NoShowDateTime = updateEntity.NoShowDateTime;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public Task<ReservationEntity> CancelAsync(
        ReservationEntity entityToUpdate,
        decimal cancellationPrice,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.ReservationState = ReservationStatus.Canceled;
                existingEntity.CancellationPrice = cancellationPrice;
                existingEntity.CancelledDateTime = updateEntity.CancelledDateTime;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public async Task<(long Id, ReservationStatus Status)> FindStatusByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Where(x => x.Id == id)
            .Select(
                x => new
                {
                    x.Id,
                    Status = x.ReservationState
                }
            );

        var data = await queryable.SingleOrDefaultAsync(
            cancellationToken
        ) ?? throw new ReservationNotfoundException();

        return (data.Id, data.Status);
    }

    public async Task<IEnumerable<CancellationData>> GetCancellationDataAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Include(x => x.Plan)
            .ThenInclude(x => x!.Cancellation)
            .ThenInclude(x => x!.CancellationCancellationDatas)!
            .ThenInclude(x => x.CancellationData)
            .Where(x => x.Id == id)
            .SelectMany(x => x.Plan!.Cancellation!.CancellationCancellationDatas!)
            .Select(x => x.CancellationData!);

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }

    public async Task<IEnumerable<ReservationRoomGroupAppDatePersonAgeType>?> GetAllPersonAgeTypesOfReservationAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Include(x => x.ReservationRoomGroupAppDatePersonAgeTypes)
            .Where(x => x.Id == reservationId)
            .SelectMany(x => x.ReservationRoomGroupAppDatePersonAgeTypes!);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<ReservationRoomGroupAppDateOptionItem>> GetAllOptionItemsOfReservationAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Include(x => x.ReservationRoomGroupAppDateOptionItems)
            .Where(x => x.Id == reservationId)
            .SelectMany(x => x.ReservationRoomGroupAppDateOptionItems!);

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }
}
