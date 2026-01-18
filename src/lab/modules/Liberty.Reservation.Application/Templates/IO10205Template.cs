using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Application.Templates;

public class IO10205Template(
    IO10205TemplateFormat format
) : BaseTemplate
{
    public IO10205TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }

    private User? User { get; set; }

    private string? Url { get; set; }

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
                { "userName", User?.UserInfo?.Name },
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

        var user = new User { UserInfo = new UserInfo { Name = "サンプルユーザ" } };

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

public class IO10205TemplateFormat
{
    public string Subject { get; set; } = "*";

    public string Body { get; set; } = "*";
}
