namespace Liberty.Reservation.Application.Domains.Services.Mails;

public interface IMailService
{
    Task SendAsync(
        string[] tos,
        string subject,
        string body
    );
}
