using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace SharedKernel.Pagination.Extensions;

public static class QueryStringExtension
{
    public static string? GetParameter(
        this QueryString query,
        string name
    )
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var parameters = QueryHelpers.ParseQuery(query.ToString());
        return parameters.TryGetValue(name, out var parameter) ? parameter[0] : null;
    }

    public static string?[] GetParameterValues(
        this QueryString query,
        string name
    )
    {
        if (string.IsNullOrEmpty(name))
        {
            return [];
        }

        var parameters = QueryHelpers.ParseQuery(query.ToString());
        return parameters.TryGetValue(name, out var value) ? [.. value] : [];
    }
}
