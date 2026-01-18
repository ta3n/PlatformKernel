using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Templates.FormatModels;
using Newtonsoft.Json;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Application.Templates;

public class TemplateFormatData
{
    private SmtpMailSetting Setting { get; set; } = new();

    public TemplateFormatData(
        SmtpMailSetting setting
    )
    {
        Setting = setting;
    }

    public TemplateFormatData() { }

    public Io10001TemplateFormat? Io10001TemplateFormat { get; private set; } = new();
    public Io10001TemplateFormat? Io10001EnTemplateFormat { get; private set; } = new();
    public Io10002TemplateFormat? Io10002TemplateFormat { get; private set; } = new();
    public Io10003TemplateFormat? Io10003TemplateFormat { get; private set; } = new();
    public Io10004TemplateFormat? Io10004TemplateFormat { get; private set; } = new();
    public Io10004TemplateFormat? Io10004EnTemplateFormat { get; private set; } = new();
    public Io10005TemplateFormat? Io10005TemplateFormat { get; private set; } = new();
    public Io10006TemplateFormat? Io10006TemplateFormat { get; private set; } = new();
    public Io10006TemplateFormat? Io10006EnTemplateFormat { get; private set; } = new();
    public Io10007TemplateFormat? Io10007TemplateFormat { get; private set; } = new();
    public Io10008TemplateFormat? Io10008TemplateFormat { get; private set; } = new();
    public Io10008TemplateFormat? Io10008EnTemplateFormat { get; private set; } = new();
    public Io10009TemplateFormat? Io10009TemplateFormat { get; private set; } = new();
    public Io10010TemplateFormat? Io10010TemplateFormat { get; private set; } = new();
    public Io10010TemplateFormat? Io10010EnTemplateFormat { get; private set; } = new();
    public Io10011TemplateFormat? Io10011TemplateFormat { get; private set; } = new();
    public Io10011TemplateFormat? Io10011EnTemplateFormat { get; private set; } = new();
    public Io10012TemplateFormat? Io10012TemplateFormat { get; private set; } = new();
    public Io10012TemplateFormat? Io10012EnTemplateFormat { get; private set; } = new();
    public Io10013TemplateFormat? Io10013TemplateFormat { get; private set; } = new();
    public Io10014TemplateFormat? Io10014TemplateFormat { get; private set; } = new();
    public Io10014TemplateFormat? Io10014EnTemplateFormat { get; private set; } = new();
    public Io10015TemplateFormat? Io10015TemplateFormat { get; private set; } = new();
    public Io10015TemplateFormat? Io10015EnTemplateFormat { get; private set; } = new();
    public Io10101TemplateFormat? Io10101TemplateFormat { get; private set; } = new();
    public Io10101TemplateFormat? Io10101EnTemplateFormat { get; private set; } = new();
    public Io10102TemplateFormat? Io10102TemplateFormat { get; private set; } = new();
    public Io10102TemplateFormat? Io10102EnTemplateFormat { get; private set; } = new();
    public Io10103TemplateFormat? Io10103TemplateFormat { get; private set; } = new();
    public Io10103TemplateFormat? Io10103EnTemplateFormat { get; private set; } = new();
    public Io10104TemplateFormat? Io10104TemplateFormat { get; private set; } = new();
    public Io10104TemplateFormat? Io10104EnTemplateFormat { get; private set; } = new();

    public Io10201TemplateFormat? Io10201TemplateFormat { get; private set; } = new();
    public Io10202TemplateFormat? Io10202TemplateFormat { get; private set; } = new();
    public Io10203TemplateFormat? Io10203TemplateFormat { get; private set; } = new();
    public Io10204TemplateFormat? Io10204TemplateFormat { get; private set; } = new();
    public Io10205TemplateFormat? Io10205TemplateFormat { get; private set; } = new();
    public Io10206TemplateFormat? Io10206TemplateFormat { get; private set; } = new();
    public Io10207TemplateFormat? Io10207TemplateFormat { get; private set; } = new();

