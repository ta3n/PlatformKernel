using System.Data.Common;

namespace Liberty.SysException.Exceptions;

public class AppDbException(
    DbException? dbException
) : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E0101;

    public override string Title => "DB Error";

    public DbException? DbException { get; } = dbException;
}
