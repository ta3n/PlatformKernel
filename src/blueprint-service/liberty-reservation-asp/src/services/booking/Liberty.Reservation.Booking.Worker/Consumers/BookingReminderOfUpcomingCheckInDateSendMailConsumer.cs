using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Newtonsoft.Json;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingReminderOfUpcomingCheckInDateSendMailConsumer(
    ILogger<BookingReminderOfUpcomingCheckInDateSendMailConsumer> logger,
    IMailService mailService,
    IBookingReservationService bookingReservationService
) : BaseConsumer<BookingReminderOfUpcomingCheckInDateSendMailEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingReminderOfUpcomingCheckInDateSendMailEvent> context
    )
    {
        var jsonContext = context.Message.JsonData ?? string.Empty;
        var message = JsonConvert.DeserializeObject<MessageDto>(
            jsonContext
        );
        if (message is null)
        {
            return;
        }

        logger.LogInformation(
            "{Action} {BookingId} to {To} subject {Subject} finished",
            nameof(BookingReminderOfUpcomingCheckInDateSendMailConsumer),
            message.BookingId,
            message.Tos,
            message.Subject
        );

        try
        {
            await mailService.SendAsync(
                message.Tos,
                message.Subject,
                message.Body
            );

            await bookingReservationService.UpdateBookingSendMailStateAsync(
                message.BookingId,
                BookingSendMailState.ReminderOfUpcomingCheckInDateSentMail
            );

            logger.LogInformation(
                "{Action} {BookingId} to {To} subject {Subject} finished",
                nameof(BookingReminderOfUpcomingCheckInDateSendMailConsumer),
                message.BookingId,
                message.Tos,
                message.Subject
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {BookingId} to {To} subject {Subject} failed: {Message}",
                nameof(BookingReminderOfUpcomingCheckInDateSendMailConsumer),
                message.BookingId,
                message.Tos,
                message.Subject,
                ex.Message
            );
        }
    }

    private sealed record MessageDto(
        long BookingId,
        string[] Tos,
        string Subject,
        string Body
    );
}
