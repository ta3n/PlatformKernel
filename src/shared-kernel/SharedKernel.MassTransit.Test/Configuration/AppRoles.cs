namespace SharedKernel.MassTransit.Test.Configuration;

public static class AppRoles
{
    public const string Api = "Api";
    public const string Processor = "Processor";
    public const string Saga = "Saga";

    public static bool IsApi(
        string role
    ) => string.Equals(role, Api, StringComparison.OrdinalIgnoreCase);

    public static string Normalize(
        string? role
    )
    {
        if (string.Equals(role, Processor, StringComparison.OrdinalIgnoreCase))
        {
            return Processor;
        }

        if (string.Equals(role, Saga, StringComparison.OrdinalIgnoreCase))
        {
            return Saga;
        }

        return Api;
    }
}
