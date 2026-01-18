using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.UnitOfWork.Abstractions;
using OrderEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public class BookingAdjustPriceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOrderBookingService orderBookingService,
    IBookingReservationService bookingReservationService,
    IBookingCreateService bookingCreateService,
    IBookingOnlinePaymentService bookingOnlinePaymentService,
    ICacheService cacheService
) : CreateCommandHandlerBase<BookingAdjustPriceCommand, ReservationEntity>(unitOfWork, mapper)
{
    private const string ApiIssueCodeFormat = "LPY{0:D10}DxxxxxxxAAAAAA";

    protected override async Task<ReservationEntity> HandleAsync(
        BookingAdjustPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var bookingDate = DateTime.UtcNow;
        var existingReservation = request.ExistingReservation;
        var planQuestions = payload.PlanQuestions;
        var optionQuestions = payload.OptionsQuestions;

        var cancellationId = existingReservation.BookingData?.Plan.CancellationDataPolicy?.Id;

        var bookingCreateRequest = new BookingCreateRequest(
            existingReservation.BookingData?.LanguageCode ?? LanguageHeaderUtil.DefaultLanguageCode,
            existingReservation.FacilityId,
            existingReservation.SiteId,
            existingReservation.PlanId,
            existingReservation.RoomGroupId,
            existingReservation.CheckInDate,
            payload.CheckInTime,
            existingReservation.CheckOutTime,
            existingReservation.PaymentType,
            payload,
            existingReservation.FacilityRecordCode,
            planQuestions,
            optionQuestions
        )
        {
            BookingDate = bookingDate,
            ModifiedDateTime = bookingDate,
            PlanType = existingReservation.Plan?.PlanType ?? PlanTypes.Combo,
            UpdateCount = ++existingReservation.UpdateCount,
            SelectedCancellation = cancellationId,
            IsNotCheckValidDateLimit = request.IsNotCheckValidDateLimit
        };

        var externalRequest = new BookingExternalInfoRequest(
            existingReservation.UserCode,
            null
        );

        var newReservation = await bookingCreateService.CreateBookingAsync(
            bookingCreateRequest,
            externalRequest,
            existingReservation,
            cancellationToken
        );

        try
        {
            await bookingReservationService.AdjustWhenChangePriceOfReservationAsync(
                new ReservationEntity
                {
                    Id = existingReservation.Id,
                    ModifiedDateTime = DateTime.UtcNow,
                    ReservationState = request.ReservationState
                },
                false,
                cancellationToken
            );

            OrderReservation newReservationOfOrder;

            if (existingReservation.IsOnlinePayment)
            {
                var order = await bookingReservationService.GetOrderOfOnlinePaymentAsync(existingReservation.Id, cancellationToken);

                newReservationOfOrder = await orderBookingService.CreateAsync(
                    new OrderReservation
                    {
                        OrderId = order.Id,
                        Reservation = newReservation,
                        IsEnabled = true
                    },
                    false,
                    cancellationToken
                );

                await bookingOnlinePaymentService.OnlinePaymentChangeAmountAsync(
                    existingReservation.Id,
                    newReservation.BookingData?.AllTotalPrice ?? 0,
                    cancellationToken
                );
            }
            else
            {
                var orderCode = EntityUtil.CreateCode();
                var newOrder = new OrderEntity
                {
                    Code = orderCode,
                    OrderDateTime = DateTime.UtcNow,
                    ApiIssueCode = ConvertUtil.Format(ApiIssueCodeFormat, orderCode[..10]),
                    IsEnabled = true
                };

                newReservationOfOrder = await orderBookingService.CreateAsync(
                    new OrderReservation
                    {
                        Order = newOrder,
                        Reservation = newReservation,
                        IsEnabled = true
                    },
                    false,
                    cancellationToken
                );
            }

            await cacheService.RemoveByPatternsAsync(
                true,
                $"*{string.Format(CacheKeys.FacilityPrefixKey, newReservation.FacilityId)}*"
            );

            return newReservationOfOrder.Reservation!;
        }
        catch (Exception)
        {
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
