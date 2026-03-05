using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10201Template(
    Io10201TemplateFormat format
) : BaseTemplate
{
    private Io10201TemplateFormat Format { get; set; } = format;
    private string? UserCode { get; set; }
    private string? UserEmail { get; set; }

    private string? Code { get; set; }
    private string? Token { get; set; }

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
                { "token", Token },
                { "returnUrl", Meta?.ReturnUrl }
            };

            return Format.Url.FormatByName(args);
        }
    }

    public void SetData(
        string applicationName,
        string userCode,
        string userEmail,
        string code,
        string token,
        LoginHistoryMeta meta
    )
    {
        ApplicationName = applicationName;
        UserCode = userCode;
        UserEmail = userEmail;
        Code = code;
        Token = token;
        Meta = meta;
    }

    public override void SetSample()
    {
        var applicationName = string.Empty;

        Code = "codeABCDE";
        Token = "token12345";
        UserEmail = "sample@liberty-mail.net";
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
