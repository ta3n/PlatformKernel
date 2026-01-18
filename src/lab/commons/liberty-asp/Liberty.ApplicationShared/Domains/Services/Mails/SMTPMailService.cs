using Liberty.ApplicationShared.Utils;

namespace Liberty.ApplicationShared.Domains.Services.Mails;

public class SmtpMailService : IMailService
{
    public SmtpMailServiceSetting Settings { get; } = new();

    public SmtpMailService(
        string smtp,
        string userName,
        string password,
        string name,
        string mail,
        int? smtpPort
    )
    {
        Settings.SendMail.From.Smtp = smtp;
        Settings.SendMail.From.SmtpPort = smtpPort;
        Settings.SendMail.From.UserName = userName;
        Settings.SendMail.From.Password = password;
        Settings.SendMail.From.Name = name;
        Settings.SendMail.From.Mail = mail;
    }

    public async Task SendAsync(
        string[] tos,
        string subject,
        string body
    )
    {
        var mb = new MailBuilder
        {
            UserName = Settings.SendMail.From.UserName ?? string.Empty,
            Smtp = Settings.SendMail.From.Smtp ?? string.Empty,
            SmtpPort = Settings.SendMail.From.SmtpPort,
            Password = Settings.SendMail.From.Password ?? string.Empty
        };
        mb.AddFrom(
            Settings.SendMail.From.Name ?? string.Empty,
            Settings.SendMail.From.Mail ?? string.Empty
        );

        foreach (var to in tos)
        {
            mb.AddTo(to, to);
        }

        mb.Subject = subject;
        mb.Body = body;
        //
        await mb.SendAsync();
    }

    public class SmtpMailServiceSetting
    {
        public SmtpMailServiceSettingSendMail SendMail { get; set; } = new();

        public class SmtpMailServiceSettingSendMail
        {
            public SmtpMailServiceSettingSendMailFrom From { get; set; } = new();

            public class SmtpMailServiceSettingSendMailFrom
            {
                public string? Password { get; internal set; }
                public string? UserName { get; internal set; }
                public string? Mail { get; internal set; }
                public string? Name { get; internal set; }
                public string? Smtp { get; internal set; }
                public int? SmtpPort { get; internal set; }
            }
        }
    }
}
