using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10206Template(
    Io10206TemplateFormat format
) : BaseTemplate
{
    private Io10206TemplateFormat Format { get; set; } = format;

    private string? UserCode { get; set; }
    private string? UserFullName { get; set; }

    private DateTime DateTime { get; set; }
    private LoginHistoryMeta? Meta { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary();
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            _ = UserCode;
            var args = new FormatDictionary
            {
                { "userName", UserFullName },
                { "dateTime", DateTime },
                { "ip", Meta?.ClientIpAddress },
                { "device", Meta?.ClientDevice },
                { "returnUrl", Meta?.ReturnUrl }
            };
            return Format.Body.FormatByName(args);
        }
    }

    public void SetData(
        string applicationName,
        string userCode,
        string userFullName,
        DateTime now,
        LoginHistoryMeta meta
    )
    {
        ApplicationName = applicationName;
        UserCode = userCode;
        UserFullName = userFullName;
        DateTime = now;
        Meta = meta;
    }

    public override void SetSample()
    {
        var applicationName = string.Empty;

        UserFullName = "サンプルユーザ";
        ApplicationName = applicationName;

        DateTime = DateTime.Now;
        Meta = new LoginHistoryMeta
        {
            ClientIpAddress = "xxx.xxx.xxx.xxx",
            ClientDevice = "device for sample message",
            ReturnUrl = "http://localhost:8080/s001"
        };
    }
}
