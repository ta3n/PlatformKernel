using Liberty.ApplicationShared.Utils;
using Microsoft.Extensions.Options;

namespace Liberty.ApplicationShared.Domains.Services.Mails;

public class SmtpMailService : IMailService
{
    private SmtpMailSetting Settings { get; set; } = new();

    public SmtpMailService(
        IOptions<SmtpMailSetting> smtpMailSettingOption
    )
    {
        Settings = smtpMailSettingOption.Value;
    }

    public SmtpMailService(
        string smtp,
        string userName,
        string password,
        string name,
        string mail,
        int? smtpPort
    )
    {
        Settings.Smtp = smtp;
        Settings.SmtpPort = smtpPort;
        Settings.UserName = userName;
        Settings.Password = password;
        Settings.Name = name;
        Settings.Mail = mail;
    }

    public async Task SendAsync(
        string[] tos,
        string subject,
        string body
    )
    {
        var mb = new MailBuilder
        {
            UserName = Settings.UserName ?? string.Empty,
            Smtp = Settings.Smtp ?? string.Empty,
            SmtpPort = Settings.SmtpPort,
            Password = Settings.Password ?? string.Empty
        };
        mb.AddFrom(
            Settings.Name ?? string.Empty,
            Settings.Mail ?? string.Empty
        );

        foreach (var to in tos)
        {
            mb.AddTo(to, to);
        }

        mb.Subject = subject;

        mb.Body = ConvertToHtml(body);

        await mb.SendAsync();
    }

    public async Task SendAsync(
        string[] tos,
        string subject,
        string body,
        string? fromDisplayName
    )
    {
        var mb = new MailBuilder
        {
            UserName = Settings.UserName ?? string.Empty,
            Smtp = Settings.Smtp ?? string.Empty,
            SmtpPort = Settings.SmtpPort,
            Password = Settings.Password ?? string.Empty
        };
        var senderName = !string.IsNullOrEmpty(fromDisplayName)
            ? fromDisplayName
            : Settings.Name ?? string.Empty;

        mb.AddFrom(senderName, Settings.Mail ?? string.Empty);

        foreach (var to in tos)
        {
            mb.AddTo(to, to);
        }

        mb.Subject = subject;

        mb.Body = ConvertToHtml(body);

        await mb.SendAsync();
    }

    private static string ConvertToHtml(
        string input
    )
    {
        input = input
            .Replace("\r\n", "<br>")
            .Replace("\n", "<br>")
            .Replace("\r", "<br>")
            .Replace("\t", "&emsp;")
            .Replace("  ", "&nbsp;&nbsp;");

        return $"<div style=\"white-space: pre-wrap;\">{input}</div>";
    }

    public class SmtpMailSetting
    {
        public string? ApplicationName { get; set; }
        public string? ApplicationNamePreview { get; set; }
        public string? FacilityNamePreview { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? Mail { get; set; }
        public string? Name { get; set; }
        public string? Smtp { get; set; }
        public int? SmtpPort { get; set; }
        public string? UrlMock { get; set; }
    }
}
