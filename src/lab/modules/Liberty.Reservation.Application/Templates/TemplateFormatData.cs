using Liberty.Reservation.Application.Constants;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Templates;

public class TemplateFormatData
{
    public IO10001TemplateFormat? IO10001TemplateFormat { get; set; } = new();
    public IO10002TemplateFormat? IO10002TemplateFormat { get; set; } = new();
    public IO10003TemplateFormat? IO10003TemplateFormat { get; set; } = new();
    public IO10004TemplateFormat? IO10004TemplateFormat { get; set; } = new();
    public IO10005TemplateFormat? IO10005TemplateFormat { get; set; } = new();
    public IO10006TemplateFormat? IO10006TemplateFormat { get; set; } = new();
    public IO10007TemplateFormat? IO10007TemplateFormat { get; set; } = new();
    public IO10008TemplateFormat? IO10008TemplateFormat { get; set; } = new();
    public IO10009TemplateFormat? IO10009TemplateFormat { get; set; } = new();
    public IO10010TemplateFormat? IO10010TemplateFormat { get; set; } = new();

    public IO10101TemplateFormat? IO10101TemplateFormat { get; set; } = new();
    public IO10102TemplateFormat? IO10102TemplateFormat { get; set; } = new();

    public IO10201TemplateFormat? IO10201TemplateFormat { get; set; } = new();
    public IO10202TemplateFormat? IO10202TemplateFormat { get; set; } = new();
    public IO10203TemplateFormat? IO10203TemplateFormat { get; set; } = new();
    public IO10204TemplateFormat? IO10204TemplateFormat { get; set; } = new();
    public IO10205TemplateFormat? IO10205TemplateFormat { get; set; } = new();
    public IO10206TemplateFormat? IO10206TemplateFormat { get; set; } = new();
    public IO10207TemplateFormat? IO10207TemplateFormat { get; set; } = new();

    public IO20001TemplateFormat? IO20001TemplateFormat { get; set; } = new();
    public IO20002TemplateFormat? IO20002TemplateFormat { get; set; } = new();
    public IO20003TemplateFormat? IO20003TemplateFormat { get; set; } = new();
    public IO20004TemplateFormat? IO20004TemplateFormat { get; set; } = new();

    public IO30001TemplateFormat? IO30001TemplateFormat { get; set; } = new();
    public IO30002TemplateFormat? IO30002TemplateFormat { get; set; } = new();
    public IO30003TemplateFormat? IO30003TemplateFormat { get; set; } = new();

    public void Fill()
    {
        // JSONテキストがない場合nullになってしまうので、null時に初期状態のインスタンスが返るようにする
        IO10001TemplateFormat ??= new IO10001TemplateFormat();
        IO10002TemplateFormat ??= new IO10002TemplateFormat();
        IO10003TemplateFormat ??= new IO10003TemplateFormat();
        IO10004TemplateFormat ??= new IO10004TemplateFormat();
        IO10005TemplateFormat ??= new IO10005TemplateFormat();
        IO10006TemplateFormat ??= new IO10006TemplateFormat();
        IO10007TemplateFormat ??= new IO10007TemplateFormat();
        IO10008TemplateFormat ??= new IO10008TemplateFormat();
        IO10009TemplateFormat ??= new IO10009TemplateFormat();
        IO10010TemplateFormat ??= new IO10010TemplateFormat();

        IO10101TemplateFormat ??= new IO10101TemplateFormat();
        IO10102TemplateFormat ??= new IO10102TemplateFormat();

        IO10201TemplateFormat ??= new IO10201TemplateFormat();
        IO10202TemplateFormat ??= new IO10202TemplateFormat();
        IO10203TemplateFormat ??= new IO10203TemplateFormat();
        IO10204TemplateFormat ??= new IO10204TemplateFormat();
        IO10205TemplateFormat ??= new IO10205TemplateFormat();
        IO10206TemplateFormat ??= new IO10206TemplateFormat();
        IO10207TemplateFormat ??= new IO10207TemplateFormat();

        IO20001TemplateFormat ??= new IO20001TemplateFormat();
        IO20002TemplateFormat ??= new IO20002TemplateFormat();
        IO20003TemplateFormat ??= new IO20003TemplateFormat();
        IO20004TemplateFormat ??= new IO20004TemplateFormat();

        IO30001TemplateFormat ??= new IO30001TemplateFormat();
        IO30002TemplateFormat ??= new IO30002TemplateFormat();
        IO30003TemplateFormat ??= new IO30003TemplateFormat();
    }

