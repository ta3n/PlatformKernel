using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingCreateService(
    IMapper mapper,
    IBookingDataAvailableService bookingDataAvailableService,
    IBookingRoomAppDateService bookingRoomAppDateService,
    IBookingReservationPriceDataService bookingReservationPriceDataService,
    IBookingReservationOptionItemDataService bookingReservationOptionItemDataService
) : IBookingCreateService
{
    public async Task<ReservationEntity> CreateBookingAsync(
        BookingCreateRequest bookingCreateRequest,
        ReservationEntity existingReservation,
        CancellationToken cancellationToken = default
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var editHeaderDataOfReservation = mapper.Map<ReservationEntity>(bookingAdjustRequest);
        var editMetaDataOfReservation = mapper.Map<ReservationEntity>(bookingAdjustRequest);

        var newReservation = existingReservation.Clone<ReservationEntity>();

        newReservation.Id = 0;
        newReservation.FacilityId = bookingCreateRequest.FacilityId;
        newReservation.Code = EntityUtil.CreateCode();
        newReservation.ParentId = existingReservation.Id;

        newReservation.RestNumber = editHeaderDataOfReservation.RestNumber;
        newReservation.RoomNumber = editHeaderDataOfReservation.RoomNumber;
        newReservation.CheckInTime = editHeaderDataOfReservation.CheckInTime;
        newReservation.IsSameMainUser = editHeaderDataOfReservation.IsSameMainUser;
        newReservation.UseRoomUser = editHeaderDataOfReservation.UseRoomUser;
        newReservation.Memo = editHeaderDataOfReservation.Memo;

        var updateMetaData = editMetaDataOfReservation.ReservationData!;
        var newMetaData = newReservation.ReservationData!;

        newMetaData.CheckInTime = updateMetaData.CheckInTime;
        newMetaData.Reserver = updateMetaData.Reserver;
        newMetaData.IsSameMainUser = updateMetaData.IsSameMainUser;
        newMetaData.MainUser = updateMetaData.MainUser;
        newMetaData.Memo = updateMetaData.Memo;

        newReservation.ReservationData = newMetaData;

        newReservation.ReservationPlanRoomGroupAppDates = [];

        newReservation = await GetReservationDataAsync(
            bookingCreateRequest,
            newReservation,
            existingReservation,
            cancellationToken
        );

        return newReservation;
    }

    private async Task<ReservationEntity> GetReservationDataAsync(
        BookingCreateRequest bookingCreateRequest,
        ReservationEntity newReservation,
        ReservationEntity existingReservation,
        CancellationToken cancellationToken = default
    )
    {
        var bookingDate = DateTime.UtcNow;
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var bookingCheckAvailable = await bookingDataAvailableService.GetDataAvailableAsync(
            bookingCreateRequest,
            cancellationToken
        );

        var bookingRoomAppDates = (await bookingRoomAppDateService.GetAllRoomAppDates(
            bookingCreateRequest,
            bookingCheckAvailable,
            cancellationToken
        )).ToList();

        var bookingReservationPriceData = bookingReservationPriceDataService.GetAllReservationPriceData(
                bookingCreateRequest,
                bookingCheckAvailable,
                bookingRoomAppDates
            )
            .ToList();

        var bookingReservationOptionItemData = bookingReservationOptionItemDataService
            .GetAllReservationOptionItemData(
                bookingCreateRequest,
                bookingCheckAvailable,
                bookingRoomAppDates
            )
            .ToList();

        for (var nightIndex = 0; nightIndex < bookingAdjustRequest.NumberOfNights; nightIndex++)
        {
            var appDateId = AppDate.GetId(AppDate.GetDateTime(existingReservation.CheckInDate).AddDays(nightIndex));

            for (var roomIndex = 0; roomIndex < bookingAdjustRequest.NumberOfRooms; roomIndex++)
            {
                var appDateOfPlanRoomInReservation = CreateReservationPlanRoomGroupAppDate(
                    appDateId,
                    nightIndex,
                    roomIndex,
                    newReservation,
                    bookingCreateRequest
                );

                appDateOfPlanRoomInReservation.ReservationRoomGroupAppDatePersonAgeTypes = GetReservationPriceData(
                        appDateId,
                        nightIndex,
                        roomIndex,
                        newReservation,
                        existingReservation,
                        bookingReservationPriceData
                    )
                    .ToList();

                appDateOfPlanRoomInReservation.ReservationRoomGroupAppDateOptionItems = GetReservationOptionItemData(
                        appDateId,
                        nightIndex,
                        roomIndex,
                        newReservation,
                        existingReservation,
                        bookingReservationOptionItemData
                    )
                    .ToList();

                newReservation.ReservationPlanRoomGroupAppDates!.Add(appDateOfPlanRoomInReservation);
            }
        }

        await newReservation.ReserveAsync(bookingDate);

        return newReservation;
    }

    private ReservationPlanRoomGroupAppDate CreateReservationPlanRoomGroupAppDate(
        long appDateId,
        int nightIndex,
        int roomIndex,
        ReservationEntity newReservation,
        BookingCreateRequest bookingCreateRequest
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var appDateOfPlanRoomInReservation = new ReservationPlanRoomGroupAppDate
        {
            Reservation = newReservation,
            PlanId = bookingCreateRequest.PlanId,
            RoomGroupId = bookingCreateRequest.RoomGroupId,
            AppDateId = appDateId,
            RestIndex = nightIndex,
            RoomGroupIndex = roomIndex,
            ReservationRoomGroupAppDatePersonAgeTypes = [],
            ReservationRoomGroupAppDateOptionItems = []
        };

        var roomRepresentative = bookingAdjustRequest
            .RoomRepresentatives?
            .FirstOrDefault(x => x.RoomIndex == roomIndex);
        if (roomRepresentative != null)
        {
            appDateOfPlanRoomInReservation.UserInfo = new()
            {
                Name = roomRepresentative.FullName,
                Kana = roomRepresentative.Kana
            };
        }

        return appDateOfPlanRoomInReservation;
    }

    private IEnumerable<ReservationRoomGroupAppDatePersonAgeType> GetReservationPriceData(
        long appDateId,
        int nightIndex,
        int roomIndex,
        ReservationEntity newReservation,
        ReservationEntity existingReservation,
        List<BookingReservationPriceData> bookingReservationPriceData
    )
    {
        var data = new List<ReservationRoomGroupAppDatePersonAgeType>();

        var reservationPriceData = bookingReservationPriceData
            .Where(a => a.AppDateId == appDateId)
            .Where(a => a.RoomGroupIndex == roomIndex);

        foreach (var priceData in reservationPriceData)
        {
            if (priceData.IsPersonsMatch != null && !priceData.IsPersonsMatch.Value)
            {
                throw new ReservationNoMatchPersonsException();
            }

            var reservationRoomGroupAppDatePersonAgeType = new ReservationRoomGroupAppDatePersonAgeType
            {
                Reservation = newReservation,
                RoomGroupId = existingReservation.RoomGroupId,
                AppDateId = appDateId,
                PersonAgeTypeId = priceData.PersonAgeType?.Id ?? 0,
                RestIndex = nightIndex,
                RoomGroupIndex = roomIndex,
                UnitPrice = priceData.Price ?? 0,
                SpaTax = priceData.SpaTax ?? 0,
                MaleNumber = priceData.MalePersons ?? 0,
                FemaleNumber = priceData.FemalePersons ?? 0,
                GenderNoneNumber = priceData.NonePersons ?? 0
            };

            data.Add(reservationRoomGroupAppDatePersonAgeType);
        }

        return data;
    }

    private IEnumerable<ReservationRoomGroupAppDateOptionItem> GetReservationOptionItemData(
        long appDateId,
        int nightIndex,
        int roomIndex,
        ReservationEntity newReservation,
        ReservationEntity existingReservation,
        List<BookingReservationOptionItemData> bookingReservationOptionItemData
    )
    {
        var data = new List<ReservationRoomGroupAppDateOptionItem>();

        var reservationOptionItemData = bookingReservationOptionItemData
            .Where(a => a.AppDateId == appDateId)
            .Where(a => a.RoomGroupIndex == roomIndex);

        foreach (var optionItemData in reservationOptionItemData)
        {
            var reservationRoomAppDateOptionItem = new ReservationRoomGroupAppDateOptionItem
            {
                Reservation = newReservation,
                RoomGroupId = existingReservation.RoomGroupId,
                AppDateId = appDateId,
                OptionItemId = optionItemData.OptionItemInfo?.Id ?? 0,
                RestIndex = nightIndex,
                RoomGroupIndex = roomIndex,
                Price = optionItemData.Price,
                Number = optionItemData.Number ?? 0
            };

            data.Add(reservationRoomAppDateOptionItem);
        }

        return data;
    }
}
