using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingReservationRepository(
    DbContext dataContext
) : RepositoryBase<Contexts.DataContexts.Entities.Data.Reservation>(dataContext), IBookingReservationRepository
{
    public async Task<IEnumerable<ReservationRoomGroupAppDatePersonAgeType>?> GetAllPersonAgeTypesOfReservationAsync(
        long facilityId,
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Include(x => x.ReservationRoomGroupAppDatePersonAgeTypes)
            .Where(
                x => x.Facility!.Id == facilityId
            )
            .Where(x => x.Id == reservationId)
            .SelectMany(x => x.ReservationRoomGroupAppDatePersonAgeTypes!);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<ReservationRoomGroupAppDateOptionItem>> GetAllOptionItemsOfReservationAsync(
        long facilityId,
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Include(x => x.ReservationRoomGroupAppDateOptionItems)
            .Where(
                x => x.Facility!.Id == facilityId
            )
            .Where(x => x.Id == reservationId)
            .SelectMany(x => x.ReservationRoomGroupAppDateOptionItems!);

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }

    public async Task<IPage<BookingHistoryDataModel>> GetAllHistoriesByMainAsync(
        IEnumerable<long> bookingIds,
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Where(x => bookingIds.Contains(x.Id))
            .OrderByDescending(x => x.Id)
            .Select(
                reservation => new BookingHistoryDataModel
                {
                    Id = reservation.Id,
                    Code = reservation.Code,
                    Serial = reservation.Serial,
                    UpdatedCount = reservation.UpdateCount,
                    LastModifiedBy = reservation.UpdatedBy ?? reservation.CreatedBy,
                    LastModifiedDateTime = reservation.UpdatedAt ?? reservation.CreatedAt,
                    Basic = new BasicOfOfBookingDataModel
                    {
                        CheckInTime = reservation.CheckInTime,
                        NumberOfNights = reservation.RestNumber,
                        NumberOfRooms = reservation.RoomNumber,
                        Memo = reservation.Memo
                    },
                    Reserver = new CustomerOfBookingDataModel
                    {
                        Name = reservation.Reserver!.Name,
                        Kana = reservation.Reserver!.Kana,
                        EMail = reservation.Reserver!.EMail,
                        CountryCode = reservation.Reserver!.CountryCode,
                        PostCode = reservation.Reserver!.PostCode,
                        Address1 = reservation.Reserver!.Address1,
                        Address2 = reservation.Reserver!.Address2,
                        Address3 = reservation.Reserver!.Address3,
                        Phone = reservation.Reserver!.Phone,
                        Gender = reservation.Reserver!.Gender
                    },
                    MainUser = new GuestOfBookingDataModel
                    {
                        Name = reservation.MainUser!.Name,
                        Kana = reservation.MainUser!.Kana,
                        CountryCode = reservation.MainUser!.CountryCode,
                        PostCode = reservation.MainUser!.PostCode,
                        Address1 = reservation.MainUser!.Address1,
                        Address2 = reservation.MainUser!.Address2,
                        Address3 = reservation.MainUser!.Address3,
                        Phone = reservation.MainUser!.Phone,
                        Gender = reservation.MainUser!.Gender
                    },
                    BookingData = reservation.BookingData
                }
            );

        var page = await queryable.UsePageableAsync(
            pageable,
            cancellationToken: cancellationToken
        );

        return page;
    }
}