    public Io20001TemplateFormat? Io20001TemplateFormat { get; private set; } = new();
    public Io20002TemplateFormat? Io20002TemplateFormat { get; private set; } = new();
    public Io20003TemplateFormat? Io20003TemplateFormat { get; private set; } = new();
    public Io20004TemplateFormat? Io20004TemplateFormat { get; private set; } = new();

    public Io30001TemplateFormat? Io30001TemplateFormat { get; private set; } = new();
    public Io30002TemplateFormat? Io30002TemplateFormat { get; private set; } = new();
    public Io30003TemplateFormat? Io30003TemplateFormat { get; private set; } = new();

    public void SetFormat(
        string type,
        string jsonString
    )
    {
        var formatMap = new Dictionary<string, Action<string>>
        {
            { IoType.IO10001, json => Io10001TemplateFormat = JsonConvert.DeserializeObject<Io10001TemplateFormat>(json) },
            { IoType.IO10001En, json => Io10001EnTemplateFormat = JsonConvert.DeserializeObject<Io10001TemplateFormat>(json) },
            { IoType.IO10002, json => Io10002TemplateFormat = JsonConvert.DeserializeObject<Io10002TemplateFormat>(json) },
            { IoType.IO10003, json => Io10003TemplateFormat = JsonConvert.DeserializeObject<Io10003TemplateFormat>(json) },
            { IoType.IO10004, json => Io10004TemplateFormat = JsonConvert.DeserializeObject<Io10004TemplateFormat>(json) },
            { IoType.IO10004En, json => Io10004EnTemplateFormat = JsonConvert.DeserializeObject<Io10004TemplateFormat>(json) },
            { IoType.IO10005, json => Io10005TemplateFormat = JsonConvert.DeserializeObject<Io10005TemplateFormat>(json) },
            { IoType.IO10006, json => Io10006TemplateFormat = JsonConvert.DeserializeObject<Io10006TemplateFormat>(json) },
            { IoType.IO10006En, json => Io10006EnTemplateFormat = JsonConvert.DeserializeObject<Io10006TemplateFormat>(json) },
            { IoType.IO10007, json => Io10007TemplateFormat = JsonConvert.DeserializeObject<Io10007TemplateFormat>(json) },
            { IoType.IO10008, json => Io10008TemplateFormat = JsonConvert.DeserializeObject<Io10008TemplateFormat>(json) },
            { IoType.IO10008En, json => Io10008EnTemplateFormat = JsonConvert.DeserializeObject<Io10008TemplateFormat>(json) },
            { IoType.IO10009, json => Io10009TemplateFormat = JsonConvert.DeserializeObject<Io10009TemplateFormat>(json) },
            { IoType.IO10010, json => Io10010TemplateFormat = JsonConvert.DeserializeObject<Io10010TemplateFormat>(json) },
            { IoType.IO10010En, json => Io10010EnTemplateFormat = JsonConvert.DeserializeObject<Io10010TemplateFormat>(json) },
            { IoType.IO10011, json => Io10011TemplateFormat = JsonConvert.DeserializeObject<Io10011TemplateFormat>(json) },
            { IoType.IO10011En, json => Io10011EnTemplateFormat = JsonConvert.DeserializeObject<Io10011TemplateFormat>(json) },
            { IoType.IO10012, json => Io10012TemplateFormat = JsonConvert.DeserializeObject<Io10012TemplateFormat>(json) },
            { IoType.IO10012En, json => Io10012EnTemplateFormat = JsonConvert.DeserializeObject<Io10012TemplateFormat>(json) },
            { IoType.IO10013, json => Io10013TemplateFormat = JsonConvert.DeserializeObject<Io10013TemplateFormat>(json) },
            { IoType.IO10014, json => Io10014TemplateFormat = JsonConvert.DeserializeObject<Io10014TemplateFormat>(json) },
            { IoType.IO10014En, json => Io10014EnTemplateFormat = JsonConvert.DeserializeObject<Io10014TemplateFormat>(json) },
            { IoType.IO10015, json => Io10015TemplateFormat = JsonConvert.DeserializeObject<Io10015TemplateFormat>(json) },
            { IoType.IO10015En, json => Io10015EnTemplateFormat = JsonConvert.DeserializeObject<Io10015TemplateFormat>(json) },
            { IoType.IO10101, json => Io10101TemplateFormat = JsonConvert.DeserializeObject<Io10101TemplateFormat>(json) },
            { IoType.IO10101En, json => Io10101EnTemplateFormat = JsonConvert.DeserializeObject<Io10101TemplateFormat>(json) },
            { IoType.IO10102, json => Io10102TemplateFormat = JsonConvert.DeserializeObject<Io10102TemplateFormat>(json) },
            { IoType.IO10102En, json => Io10102EnTemplateFormat = JsonConvert.DeserializeObject<Io10102TemplateFormat>(json) },
            { IoType.IO10103, json => Io10103TemplateFormat = JsonConvert.DeserializeObject<Io10103TemplateFormat>(json) },
            { IoType.IO10103En, json => Io10103EnTemplateFormat = JsonConvert.DeserializeObject<Io10103TemplateFormat>(json) },
            { IoType.IO10104, json => Io10104TemplateFormat = JsonConvert.DeserializeObject<Io10104TemplateFormat>(json) },
            { IoType.IO10104En, json => Io10104EnTemplateFormat = JsonConvert.DeserializeObject<Io10104TemplateFormat>(json) },
            { IoType.IO10201, json => Io10201TemplateFormat = JsonConvert.DeserializeObject<Io10201TemplateFormat>(json) },
            { IoType.IO10202, json => Io10202TemplateFormat = JsonConvert.DeserializeObject<Io10202TemplateFormat>(json) },
            { IoType.IO10203, json => Io10203TemplateFormat = JsonConvert.DeserializeObject<Io10203TemplateFormat>(json) },
            { IoType.IO10204, json => Io10204TemplateFormat = JsonConvert.DeserializeObject<Io10204TemplateFormat>(json) },
            { IoType.IO10205, json => Io10205TemplateFormat = JsonConvert.DeserializeObject<Io10205TemplateFormat>(json) },
            { IoType.IO10206, json => Io10206TemplateFormat = JsonConvert.DeserializeObject<Io10206TemplateFormat>(json) },
            { IoType.IO10207, json => Io10207TemplateFormat = JsonConvert.DeserializeObject<Io10207TemplateFormat>(json) },
            { IoType.IO20001, json => Io20001TemplateFormat = JsonConvert.DeserializeObject<Io20001TemplateFormat>(json) },
            { IoType.IO20002, json => Io20002TemplateFormat = JsonConvert.DeserializeObject<Io20002TemplateFormat>(json) },
            { IoType.IO20003, json => Io20003TemplateFormat = JsonConvert.DeserializeObject<Io20003TemplateFormat>(json) },
            { IoType.IO20004, json => Io20004TemplateFormat = JsonConvert.DeserializeObject<Io20004TemplateFormat>(json) },
            { IoType.IO30001, json => Io30001TemplateFormat = JsonConvert.DeserializeObject<Io30001TemplateFormat>(json) },
            { IoType.IO30002, json => Io30002TemplateFormat = JsonConvert.DeserializeObject<Io30002TemplateFormat>(json) },
            { IoType.IO30003, json => Io30003TemplateFormat = JsonConvert.DeserializeObject<Io30003TemplateFormat>(json) }
        };

        if (formatMap.TryGetValue(type, out var setFormatAction))
        {
            setFormatAction(jsonString);
        }
        else
        {
            throw new ArgumentException($"Unsupported IoType: {type}", nameof(type));
        }
    }

