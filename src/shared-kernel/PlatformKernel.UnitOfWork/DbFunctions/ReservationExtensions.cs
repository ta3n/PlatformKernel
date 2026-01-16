using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PlatformKernel.UnitOfWork.DbFunctions.Base;

namespace PlatformKernel.UnitOfWork.DbFunctions;

public static class ReservationExtensions
{
    private const string Extension = "This function is only for database queries.";

    public static string GetRootReservationCode(
        this long reservationId
    )
    {
        throw new NotSupportedException(Extension);
    }

    public static DateTime GetRootReservationDateTime(
        this long reservationId
    )
    {
        throw new NotSupportedException(Extension);
    }

    public static bool GetRegisterReservationDiffAmountGmoPayment(
        this long parentId
    )
    {
        throw new NotSupportedException(Extension);
    }

    public static long GetCheckOutDate(
        this long checkInDate,
        int restNumber
    )
    {
        throw new NotSupportedException(Extension);
    }
}

public class ReservationExtensionsRegister : IDbFunctionRegister
{
    public void RegisterFunctions(
        ModelBuilder modelBuilder
    )
    {
        RegisterGetRootReservationId(modelBuilder);
        RegisterGetRootReservationDateTime(modelBuilder);
        RegisterReservationDiffAmountGmoPayment(modelBuilder);
        RegisterGetCheckOutDate(modelBuilder);
    }

    private static void RegisterGetRootReservationId(
        ModelBuilder modelBuilder
    )
    {
        var method = typeof(ReservationExtensions).GetMethod(nameof(ReservationExtensions.GetRootReservationCode))!;

        modelBuilder
            .HasDbFunction(method)
            .HasTranslation(
                args =>
                    new SqlFunctionExpression(
                        "get_root_reservation_code",
                        args,
                        true,
                        [true],
                        typeof(string),
                        null
                    )
            );
    }

    private static void RegisterGetRootReservationDateTime(
        ModelBuilder modelBuilder
    )
    {
        var method = typeof(ReservationExtensions).GetMethod(nameof(ReservationExtensions.GetRootReservationDateTime))!;

        modelBuilder
            .HasDbFunction(method)
            .HasTranslation(
                args =>
                    new SqlFunctionExpression(
                        "get_root_reservation_datetime",
                        args,
                        true,
                        [true],
                        typeof(DateTime),
                        null
                    )
            );
    }

    private static void RegisterReservationDiffAmountGmoPayment(
        ModelBuilder modelBuilder
    )
    {
        var method = typeof(ReservationExtensions).GetMethod(nameof(ReservationExtensions.GetRegisterReservationDiffAmountGmoPayment))!;

        modelBuilder
            .HasDbFunction(method)
            .HasTranslation(
                args =>
                    new SqlFunctionExpression(
                        "reservation_diff_amount_gmo_online_payment",
                        args,
                        true,
                        [true],
                        typeof(bool),
                        null
                    )
            );
    }

    private static void RegisterGetCheckOutDate(
        ModelBuilder modelBuilder
    )
    {
        var method = typeof(ReservationExtensions).GetMethod(nameof(ReservationExtensions.GetCheckOutDate))!;

        modelBuilder
            .HasDbFunction(method)
            .HasTranslation(
                args =>
                    new SqlFunctionExpression(
                        "add_days_to_yyyymmdd",
                        args,
                        true,
                        [true],
                        typeof(long),
                        null
                    )
            );
    }
}
