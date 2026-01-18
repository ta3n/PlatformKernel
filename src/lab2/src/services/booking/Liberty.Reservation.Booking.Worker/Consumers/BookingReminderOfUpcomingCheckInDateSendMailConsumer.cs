using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Newtonsoft.Json;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingReminderOfUpcomingCheckInDateSendMailConsumer(
    ILogger<BookingReminderOfUpcomingCheckInDateSendMailConsumer> logger,
    IMailService mailService,
    IBookingReservationService bookingReservationService,
    IBookingAggregateMailAuditLogService bookingAggregateMailAuditLogService
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
            await SendEmailAsync(message);

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

    private async Task SendEmailAsync(
        MessageDto message
    )
    {
        try
        {
            await mailService.SendAsync(
                message.Tos,
                message.Subject,
                message.Body,
                message.FromDisplayName
            );

            logger.LogInformation(
                "Send mail successful to - {ToEmails}",
                string.Join(", ", message.Tos)
            );

            message.MailSentStatus = MailSentStatus.Success;
            message.IsReminderEmail = true;

            await SaveBookingMailAuditLogAsync(message);
        }
        catch
        {
            Console.WriteLine("Error when sending email");

            message.MailSentStatus = MailSentStatus.Failed;
            message.IsReminderEmail = true;

            await SaveBookingMailAuditLogAsync(message);

            throw;
        }
    }

    private async Task SaveBookingMailAuditLogAsync(
        MessageDto message,
        CancellationToken cancellationToken = default
    )
    {
        var emailAuditLog = new BookingAggregateMailAuditLog
        {
            TriggerSource = MailActorTypes.Reminder,
            Recipient = string.Join(";", message.Tos),
            Subject = message.Subject,
            Body = message.Body,
            MailSentStatus = message.MailSentStatus
        };

        await bookingAggregateMailAuditLogService.CreateAsync(
            emailAuditLog,
            true,
            cancellationToken
        );
    }

    private sealed record MessageDto(
        long BookingId,
        string[] Tos,
        string Subject,
        string Body,
        bool IsSendFax,
        string FaxNumber,
        string? FromDisplayName,
        MailActorTypes TriggerSource
    )
    {
        public MailSentStatus MailSentStatus { get; set; }
        public bool IsReminderEmail { get; set; } = false;
    };
}
