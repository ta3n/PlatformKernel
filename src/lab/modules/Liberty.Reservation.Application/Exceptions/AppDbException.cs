using Liberty.Reservation.Application.Constants;
using System.Data.Common;

namespace Liberty.Reservation.Application.Exceptions;

public class AppDbException(
    DbException? dbException
) : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "DB Error";

    public DbException? DbException { get; } = dbException;
}
