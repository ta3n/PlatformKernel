using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.UnitOfWork.DbFunctions.Base;

namespace SharedKernel.UnitOfWork.DbFunctions;

/// <summary>
/// Provides a set of extension methods for handling and querying JSONB data in a PostgreSQL
/// database through Entity Framework Core.
/// </summary>
/// <remarks>
/// This class is designed to enable database operations using JSONB-specific features in
/// PostgreSQL. It is intended for use in LINQ queries to generate appropriate SQL functions
/// for interacting with JSONB data.
/// </remarks>
public static class JsonbExtensions
{
    /// <summary>
    /// Retrieves the value associated with the specified key from a JSONB object represented as a dictionary.
    /// This method is intended for use in database queries and relies on PostgreSQL's jsonb_extract_path_text function.
    /// </summary>
    /// <param name="jsonb">A dictionary representing a JSONB object. Can be null.</param>
    /// <param name="key">The key for which the associated value is to be retrieved from the JSONB object.</param>
    /// <returns>The value associated with the specified key as a string, or null if the key does not exist or the value is not found.</returns>
    /// <exception cref="NotSupportedException">Thrown if this method is called outside the context of a database query.</exception>
    public static string JsonGetKey(
        this Dictionary<string, string>? jsonb,
        string key
    )
    {
        throw new NotSupportedException("This function is only for database queries.");
    }

    /// <summary>
    /// Searches all values in a JSONB object for a specified search term. This method is designed
    /// for use in database queries and relies on PostgreSQL's jsonb_search_all_values function.
    /// </summary>
    /// <param name="json">A dictionary representing a JSONB object. Can be null.</param>
    /// <param name="searchTerm">The term to search for within all values of the JSONB object.</param>
    /// <returns>True if the search term is found in any value of the JSONB object, otherwise false.</returns>
    /// <exception cref="NotSupportedException">Thrown if this method is called outside the context of a database query.</exception>
    public static bool JsonSearchAllValues(
        this Dictionary<string, string>? json,
        string searchTerm
    )
    {
        throw new NotSupportedException("This function is only for database queries.");
    }
}

/// <summary>
/// Represents a registration process for PostgreSQL JSONB-specific DbFunctions
/// in an Entity Framework Core model. Implements the <see cref="IDbFunctionRegister"/>
/// interface to dynamically associate database functions with C# methods.
/// </summary>
public class JsonbExtensionsRegister : IDbFunctionRegister
{
    /// <summary>
    /// Registers database functions specific to JSONB operations for the Entity Framework Core model builder.
    /// </summary>
    /// <param name="modelBuilder">
    /// The <see cref="ModelBuilder"/> instance used to configure the Entity Framework Core model.
    /// </param>
    public void RegisterFunctions(
        ModelBuilder modelBuilder
    )
    {
        RegisterJsonGetKeyFunction(modelBuilder);
        RegisterJsonSearchAllValuesFunction(modelBuilder);
    }

    /// <summary>
    /// Registers the DbFunction mapping for the JsonGetKey method to enable its use
    /// as a PostgreSQL jsonb_extract_path_text function in Entity Framework Core queries.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder used for configuring the Entity Framework Core model.</param>
    private static void RegisterJsonGetKeyFunction(
        ModelBuilder modelBuilder
    )
    {
        var jsonGetKeyMethodInfo = typeof(JsonbExtensions).GetMethod(nameof(JsonbExtensions.JsonGetKey));

        modelBuilder
            .HasDbFunction(jsonGetKeyMethodInfo!)
            .HasTranslation(
                args => new SqlFunctionExpression(
                    "jsonb_extract_path_text", // PostgreSQL function
                    [.. args],
                    true, // Value can be NULL
                    [true, true],
                    typeof(string),
                    new StringTypeMapping("text", System.Data.DbType.String)
                )
            );
    }

    /// <summary>
    /// Registers the `jsonb_search_all_values` PostgreSQL database function in an Entity Framework Core model.
    /// Associates the DbFunction with the JsonSearchAllValues method in the JsonbExtensions class,
    /// enabling queries to utilize the function directly within LINQ expressions.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance used to configure the Entity Framework Core model.</param>
    private static void RegisterJsonSearchAllValuesFunction(
        ModelBuilder modelBuilder
    )
    {
        var jsonbSearchAllValuesMethodInfo = typeof(JsonbExtensions).GetMethod(nameof(JsonbExtensions.JsonSearchAllValues));

        modelBuilder
            .HasDbFunction(jsonbSearchAllValuesMethodInfo!)
            .HasTranslation(
                args => new SqlFunctionExpression(
                    "jsonb_search_all_values",
                    args,
                    true,
                    [true, true],
                    typeof(bool),
                    null
                )
            );
    }
}
