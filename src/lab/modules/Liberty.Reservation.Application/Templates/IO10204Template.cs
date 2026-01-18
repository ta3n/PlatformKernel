using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Application.Templates;

public class IO10204Template(
    IO10204TemplateFormat format
) : BaseTemplate
{
    public IO10204TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }
    private User? User { get; set; }

    private string? Code { get; set; }
    private string? Token { get; set; }

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
                { "url", Url },
                { "dateTime", DateTime },
                { "ip", Meta?.ClientIpAddress },
                { "device", Meta?.ClientDevice }
            };
            return Format.Body.FormatByName(args);
        }
    }

    public string Url
    {
        get
        {
            var args = new FormatDictionary
            {
                { "code", Code },
                { "token", Token }
            };

            return Format.Url.FormatByName(args);
        }
    }

    public void SetData(
        string applicationName,
        User user,
        string code,
        string token,
        DateTime now,
        LoginHistoryMeta meta
    )
    {
        ApplicationName = applicationName;
        User = user;
        Code = code;
        Token = token;
        DateTime = now;
        Meta = meta;
    }

    public override void SetSample()
    {
        var applicationName = "";

        var user = new User { UserInfo = new UserInfo { Name = "サンプルユーザ" } };

        //
        Code = "codeABCDE";
        Token = "token12345";
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

public class IO10204TemplateFormat
{
    public string Subject { get; set; } = "*";

    public string Body { get; set; } = "*";

    public string Url { get; set; } = "";
}
