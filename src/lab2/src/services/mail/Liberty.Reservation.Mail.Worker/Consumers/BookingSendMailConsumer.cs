using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Fax.Services.Interfaces;
using Liberty.Reservation.Mail.Worker.Application;
using Liberty.Reservation.Mail.Worker.Application.Services;
using Liberty.Reservation.Mail.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Newtonsoft.Json;

namespace Liberty.Reservation.Mail.Worker.Consumers;

public class BookingSendMailConsumer(
    ILogger<BookingSendMailConsumer> logger,
    IMailService mailService,
    IFaxService faxService,
    IConvertHtmlToAsciiService convertHtmlToAsciiService,
    IBus bus
) : BaseConsumer<BookingSendMailEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingSendMailEvent> context
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
            "Start send mail - {EventName} with booking id {BookingId}",
            context.Message.EventName,
            message.BookingId
        );

        await SendEmailAsync(message);

        if (message.IsSendFax)
        {
            await SendFaxAsync(message);
        }
    }

    private async Task SendFaxAsync(
        MessageDto message
    )
    {
        if (string.IsNullOrEmpty(message.FaxNumber))
        {
            return;
        }

        try
        {
            var body = $"""
                    {message.Subject}

                    {convertHtmlToAsciiService.ConvertAndReplaceTables(message.Body)}
                """;
            var faxResponse = await faxService.SendFaxAsync(
                message.BookingId,
                message.FaxNumber,
                message.Subject,
                body
            );

            var eventPayload = new BookingAggregateFaxAuditLogEvent();
            eventPayload.SerializeJsonData(
                new
                {
                    message.FaxNumber,
                    message.Subject,
                    Result = faxResponse?.Result ?? string.Empty,
                    IsSuccess = faxResponse?.IsSuccess ?? false,
                    message.BookingId,
                    message.Body
                }
            );

            await bus.Send(eventPayload);

            if (faxResponse is null)
            {
                return;
            }

            if (faxResponse.IsSuccess)
            {
                logger.LogInformation(
                    "Successfully sent fax for booking {BookingId} to {FaxNumber}",
                    message.BookingId,
                    message.FaxNumber
                );
            }
            else
            {
                logger.LogError(
                    "Failed to send fax for booking {BookingId} to {FaxNumber}. Error: {ErrorMessage}",
                    message.BookingId,
                    message.FaxNumber,
                    faxResponse.Result
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to send fax for booking {BookingId} to {FaxNumber}. Error: {ErrorMessage}",
                message.BookingId,
                message.FaxNumber,
                ex.Message
            );
        }
    }

    private async Task SendEmailAsync(
        MessageDto message
    )
    {
        var eventPayload = new EmailSentAuditLogEvent();

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
            eventPayload.SerializeJsonData(message);
            await bus.Send(eventPayload);
        }
        catch
        {
            Console.WriteLine("Error when sending email");

            message.MailSentStatus = MailSentStatus.Failed;
            eventPayload.SerializeJsonData(message);

            await bus.Send(eventPayload);

            throw;
        }
    }

    private sealed record MessageDto(
        long BookingId,
        string[] Tos,
        string Subject,
        string Body,
        bool IsSendFax,
        string FaxNumber,
        string? FromDisplayName,
        MailActorTypes TriggerSource,
        string? EventType
    )
    {
        public MailSentStatus MailSentStatus { get; set; }
        public bool IsReminderEmail { get; set; } = false;
    }
}
