using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SharedKernel.UnitOfWork.DbFunctions.Base;

namespace SharedKernel.UnitOfWork.DbFunctions;

public static class TimeExtensions
{
    public static DateTime AddHourOffset(
        this DateTime ts,
        TimeSpan? facilityOffset
    )
    {
        throw new NotSupportedException("This function is only for database queries.");
    }
}

public class TimeExtensionsRegister : IDbFunctionRegister
{
    public void RegisterFunctions(
        ModelBuilder modelBuilder
    )
    {
        RegisterAddHourOffsetFunction(modelBuilder);
    }
  
    private static void RegisterAddHourOffsetFunction(
        ModelBuilder modelBuilder
    )
    {
        var addHourOffsetMethodInfo =
            typeof(TimeExtensions).GetMethod(nameof(TimeExtensions.AddHourOffset));

        modelBuilder
            .HasDbFunction(addHourOffsetMethodInfo!)
            .HasTranslation(
                args => new SqlFunctionExpression(
                    "add_hour_offset",
                    args,
                    true,
                    [true, true],
                    typeof(DateTime),
                    null
                )
            );
    }
}