    public T? CreateTemplate<T>(
        string type
    ) where T : BaseTemplate
    {
        return type switch
        {
            IoType.IO10001 => new Io10001Template(Io10001TemplateFormat!, Setting) as T,
            IoType.IO10001En => new Io10001Template(Io10001EnTemplateFormat!, Setting) as T,
            IoType.IO10002 => new Io10002Template(Io10002TemplateFormat!) as T,
            IoType.IO10003 => new Io10003Template(Io10003TemplateFormat!, Setting) as T,
            IoType.IO10004 => new Io10004Template(Io10004TemplateFormat!, Setting) as T,
            IoType.IO10004En => new Io10004EnTemplate(Io10004EnTemplateFormat!, Setting) as T,
            IoType.IO10005 => new Io10005Template(Io10005TemplateFormat!, Setting) as T,
            IoType.IO10006 => new Io10006Template(Io10006TemplateFormat!, Setting) as T,
            IoType.IO10006En => new Io10006EnTemplate(Io10006EnTemplateFormat!, Setting) as T,
            IoType.IO10007 => new Io10007Template(Io10007TemplateFormat!, Setting) as T,
            IoType.IO10008 => new Io10008Template(Io10008TemplateFormat!, Setting) as T,
            IoType.IO10008En => new Io10008EnTemplate(Io10008EnTemplateFormat!, Setting) as T,
            IoType.IO10009 => new Io10009Template(Io10009TemplateFormat!) as T,
            IoType.IO10010 => new Io10010Template(Io10010TemplateFormat!, Setting) as T,
            IoType.IO10010En => new Io10010EnTemplate(Io10010EnTemplateFormat!, Setting) as T,
            IoType.IO10011 => new Io10011Template(Io10011TemplateFormat!, Setting) as T,
            IoType.IO10011En => new Io10011EnTemplate(Io10011EnTemplateFormat!, Setting) as T,
            IoType.IO10012 => new Io10012Template(Io10012TemplateFormat!, Setting) as T,
            IoType.IO10012En => new Io10012EnTemplate(Io10012EnTemplateFormat!, Setting) as T,
            IoType.IO10013 => new Io10013Template(Io10013TemplateFormat!, Setting) as T,
            IoType.IO10014 => new Io10014Template(Io10014TemplateFormat!, Setting) as T,
            IoType.IO10014En => new Io10014EnTemplate(Io10014EnTemplateFormat!, Setting) as T,
            IoType.IO10015 => new Io10015Template(Io10015TemplateFormat!, Setting) as T,
            IoType.IO10015En => new Io10015EnTemplate(Io10015EnTemplateFormat!, Setting) as T,
            IoType.IO10101 => new Io10101Template(Io10101TemplateFormat!, Setting) as T,
            IoType.IO10101En => new Io10101EnTemplate(Io10101EnTemplateFormat!, Setting) as T,
            IoType.IO10102 => new Io10102Template(Io10102TemplateFormat!, Setting) as T,
            IoType.IO10102En => new Io10102EnTemplate(Io10102EnTemplateFormat!, Setting) as T,
            IoType.IO10103 => new Io10103Template(Io10103TemplateFormat!, Setting) as T,
            IoType.IO10103En => new Io10103EnTemplate(Io10103EnTemplateFormat!, Setting) as T,
            IoType.IO10104 => new Io10104Template(Io10104TemplateFormat!, Setting) as T,
            IoType.IO10104En => new Io10104EnTemplate(Io10104EnTemplateFormat!, Setting) as T,
            IoType.IO10201 => new Io10201Template(Io10201TemplateFormat!) as T,
            IoType.IO10202 => new Io10202Template(Io10202TemplateFormat!) as T,
            IoType.IO10203 => new Io10203Template(Io10203TemplateFormat!) as T,
            IoType.IO10204 => new Io10204Template(Io10204TemplateFormat!) as T,
            IoType.IO10205 => new Io10205Template(Io10205TemplateFormat!) as T,
            IoType.IO10206 => new Io10206Template(Io10206TemplateFormat!) as T,
            IoType.IO10207 => new Io10207Template(Io10207TemplateFormat!) as T,
            IoType.IO20001 => new Io20001Template(Io20001TemplateFormat!) as T,
            IoType.IO20002 => new Io20002Template(Io20002TemplateFormat!) as T,
            IoType.IO20003 => new Io20003Template(Io20003TemplateFormat!) as T,
            IoType.IO20004 => new Io20004Template(Io20004TemplateFormat!) as T,
            IoType.IO30001 => new Io30001Template(Io30001TemplateFormat!) as T,
            IoType.IO30002 => new Io30002Template(Io30002TemplateFormat!) as T,
            IoType.IO30003 => new Io30003Template(Io30003TemplateFormat!) as T,
            _ => throw new FileNotFoundException(type)
        };
    }

