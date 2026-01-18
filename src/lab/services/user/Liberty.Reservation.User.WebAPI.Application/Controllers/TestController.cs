using Liberty.Reservation.Employee.WebAPI.Application.Controllers.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Controllers.Requests;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Extensions;

namespace Liberty.Reservation.Employee.WebAPI.Application.Controllers;

public interface ITestController
{
    Task<ResponseTestGet> Get();

    Task<ResponseTestErrorGet> GetError(
        string? errorCode
    );

    Task<ResponseTestPost> Create(
        RequestTestPost request
    );

    Task Update(
        RequestTestPut request
    );

    Task Delete(
        RequestTestDelete request
    );
}

public class TestController : BaseController, ITestController
{
    public TestController()
    {
    }

    public async Task<ResponseTestGet> Get()
    {
        // FIXME: Getテスト
        return new ResponseTestGet
        {
            AppName = "AppName",
            AppVersion = "AppVersion",
            Date = DateTime.UtcNow
        };
    }

    public async Task<ResponseTestErrorGet> GetError(
        string? errorCode
    )
    {
        // throw するのでResponseは実質不要

        var code = (ErrorCodes)Enum.Parse(typeof(ErrorCodes), errorCode);

        var exception = code.GetException();
        throw exception;
    }

    public async Task<ResponseTestPost> Create(
        RequestTestPost request
    )
    {
        // FIXME: Create(Post)テスト
        return new ResponseTestPost
        {
        };
    }

    public async Task Update(
        RequestTestPut request
    )
    {
        // FIXME: Put(Update)テスト
    }

    public async Task Delete(
        RequestTestDelete request
    )
    {
        // FIXME: Deleteテスト
    }
}
