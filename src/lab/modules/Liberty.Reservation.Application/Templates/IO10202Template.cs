using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Application.Templates;

public class IO10202Template(
    IO10202TemplateFormat format
) : BaseTemplate
{
    public IO10202TemplateFormat Format { get; private set; } = format;

    private User? User { get; set; }

    private string? ApplicationName { get; set; }

    private DateTime DateTime { get; set; }
    private LoginHistoryMeta? Meta { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary { };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var args = new FormatDictionary
            {
                { "email", User?.Email },
                { "dateTime", DateTime },
                { "ip", Meta?.ClientIpAddress },
                { "device", Meta?.ClientDevice }
            };
            return Format.Body.FormatByName(args);
        }
    }

    public void SetData(
        string applicationName,
        User user,
        DateTime now,
        LoginHistoryMeta meta
    )
    {
        ApplicationName = applicationName;
        User = user;
        DateTime = now;
        Meta = meta;
    }

    public override void SetSample()
    {
        var applicationName = "";

        var user = new User
        {
            Email = "sample@liberty-mail.net",
            UserInfo = new UserInfo { }
        };

        //
        User = user;
        ApplicationName = applicationName;
        //
        DateTime = DateTime.Now;
        Meta = new LoginHistoryMeta
        {
            ClientIpAddress = "xxx.xxx.xxx.xxx",
            ClientDevice = "device for sample message",
            ReturnUrl = "http://localhost:8080/s001"
        };
    }
}

public class IO10202TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
