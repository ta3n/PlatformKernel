using System.Data.Common;
using System.Linq.Expressions;
using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Templates;
using Liberty.SysException.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using OrderEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingReservationService(
    ILogger<BookingReservationService> logger,
    IBookingReservationRepository reservationRepository,
    IMailTemplateRepository mailTemplateRepository
) : BaseService<ReservationEntity>(logger, reservationRepository, new ReservationNotfoundException()), IBookingReservationService
{
    public Task<ReservationEntity> AdjustHeaderDataOfReservationAsync(
        ReservationEntity entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        return UpdateWithFindByActionAsync(
            entityToUpdate,
            autoSave,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.CheckInTime = updateEntity.CheckInTime;
                existingEntity.Memo = updateEntity.Memo;
                existingEntity.ModifiedDateTime = DateTime.UtcNow;
                existingEntity.UpdateCount += 1;

                existingEntity.Reserver ??= new CustomerInfo();
                existingEntity.Reserver.Name = updateEntity.Reserver!.Name;
                existingEntity.Reserver.Kana = updateEntity.Reserver.Kana;
                existingEntity.Reserver.EMail = updateEntity.Reserver.EMail;
                existingEntity.Reserver.PostCode = updateEntity.Reserver.PostCode;
                existingEntity.Reserver.Address1 = updateEntity.Reserver.Address1;
                existingEntity.Reserver.Address2 = updateEntity.Reserver.Address2;
                existingEntity.Reserver.Address3 = updateEntity.Reserver.Address3;
                existingEntity.Reserver.Phone = updateEntity.Reserver.Phone;
                existingEntity.Reserver.CountryCode = updateEntity.Reserver.CountryCode;
                existingEntity.Reserver.Gender = updateEntity.Reserver.Gender;

                existingEntity.IsSameMainUser = updateEntity.IsSameMainUser;
                if (!updateEntity.IsSameMainUser)
                {
                    existingEntity.MainUser ??= new CustomerInfo();
                    existingEntity.MainUser.Name = updateEntity.MainUser!.Name;
                    existingEntity.MainUser.Kana = updateEntity.MainUser.Kana;
                    existingEntity.MainUser.Gender = updateEntity.MainUser.Gender;
                    existingEntity.MainUser.BirthDay = updateEntity.MainUser.BirthDay;
                    existingEntity.MainUser.PostCode = updateEntity.MainUser.PostCode;
                    existingEntity.MainUser.Address1 = updateEntity.MainUser.Address1;
                    existingEntity.MainUser.Address2 = updateEntity.MainUser.Address2;
                    existingEntity.MainUser.Address3 = updateEntity.MainUser.Address3;
                    existingEntity.MainUser.Phone = updateEntity.MainUser.Phone;
                    existingEntity.MainUser.CountryCode = updateEntity.MainUser.CountryCode;
                }

                existingEntity.UseRoomUser = updateEntity.UseRoomUser;
                existingEntity.ReservationState = updateEntity.ReservationState;
                existingEntity = UpdateQuestion(entityToUpdate, existingEntity);

                if (updateEntity.ReservationPlanRoomGroupAppDates is null)
                {
                    return existingEntity;
                }

                foreach (var updatedItem in updateEntity.ReservationPlanRoomGroupAppDates)
                {
                    var existingItem = existingEntity.ReservationPlanRoomGroupAppDates?
                        .FirstOrDefault(x => x.RoomGroupIndex == updatedItem.RoomGroupIndex);

                    if (existingItem is null)
                    {
                        continue;
                    }

                    existingItem.CustomerInfo ??= new CustomerInfo();
                    existingItem.CustomerInfo.Name = updatedItem.CustomerInfo!.Name;
                    existingItem.CustomerInfo.Kana = updatedItem.CustomerInfo.Kana;
                    var appdate = existingEntity.BookingData!.AppDates!.Find(x => x.AppDateId == existingItem.BookingDateId);
                    var room = appdate!.Rooms.FirstOrDefault(x => x.RoomIndex == updatedItem.RoomGroupIndex);

                    room!.CustomerInfo = new CustomerData();
                    room.CustomerInfo!.Name = updatedItem.CustomerInfo.Name;
                    room.CustomerInfo.Kana = updatedItem.CustomerInfo.Kana;
                }

                return existingEntity;
            },
            async id =>
            {
                var queryable = GetQueryable()
                    .Include(x => x.Reserver)
                    .Include(x => x.MainUser)
                    .Include(x => x.ReservationPlanRoomGroupAppDates!)
                    .ThenInclude(t => t.CustomerInfo)
                    .Where(x => x.Id == id);
                var existingEntity = await queryable.SingleOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken
                    )
                    ?? throw new ReservationNotfoundException();

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
                existingEntity.ReservationState = entityToUpdate.ReservationState;
                existingEntity.ModifiedDateTime = updateEntity.ModifiedDateTime;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public Task<ReservationEntity> CancelAsync(
        ReservationEntity entityToUpdate,
        decimal cancellationPrice,
        float rateFee,
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
                existingEntity.CancellationPrice = cancellationPrice;
                existingEntity.CancellationFeeType = updateEntity.CancellationFeeType;
                existingEntity.ReservationState = updateEntity.ReservationState;
                existingEntity.CancelledDateTime = DateTime.UtcNow;
                existingEntity.ModifiedDateTime = DateTime.UtcNow;
                existingEntity.UpdateCount += 1;
                existingEntity.CancelRateFee = rateFee;
                return existingEntity;
            },
            cancellationToken
        );
    }

    public async Task<long> UpdateBookingSendMailStateAsync(
        long id,
        BookingSendMailState sendMailState,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var reservation = await reservationRepository
                    .GetQueryableWithAsNoTracking()
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync(cancellationToken)
                ?? throw new ReservationNotfoundException();

            switch (sendMailState)
            {
                case BookingSendMailState.ReminderOfUpcomingCheckInDateSendMail:
                    reservation.BookingData!.SendMailState.BookingReminderOfUpcomingCheckInDateSend = true;
                    break;
                case BookingSendMailState.ReminderOfUpcomingCheckInDateSentMail:
                    reservation.BookingData!.SendMailState.BookingReminderOfUpcomingCheckInDateSent = false;
                    break;
                case BookingSendMailState.CancellationFeeReminderSendMail:
                    reservation.BookingData!.SendMailState.BookingCancellationFeeReminderSend = true;
                    break;
                case BookingSendMailState.CancellationFeeReminderSentMail:
                    reservation.BookingData!.SendMailState.BookingCancellationFeeReminderSent = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sendMailState), sendMailState, null);
            }

            await reservationRepository.UpdateAsync(
                reservation,
                true,
                cancellationToken
            );

            return reservation.Id;
        }
        catch (DbException ex)
        {
            logger.LogError(ex, "Update send mail status failed: {Message}", ex.Message);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    public Task<ReservationEntity> UpdateBookingCancellationStatusAsync(
        ReservationEntity entityToUpdate,
        CancellationToken cancellationToken = default
    )
    {
        return base.UpdateAsync(
            entityToUpdate,
            true,
            (
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.CancellationStatus = updateEntity.CancellationStatus;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public async Task<IPage<ReservationEntity>> GetPageReminderCancelCheckInReservationsAsync(
        long facilityId,
        DateTime reminderDate,
        IPageable pageable,
        Expression<Func<ReservationEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        var reminderDateIdList = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(
                x => x.ReservationState == ReservationStatus.Confirmed
                    || x.ReservationState == ReservationStatus.Reserved
                    || x.ReservationState == ReservationStatus.Modified
            )
            .Select(
                r => new
                {
                    r.Id,
                    CheckInDateId = FindCalculateReminderDateId(r.BookingData, reminderDate)
                }
            )
            .ToListAsync(cancellationToken);

        var validReminderDateIds = reminderDateIdList
            .Select(x => x.Id)
            .ToList();

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Reserver)
            .Include(x => x.MainUser)
            .Include(x => x.ReservationPlanRoomGroupAppDates!)
            .ThenInclude(t => t.ReservationRoomGroupAppDatePersonAgeTypes!)
            .ThenInclude(t => t.PersonAgeType)
            .Include(x => x.Plan)
            .Where(x => validReminderDateIds.Contains(x.Id));

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        var result = await queryable.UsePageableAsync(
            pageable,
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<IPage<ReservationEntity>> GetPageReminderUpComingCheckInReservationsAsync(
        long facilityId,
        long reminderDate,
        IPageable pageable,
        Expression<Func<ReservationEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Reserver)
            .Include(x => x.MainUser)
            .Include(x => x.ReservationPlanRoomGroupAppDates!)
            .ThenInclude(t => t.ReservationRoomGroupAppDatePersonAgeTypes!)
            .ThenInclude(t => t.PersonAgeType)
            .Include(x => x.Plan)
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.CheckInDate == reminderDate)
            .Where(
                x => x.ReservationState == ReservationStatus.Confirmed
                    || x.ReservationState == ReservationStatus.Reserved
                    || x.ReservationState == ReservationStatus.Modified
            );

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        var result = await queryable.UsePageableAsync(
            pageable,
            cancellationToken: cancellationToken
        );

        return result;
    }

    private static long FindCalculateReminderDateId(
        BookingData? bookingData,
        DateTime reminderDate
    )
    {
        var plan = bookingData?.Plan ?? throw new InvalidOperationException("Plan not found in BookingData.");
        int addDays;
        if (plan.IsCancelSameAccept)
        {
            addDays = plan.ReceptionDayLimit is null or 0 ? 1 : plan.ReceptionDayLimit.Value;
        }
        else
        {
            addDays = plan.CancelDayLimit is null or 0 ? 1 : plan.CancelDayLimit.Value;
        }

        var result = AppDate.GetId(reminderDate.AddDays(addDays));

        return result;
    }

    public long CalculateReminderDateId(
        BookingData? bookingData,
        DateTime reminderDate
    )
    {
        return FindCalculateReminderDateId(bookingData, reminderDate);
    }

    public Task<ReservationEntity> ConfirmedAsync(
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
                existingEntity.ReservationState = updateEntity.ReservationState;
                existingEntity.ConfirmedDateTime = updateEntity.ConfirmedDateTime;
                existingEntity.ModifiedDateTime = updateEntity.ModifiedDateTime;
                existingEntity.PaymentType = updateEntity.PaymentType;
                existingEntity.UpdateCount = updateEntity.UpdateCount;

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
                existingEntity.NoShowDateTime = updateEntity.NoShowDateTime;
                existingEntity.IsNoShow = updateEntity.IsNoShow;
                existingEntity.NoShowReason = updateEntity.NoShowReason;
                existingEntity.ModifiedDateTime = DateTime.UtcNow;
                existingEntity.UpdateCount += 1;

                return existingEntity;
            },
            cancellationToken
        );
    }

    public async Task<string> FindOderIdOfOnlinePaymentAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.Id == reservationId)
                .Where(x => x.PaymentType == PaymentTypes.OnLinePayment)
                .Select(x => new { x.OrderReservations!.SingleOrDefault()!.Order!.ApiIssueCode })
                .SingleOrDefaultAsync(cancellationToken)
            ?? throw new ReservationInvalidException("OrderId online payment not found");

        return reservation.ApiIssueCode!;
    }

    public decimal GetCancellationPrice(
        DateTime cancelledDateTime,
        ReservationEntity reservation
    )
    {
        var reservationData = reservation.BookingData;

        var checkInDate = AppDate.GetDateTime(reservation.CheckInDate);
        var cancellationTargetPrice = CancellationTargetPrice(reservationData);
        var cancellationData =
            reservationData!.Plan.CancellationDataPolicy?.CancellationData
            ?? throw new ReservationCancellationDataNotFoundException();

        var totalDays = (checkInDate - cancelledDateTime).TotalDays;
        var days = (int)Math.Ceiling(totalDays);

        var availableCancellationData = cancellationData
            .FirstOrDefault(x => x.IsRange(days));

        if (availableCancellationData is null)
        {
            return 0;
        }

        var cancellationPrice = availableCancellationData.Calc(cancellationTargetPrice);

        return cancellationPrice;
    }

    public BookingCancellationFeeResponse GetBookingCancellationFee(
        DateTime cancelledDateTime,
        ReservationEntity reservation
    )
    {
        var reservationData = reservation.BookingData;
        var checkInDate = AppDate.GetDateTime(reservation.CheckInDate);

        var cancellationTargetPrice = CancellationTargetPrice(reservationData);

        var cancellationData = reservationData!.Plan.CancellationDataPolicy!.CancellationData
            ?? throw new ReservationCancellationDataNotFoundException();

        var totalDays = (checkInDate - cancelledDateTime).TotalDays;
        var days = (int)Math.Ceiling(totalDays);

        var availableCancellationData = cancellationData
            .FirstOrDefault(x => x.IsRange(days));

        if (availableCancellationData is null)
        {
            return new BookingCancellationFeeResponse(
                reservation.Id,
                cancellationTargetPrice,
                0,
                0,
                false
            );
        }

        var cancellationPrice = availableCancellationData.Calc(cancellationTargetPrice);

        return new BookingCancellationFeeResponse(
            reservation.Id,
            cancellationTargetPrice,
            cancellationPrice,
            availableCancellationData.Rate,
            true
        );
    }

    public async Task<TemplateFormatData?> FindMailTemplateAsync(
        CancellationToken cancellationToken = default
    )
    {
        var systemConfig = await mailTemplateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .FirstOrDefaultAsync(cancellationToken);

        var templateFormatData = systemConfig?.TemplateFormatData;

        return templateFormatData;
    }

    private static decimal CancellationTargetPrice(
        BookingData? reservationData
    )
    {
        return reservationData?.AllTotalPrice ?? 0;
    }

    public Task<ReservationEntity> ChangeLocationAsync(
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
                _
            ) =>
            {
                existingEntity.BookingData!.IsSiteLocation = entityToUpdate.BookingData!.IsSiteLocation;

                return existingEntity;
            },
            cancellationToken
        );
    }

    private static ReservationEntity UpdateQuestion(
        ReservationEntity entityToUpdate,
        ReservationEntity existingEntity
    )
    {
        foreach (
            var question in entityToUpdate.ReservationQuestions?
                .Where(x => x.ReservationQuestionType == ReservationQuestionTypes.Plan)
            ?? []
        )
        {
            var planQuestion = existingEntity.BookingData?.PlanQuestions?.Find(x => x.Id == question.QuestionId);
            if (planQuestion is not null)
            {
                planQuestion.AnswerData = question.AnswerData;
            }
        }

        foreach (var question in entityToUpdate.ReservationQuestions?.Where(
                x => x.ReservationQuestionType == ReservationQuestionTypes.OptionItem
            )
            ?? [])
        {
            var optionsQuestion = existingEntity.BookingData?.OptionQuestions?.Find(x => x.Id == question.QuestionId);
            if (optionsQuestion is not null)
            {
                optionsQuestion.AnswerData = question.AnswerData;
            }
        }

        return existingEntity;
    }

    public async Task<bool> IsBookingHasFacilitySetOnlyOnlinePaymentMethodAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.Id == reservationId
                    && x.Facility!.IsOnLinePayment
                    && x.Facility!.CanOnLinePayment
                    && !x.Facility!.IsOnSidePayment
            );

        var result = await queryable.AnyAsync(cancellationToken);

        return result;
    }

    public async Task<OrderEntity> GetOrderOfOnlinePaymentAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var order = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == reservationId)
            .Where(x => x.PaymentType == PaymentTypes.OnLinePayment)
            .Select(x => x.OrderReservations!.Select(or => or.Order).FirstOrDefault())
            .SingleOrDefaultAsync(cancellationToken);

        return order ?? throw new ReservationInvalidException("Order online payment not found");
    }

    public async Task<OrderEntity> GetOrderByReservationIdAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var order = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == reservationId)
            .Select(x => x.OrderReservations!.Select(or => or.Order).FirstOrDefault())
            .SingleOrDefaultAsync(cancellationToken);

        return order ?? throw new ReservationInvalidException("Order not found");
    }
}
