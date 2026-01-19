using Microsoft.AspNetCore.Http;

namespace BlueprintCqrs.Infrastructure.Web.Rest.Utilities;

public static class HeaderUtil
{
    private const string ApplicationName = "blueprintCqrsApp";

    public static IHeaderDictionary CreateAlert(
        string message,
        string param
    )
    {
        IHeaderDictionary headers = new HeaderDictionary();
        headers.Add($"X-{ApplicationName}-alert", message);
        headers.Add($"X-{ApplicationName}-params", param);
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
        //log.error("Entity processing failed, {}", defaultMessage);
        IHeaderDictionary headers = new HeaderDictionary();
        headers.Add($"X-{ApplicationName}-error", $"error.{errorKey}");
        headers.Add($"X-{ApplicationName}-params", entityName);
        return headers;
    }
}
