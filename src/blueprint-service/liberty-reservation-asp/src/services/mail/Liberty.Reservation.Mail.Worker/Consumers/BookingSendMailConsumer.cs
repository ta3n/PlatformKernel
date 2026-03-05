using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Fax.Services;
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
    IConvertHtmlToAsciiService convertHtmlToAsciiService
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
        await mailService.SendAsync(
            message.Tos,
            message.Subject,
            message.Body
        );

        logger.LogInformation(
            "Send mail successful to - {ToEmails}",
            string.Join(", ", message.Tos)
        );
    }

    private sealed record MessageDto(
        long BookingId,
        string[] Tos,
        string Subject,
        string Body,
        bool IsSendFax,
        string FaxNumber
    );
}
