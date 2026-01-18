using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

public static class HeaderUtil
{
    private const string ApplicationName = "liberty";

    public static IHeaderDictionary CreateAlert(
        string message,
        string param
    )
    {
        IHeaderDictionary headers = new HeaderDictionary();
        headers.Append($"X-{ApplicationName}-alert", message);
        headers.Append($"X-{ApplicationName}-params", param);
        return headers;
    }

    public static IHeaderDictionary CreateEntityCreationAlert(
        string entityName,
        string param
    )
    {
        return CreateAlert($"{ApplicationName}.{entityName}.created", param);
    }

    public static IHeaderDictionary CreateEntityUpdateAlert(
        string entityName,
        string param
    )
    {
        return CreateAlert($"{ApplicationName}.{entityName}.updated", param);
    }

    public static IHeaderDictionary CreateEntityDeletionAlert(
        string entityName,
        string param
    )
    {
        return CreateAlert($"{ApplicationName}.{entityName}.deleted", param);
    }

    public static IHeaderDictionary CreateFailureAlert(
        string entityName,
        string errorKey,
        string defaultMessage
    )
    {
        IHeaderDictionary headers = new HeaderDictionary();
        headers.Append($"X-{ApplicationName}-error", $"error.{errorKey}");
        headers.Append($"X-{ApplicationName}-params", entityName);
        return headers;
    }
}
