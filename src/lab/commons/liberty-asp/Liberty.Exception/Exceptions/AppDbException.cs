using System.Data.Common;

namespace Liberty.Exception.Exceptions;

public class AppDbException(
    DbException? dbException
) : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "DB Error";

    public DbException? DbException { get; } = dbException;
}
