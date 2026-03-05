using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10203Template(
    Io10203TemplateFormat format
) : BaseTemplate
{
    private Io10203TemplateFormat Format { get; set; } = format;

    private string? UserCode { get; set; }
    private string? UserEmail { get; set; }

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
                { "email", UserEmail },
                { "dateTime", DateTime },
                { "ip", Meta?.ClientIpAddress },
                { "device", Meta?.ClientDevice }
            };
            return Format.Body.FormatByName(args);
        }
    }

    public void SetData(
        string applicationName,
        string userCode,
        string userEmail,
        DateTime now,
        LoginHistoryMeta meta
    )
    {
        ApplicationName = applicationName;
        UserCode = userCode;
        UserEmail = userEmail;
        DateTime = now;
        Meta = meta;
    }

    public override void SetSample()
    {
        var applicationName = string.Empty;

        UserEmail = "sample@liberty-mail.net";
        ApplicationName = applicationName;

        DateTime = DateTime.Now;
        Meta = new LoginHistoryMeta
        {
            ClientIpAddress = "xxx.xxx.xxx.xxx",
            ClientDevice = "device for sample message"
        };
    }
}
