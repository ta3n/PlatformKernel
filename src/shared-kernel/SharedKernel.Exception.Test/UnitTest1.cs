using System.Data.Common;
using SharedKernel.Exception;
using SharedKernel.Exception.Exceptions;

namespace SharedKernel.Exception.Test;

public class UnitTest1
{
    [Fact]
    public void GetAppException_MapsDbExceptionToAppDbException()
    {
        var exception = new FakeDbException("db");

        var result = AppException.GetAppException(exception);

        var appDbException = Assert.IsType<AppDbException>(result);
        Assert.Same(exception, appDbException.DbException);
        Assert.Equal(ErrorCode.E0101, appDbException.ErrorCode);
    }

    [Fact]
    public void GetAppException_ReturnsExistingAppExceptionInstance()
    {
        var exception = new AppBaseException("message");

        var result = AppException.GetAppException(exception);

        Assert.Same(exception, result);
    }

    [Fact]
    public void GetAppException_WrapsUnknownException()
    {
        var exception = new InvalidOperationException("boom");

        var result = AppException.GetAppException(exception);

        var unknown = Assert.IsType<AppUnknownErrorException>(result);
        Assert.Same(exception, unknown.Exception);
        Assert.Equal("Error the system", unknown.Title);
    }

    [Fact]
    public void TooManyConcurrentUploadException_UsesEnumDescriptionAsTitle()
    {
        var exception = new TooManyConcurrentUploadException();

        Assert.Equal(ErrorCode.E0104, exception.ErrorCode);
        Assert.Equal("System is currently processing too many concurrent requests", exception.Title);
    }

    private sealed class FakeDbException(string message) : DbException(message);
}