    public IIoTemplate? GetTemplate(
        string type
    )
    {
        return type switch
        {
            IoType.IO10001 => CreateTemplate<Io10001Template>(type),
            IoType.IO10001En => CreateTemplate<Io10001Template>(type),
            IoType.IO10002 => CreateTemplate<Io10002Template>(type),
            IoType.IO10003 => CreateTemplate<Io10003Template>(type),
            IoType.IO10004 => CreateTemplate<Io10004Template>(type),
            IoType.IO10004En => CreateTemplate<Io10004EnTemplate>(type),
            IoType.IO10005 => CreateTemplate<Io10005Template>(type),
            IoType.IO10006 => CreateTemplate<Io10006Template>(type),
            IoType.IO10006En => CreateTemplate<Io10006EnTemplate>(type),
            IoType.IO10007 => CreateTemplate<Io10007Template>(type),
            IoType.IO10008 => CreateTemplate<Io10008Template>(type),
            IoType.IO10008En => CreateTemplate<Io10008EnTemplate>(type),
            IoType.IO10009 => CreateTemplate<Io10009Template>(type),
            IoType.IO10010 => CreateTemplate<Io10010Template>(type),
            IoType.IO10010En => CreateTemplate<Io10010EnTemplate>(type),
            IoType.IO10011 => CreateTemplate<Io10011Template>(type),
            IoType.IO10011En => CreateTemplate<Io10011EnTemplate>(type),
            IoType.IO10012 => CreateTemplate<Io10012Template>(type),
            IoType.IO10012En => CreateTemplate<Io10012EnTemplate>(type),
            IoType.IO10013 => CreateTemplate<Io10013Template>(type),
            IoType.IO10014 => CreateTemplate<Io10014Template>(type),
            IoType.IO10014En => CreateTemplate<Io10014EnTemplate>(type),
            IoType.IO10015 => CreateTemplate<Io10015Template>(type),
            IoType.IO10015En => CreateTemplate<Io10015EnTemplate>(type),
            IoType.IO10101 => CreateTemplate<Io10101Template>(type),
            IoType.IO10101En => CreateTemplate<Io10101EnTemplate>(type),
            IoType.IO10103 => CreateTemplate<Io10103Template>(type),
            IoType.IO10103En => CreateTemplate<Io10103EnTemplate>(type),
            IoType.IO10104 => CreateTemplate<Io10104Template>(type),
            IoType.IO10104En => CreateTemplate<Io10104EnTemplate>(type),
            IoType.IO10201 => CreateTemplate<Io10201Template>(type),
            IoType.IO10202 => CreateTemplate<Io10202Template>(type),
            IoType.IO10203 => CreateTemplate<Io10203Template>(type),
            IoType.IO10204 => CreateTemplate<Io10204Template>(type),
            IoType.IO10205 => CreateTemplate<Io10205Template>(type),
            IoType.IO10206 => CreateTemplate<Io10206Template>(type),
            IoType.IO10207 => CreateTemplate<Io10207Template>(type),
            IoType.IO20001 => CreateTemplate<Io20001Template>(type),
            IoType.IO20002 => CreateTemplate<Io20002Template>(type),
            IoType.IO20003 => CreateTemplate<Io20003Template>(type),
            IoType.IO20004 => CreateTemplate<Io20004Template>(type),
            IoType.IO30001 => CreateTemplate<Io30001Template>(type),
            IoType.IO30002 => CreateTemplate<Io30002Template>(type),
            IoType.IO30003 => CreateTemplate<Io30003Template>(type),
            IoType.IO10102 => CreateTemplate<Io10102Template>(type),
            IoType.IO10102En => CreateTemplate<Io10102EnTemplate>(type),
            _ => throw new FileNotFoundException(type)
        };
    }

    public void SetUrl(
        string url
    )
    {
        Io10001TemplateFormat!.Url = url;
        Io10002TemplateFormat!.Url = url;
        Io10201TemplateFormat!.Url = url;
        Io10204TemplateFormat!.Url = url;
        Io10207TemplateFormat!.Url = url;
    }
}
