using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Templates.FormatModels;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Application.Templates;

/// <summary>
/// Provides base mail parameters and methods to retrieve mail template parameters
/// for various reservation-related scenarios.
/// </summary>
public static class BaseMailParameter
{
    /// <summary>
    /// Retrieves a list of base mail parameters that are common across templates.
    /// </summary>
    /// <returns>A list of <see cref="MailTemplateParameter"/> representing base mail parameters.</returns>
    public static List<MailTemplateParameter> GetBaseMailParameters()
    {
        return
        [
            MailParameter.ReserverName,
            MailParameter.ReserverNameKana,
            MailParameter.ReserverEmail,
            MailParameter.ReserverAddressCountry,
            MailParameter.ReserverAddressPostalCode,
            MailParameter.ReserverAddressPrefecture,
            MailParameter.ReserverAddressCity,
            MailParameter.ReserverAddressStreet,
            MailParameter.ReserverTel,
            MailParameter.ReserverGender
        ];
    }

    /// <summary>
    /// Retrieves a list of booking-specific mail parameters.
    /// </summary>
    /// <returns>A list of <see cref="MailTemplateParameter"/> representing booking parameters.</returns>
    public static List<MailTemplateParameter> GetBookingParameters()
    {
        return
        [
            MailParameter.ApplicationName,
            MailParameter.Code,
            MailParameter.FacilityName,
            MailParameter.ReservationDateTime,
            MailParameter.CheckInDate,
            MailParameter.CheckOutDate,
            MailParameter.CheckInTime,
            MailParameter.CheckOutTime,
            MailParameter.RestNumber,
            MailParameter.RoomNumber,
            MailParameter.TotalPersons,
            MailParameter.FirstRestMaleNumber,
            MailParameter.FirstRestFemaleNumber,
            MailParameter.FirstRestChildNumber,
            MailParameter.RoomGroupName,
            MailParameter.PlanName,
            MailParameter.MealType,
            MailParameter.ReservationDetail,
            MailParameter.MainUserName,
            MailParameter.PayOff
        ];
    }

    /// <summary>
    /// Combines base mail parameters and booking parameters into a single list.
    /// </summary>
    /// <returns>A list of <see cref="MailTemplateParameter"/> containing both base and booking parameters.</returns>
    public static List<MailTemplateParameter> GetBaseMailBookingParameters()
    {
        return
        [
            .. GetBaseMailParameters(),
            .. GetBookingParameters()
        ];
    }

    /// <summary>
    /// Retrieves mail template parameters based on the specified template type.
    /// </summary>
    /// <param name="type">The type of the mail template.</param>
    /// <returns>A list of <see cref="MailTemplateParameter"/> for the specified template type.</returns>
    /// <exception cref="NotImplementedException">Thrown when the specified template type is not implemented.</exception>
    public static List<MailTemplateParameter> GetTemplateMailParameters(
        string type
    )
    {
        var smptMailSetting = new SmtpMailSetting();

        // Selects the appropriate template based on the provided type.
        BaseTemplate template = type switch
        {
            IoType.IO10001 => new Io10001Template(new Io10001TemplateFormat(), smptMailSetting),
            IoType.IO10001En => new Io10001Template(new Io10001TemplateFormat(), smptMailSetting),
            IoType.IO10002 => new Io10002Template(new Io10002TemplateFormat()),
            IoType.IO10003 => new Io10003Template(new Io10003TemplateFormat(), smptMailSetting),
            IoType.IO10004 => new Io10004Template(new Io10004TemplateFormat(), smptMailSetting),
            IoType.IO10004En => new Io10004Template(new Io10004TemplateFormat(), smptMailSetting),
            IoType.IO10005 => new Io10005Template(new Io10005TemplateFormat(), smptMailSetting),
            IoType.IO10006 => new Io10006Template(new Io10006TemplateFormat(), smptMailSetting),
            IoType.IO10006En => new Io10006Template(new Io10006TemplateFormat(), smptMailSetting),
            IoType.IO10007 => new Io10007Template(new Io10007TemplateFormat(), smptMailSetting),
            IoType.IO10008 => new Io10008Template(new Io10008TemplateFormat(), smptMailSetting),
            IoType.IO10008En => new Io10008Template(new Io10008TemplateFormat(), smptMailSetting),
            IoType.IO10009 => new Io10009Template(new Io10009TemplateFormat()),
            IoType.IO10010 => new Io10010Template(new Io10010TemplateFormat(), smptMailSetting),
            IoType.IO10010En => new Io10010Template(new Io10010TemplateFormat(), smptMailSetting),
            IoType.IO10011 => new Io10011Template(new Io10011TemplateFormat(), smptMailSetting),
            IoType.IO10011En => new Io10011Template(new Io10011TemplateFormat(), smptMailSetting),
            IoType.IO10012 => new Io10012Template(new Io10012TemplateFormat(), smptMailSetting),
            IoType.IO10012En => new Io10012Template(new Io10012TemplateFormat(), smptMailSetting),
            IoType.IO10013 => new Io10013Template(new Io10013TemplateFormat(), smptMailSetting),
            IoType.IO10014 => new Io10014Template(new Io10014TemplateFormat(), smptMailSetting),
            IoType.IO10014En => new Io10014Template(new Io10014TemplateFormat(), smptMailSetting),
            IoType.IO10015 => new Io10015Template(new Io10015TemplateFormat(), smptMailSetting),
            IoType.IO10015En => new Io10015Template(new Io10015TemplateFormat(), smptMailSetting),
            IoType.IO10101 => new Io10101Template(new Io10101TemplateFormat(), smptMailSetting),
            IoType.IO10101En => new Io10101Template(new Io10101TemplateFormat(), smptMailSetting),
            IoType.IO10102 => new Io10102Template(new Io10102TemplateFormat(), smptMailSetting),
            IoType.IO10102En => new Io10102Template(new Io10102TemplateFormat(), smptMailSetting),
            IoType.IO10103 => new Io10103Template(new Io10103TemplateFormat(), smptMailSetting),
            IoType.IO10103En => new Io10103Template(new Io10103TemplateFormat(), smptMailSetting),
            IoType.IO10104 => new Io10104Template(new Io10104TemplateFormat(), smptMailSetting),
            IoType.IO10104En => new Io10104Template(new Io10104TemplateFormat(), smptMailSetting),
            IoType.IO10201 => new Io10201Template(new Io10201TemplateFormat()),
            IoType.IO10202 => new Io10202Template(new Io10202TemplateFormat()),
            IoType.IO10203 => new Io10203Template(new Io10203TemplateFormat()),
            IoType.IO10204 => new Io10204Template(new Io10204TemplateFormat()),
            IoType.IO10205 => new Io10205Template(new Io10205TemplateFormat()),
            IoType.IO10206 => new Io10206Template(new Io10206TemplateFormat()),
            IoType.IO10207 => new Io10207Template(new Io10207TemplateFormat()),
            IoType.IO20001 => new Io20001Template(new Io20001TemplateFormat()),
            IoType.IO20002 => new Io20002Template(new Io20002TemplateFormat()),
            IoType.IO20003 => new Io20003Template(new Io20003TemplateFormat()),
            IoType.IO20004 => new Io20004Template(new Io20004TemplateFormat()),
            IoType.IO30001 => new Io30001Template(new Io30001TemplateFormat()),
            IoType.IO30002 => new Io30002Template(new Io30002TemplateFormat()),
            IoType.IO30003 => new Io30003Template(new Io30003TemplateFormat()),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return template.GetMailTemplateParameters();
    }
}
