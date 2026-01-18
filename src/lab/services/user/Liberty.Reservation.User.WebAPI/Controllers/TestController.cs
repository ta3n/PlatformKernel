using Liberty.Reservation.Employee.WebAPI.Application.Controllers;
using Liberty.Reservation.Employee.WebAPI.Application.Controllers.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Controllers.Responses;
using Liberty.Reservation.Employee.WebAPI.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Liberty.Reservation.Employee.WebAPI.Controllers;

/// <summary>
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController(
    ILogger<TestController> logger,
    IConfiguration configuration,
    ITestController testController
)
    : BaseController
{
    protected readonly ITestController _Controller = testController;
    private readonly ILogger<TestController> _logger = logger;

    /// <summary>
    /// �A�v���P�[�V��������\�����܂�
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResponseTestGet> Get()
    {
        var setting = configuration.Get<AppSetting>();

        var response = await _Controller.Get();

        // �ݒ���͂����ŏ���������
        response.AppName = setting.App.AppName;
        response.AppVersion = setting.App.AppVersion;

        return response;
    }

    [HttpGet("error")]
    public async Task<ResponseTestErrorGet> GetError(
        string? errorCode
    )
    {
        return await _Controller.GetError(errorCode);
    }

    [HttpPost]
    public async Task<ResponseTestPost> Create(
        RequestTestPost request
    )
    {
        return await _Controller.Create(request);
    }

    [HttpPut]
    public async Task Update(
        RequestTestPut request
    )
    {
        await _Controller.Update(request);
    }

    [HttpDelete]
    public async Task Delete(
        RequestTestDelete request
    )
    {
        await _Controller.Delete(request);
    }
}