    public void SetFormat(
        string type,
        string jsonString
    )
    {
        switch (type)
        {
            case IoType.IO10001:
                IO10001TemplateFormat = JsonConvert.DeserializeObject<IO10001TemplateFormat>(jsonString);
                break;

            case IoType.IO10002:
                IO10002TemplateFormat = JsonConvert.DeserializeObject<IO10002TemplateFormat>(jsonString);
                break;

            case IoType.IO10003:
                IO10003TemplateFormat = JsonConvert.DeserializeObject<IO10003TemplateFormat>(jsonString);
                break;

            case IoType.IO10004:
                IO10004TemplateFormat = JsonConvert.DeserializeObject<IO10004TemplateFormat>(jsonString);
                break;

            case IoType.IO10005:
                IO10005TemplateFormat = JsonConvert.DeserializeObject<IO10005TemplateFormat>(jsonString);
                break;

            case IoType.IO10006:
                IO10006TemplateFormat = JsonConvert.DeserializeObject<IO10006TemplateFormat>(jsonString);
                break;

            case IoType.IO10007:
                IO10007TemplateFormat = JsonConvert.DeserializeObject<IO10007TemplateFormat>(jsonString);
                break;

            case IoType.IO10008:
                IO10008TemplateFormat = JsonConvert.DeserializeObject<IO10008TemplateFormat>(jsonString);
                break;
            case IoType.IO10009:
                IO10009TemplateFormat = JsonConvert.DeserializeObject<IO10009TemplateFormat>(jsonString);
                break;
            case IoType.IO10010:
                IO10010TemplateFormat = JsonConvert.DeserializeObject<IO10010TemplateFormat>(jsonString);
                break;

            case IoType.IO10101:
                IO10101TemplateFormat = JsonConvert.DeserializeObject<IO10101TemplateFormat>(jsonString);
                break;
            case IoType.IO10102:
                IO10102TemplateFormat = JsonConvert.DeserializeObject<IO10102TemplateFormat>(jsonString);
                break;

            case IoType.IO10201:
                IO10201TemplateFormat = JsonConvert.DeserializeObject<IO10201TemplateFormat>(jsonString);
                break;
            case IoType.IO10202:
                IO10202TemplateFormat = JsonConvert.DeserializeObject<IO10202TemplateFormat>(jsonString);
                break;
            case IoType.IO10203:
                IO10203TemplateFormat = JsonConvert.DeserializeObject<IO10203TemplateFormat>(jsonString);
                break;
            case IoType.IO10204:
                IO10204TemplateFormat = JsonConvert.DeserializeObject<IO10204TemplateFormat>(jsonString);
                break;
            case IoType.IO10205:
                IO10205TemplateFormat = JsonConvert.DeserializeObject<IO10205TemplateFormat>(jsonString);
                break;
            case IoType.IO10206:
                IO10206TemplateFormat = JsonConvert.DeserializeObject<IO10206TemplateFormat>(jsonString);
                break;
            case IoType.IO10207:
                IO10207TemplateFormat = JsonConvert.DeserializeObject<IO10207TemplateFormat>(jsonString);
                break;

            case IoType.IO20001:
                IO20001TemplateFormat = JsonConvert.DeserializeObject<IO20001TemplateFormat>(jsonString);
                break;
            case IoType.IO20002:
                IO20002TemplateFormat = JsonConvert.DeserializeObject<IO20002TemplateFormat>(jsonString);
                break;
            case IoType.IO20003:
                IO20003TemplateFormat = JsonConvert.DeserializeObject<IO20003TemplateFormat>(jsonString);
                break;
            case IoType.IO20004:
                IO20004TemplateFormat = JsonConvert.DeserializeObject<IO20004TemplateFormat>(jsonString);
                break;

            case IoType.IO30001:
                IO30001TemplateFormat = JsonConvert.DeserializeObject<IO30001TemplateFormat>(jsonString);
                break;
            case IoType.IO30002:
                IO30002TemplateFormat = JsonConvert.DeserializeObject<IO30002TemplateFormat>(jsonString);
                break;
            case IoType.IO30003:
                IO30003TemplateFormat = JsonConvert.DeserializeObject<IO30003TemplateFormat>(jsonString);
                break;
        }
    }

