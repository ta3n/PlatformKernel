using Liberty.UnitOfWork.DbFunctions.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Liberty.UnitOfWork.DbFunctions;

public static class DateTimeExtensions
{
    public static DateTime GetDateTime(
        this long appDateId,
        TimeSpan? checkOutTime
    )
    {
        throw new NotSupportedException("This function is only for database queries.");
    }
}

public class AppDateExtensionsRegister : IDbFunctionRegister
{
    public void RegisterFunctions(
        ModelBuilder modelBuilder
    )
    {
        RegisterAppDateGetDateTimeFunction(modelBuilder);
    }

    private static void RegisterAppDateGetDateTimeFunction(
        ModelBuilder modelBuilder
    )
    {
        var appDateGetDatetimeMethodInfo = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.GetDateTime));

        modelBuilder
            .HasDbFunction(appDateGetDatetimeMethodInfo!)
            .HasTranslation(
                args => new SqlFunctionExpression(
                    "get_date_time",
                    args,
                    true,
                    [true, true, true],
                    typeof(DateTime),
                    null
                )
            );
    }
}
