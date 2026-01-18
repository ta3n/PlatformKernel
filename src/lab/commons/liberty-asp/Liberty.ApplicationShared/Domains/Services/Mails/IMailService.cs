namespace Liberty.ApplicationShared.Domains.Services.Mails;

public interface IMailService
{
    Task SendAsync(
        string[] tos,
        string subject,
        string body
    );
}