    public T? CreateTemplate<T>(
        string type
    ) where T : BaseTemplate
    {
        return type switch
        {
            IoType.IO10001 => new IO10001Template(IO10001TemplateFormat!) as T,
            IoType.IO10002 => new IO10002Template(IO10002TemplateFormat!) as T,
            IoType.IO10003 => new IO10003Template(IO10003TemplateFormat!) as T,
            IoType.IO10004 => new IO10004Template(IO10004TemplateFormat!) as T,
            IoType.IO10005 => new IO10005Template(IO10005TemplateFormat!) as T,
            IoType.IO10006 => new IO10006Template(IO10006TemplateFormat!) as T,
            IoType.IO10007 => new IO10007Template(IO10007TemplateFormat!) as T,
            IoType.IO10008 => new IO10008Template(IO10008TemplateFormat!) as T,
            IoType.IO10009 => new IO10009Template(IO10009TemplateFormat!) as T,
            IoType.IO10010 => new IO10010Template(IO10010TemplateFormat!) as T,
            IoType.IO10101 => new IO10101Template(IO10101TemplateFormat!) as T,
            IoType.IO10102 => new IO10102Template(IO10102TemplateFormat!) as T,
            IoType.IO10201 => new IO10201Template(IO10201TemplateFormat!) as T,
            IoType.IO10202 => new IO10202Template(IO10202TemplateFormat!) as T,
            IoType.IO10203 => new IO10203Template(IO10203TemplateFormat!) as T,
            IoType.IO10204 => new IO10204Template(IO10204TemplateFormat!) as T,
            IoType.IO10205 => new IO10205Template(IO10205TemplateFormat!) as T,
            IoType.IO10206 => new IO10206Template(IO10206TemplateFormat!) as T,
            IoType.IO10207 => new IO10207Template(IO10207TemplateFormat!) as T,
            IoType.IO20001 => new IO20001Template(IO20001TemplateFormat!) as T,
            IoType.IO20002 => new IO20002Template(IO20002TemplateFormat!) as T,
            IoType.IO20003 => new IO20003Template(IO20003TemplateFormat!) as T,
            IoType.IO20004 => new IO20004Template(IO20004TemplateFormat!) as T,
            IoType.IO30001 => new IO30001Template(IO30001TemplateFormat!) as T,
            IoType.IO30002 => new IO30002Template(IO30002TemplateFormat!) as T,
            IoType.IO30003 => new IO30003Template(IO30003TemplateFormat!) as T,
            // FIXME
            _ => throw new FileNotFoundException(type)
        };
    }

    public IIOTemplate? GetTemplate(
        string type
    )
    {
        return type switch
        {
            IoType.IO10001 => CreateTemplate<IO10001Template>(type),
            IoType.IO10002 => CreateTemplate<IO10002Template>(type),
            IoType.IO10003 => CreateTemplate<IO10003Template>(type),
            IoType.IO10004 => CreateTemplate<IO10004Template>(type),
            IoType.IO10005 => CreateTemplate<IO10005Template>(type),
            IoType.IO10006 => CreateTemplate<IO10006Template>(type),
            IoType.IO10007 => CreateTemplate<IO10007Template>(type),
            IoType.IO10008 => CreateTemplate<IO10008Template>(type),
            IoType.IO10009 => CreateTemplate<IO10009Template>(type),
            IoType.IO10010 => CreateTemplate<IO10010Template>(type),
            IoType.IO10101 => CreateTemplate<IO10101Template>(type),
            IoType.IO10102 => CreateTemplate<IO10102Template>(type),
            IoType.IO10201 => CreateTemplate<IO10201Template>(type),
            IoType.IO10202 => CreateTemplate<IO10202Template>(type),
            IoType.IO10203 => CreateTemplate<IO10203Template>(type),
            IoType.IO10204 => CreateTemplate<IO10204Template>(type),
            IoType.IO10205 => CreateTemplate<IO10205Template>(type),
            IoType.IO10206 => CreateTemplate<IO10206Template>(type),
            IoType.IO10207 => CreateTemplate<IO10207Template>(type),
            IoType.IO20001 => CreateTemplate<IO20001Template>(type),
            IoType.IO20002 => CreateTemplate<IO20002Template>(type),
            IoType.IO20003 => CreateTemplate<IO20003Template>(type),
            IoType.IO20004 => CreateTemplate<IO20004Template>(type),
            IoType.IO30001 => CreateTemplate<IO30001Template>(type),
            IoType.IO30002 => CreateTemplate<IO30002Template>(type),
            IoType.IO30003 => CreateTemplate<IO30003Template>(type),
            // FIXME
            _ => throw new FileNotFoundException(type)
        };
    }
}
