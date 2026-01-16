using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PlatformKernel.UnitOfWork.DbFunctions.Base;

namespace PlatformKernel.UnitOfWork.DbFunctions;

public static class TimeZoneExtensions
{
    public static DateTime ApplyTimezoneOffset(
        this DateTime ts,
        string? timezone
    )
    {
        throw new NotSupportedException("This function is only for database translation.");
    }
}

public class TimeZoneExtensionsRegister : IDbFunctionRegister
{
    public void RegisterFunctions(
        ModelBuilder modelBuilder
    )
    {
        var method = typeof(TimeZoneExtensions).GetMethod(nameof(TimeZoneExtensions.ApplyTimezoneOffset))!;

        modelBuilder
            .HasDbFunction(method)
            .HasTranslation(
                args =>
                    new SqlFunctionExpression(
                        "apply_timezone_offset",
                        args,
                        true,
                        [true, true],
                        typeof(DateTime),
                        null
                    )
            );
    }
}
