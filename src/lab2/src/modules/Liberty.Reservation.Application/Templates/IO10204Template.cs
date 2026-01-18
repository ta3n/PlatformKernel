using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10204Template(
    Io10204TemplateFormat format
) : BaseTemplate
{
    private Io10204TemplateFormat Format { get; set; } = format;
    private string? UserCode { get; set; }
    private string? UserFullName { get; set; }

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
                { "userName", UserFullName },
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
        string userCode,
        string userFullName,
        string code,
        string token,
        DateTime now,
        LoginHistoryMeta meta
    )
    {
        ApplicationName = applicationName;
        UserCode = userCode;
        UserFullName = userFullName;
        Code = code;
        Token = token;
        DateTime = now;
        Meta = meta;
    }

    public override void SetSample()
    {
        var applicationName = string.Empty;

        Code = "codeABCDE";
        Token = "token12345";
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
