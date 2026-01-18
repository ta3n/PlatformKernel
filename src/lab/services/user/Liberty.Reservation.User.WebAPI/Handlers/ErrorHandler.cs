using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;

namespace Liberty.Reservation.Employee.WebAPI.Handlers;

public class ErrorHandler
{
    public static async Task HandleRequest(
        HttpContext context
    )
    {
        var serviceProvider = context.RequestServices;

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var appSetting = configuration.Get<AppSetting>();

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        var appException = AppException.GetAppException(exceptionHandlerPathFeature.Error);

        // FIXME: とりあえずエラーログ
        Console.WriteLine(appException.Message);

        // 変換エラーがある可能性があるので、握り潰し処理もしておく
        var title = appException.Title;
        var description = appException.Message;
        try
        {
            var i18nEData = I18nE.Localizer2(appException.ErrorCode.ToString());

            title = i18nEData?.Title;
            description = i18nEData?.Description;
        }
        catch (Exception)
        {
        }

        // 返却するJSONデータを成形
        var ex = new
        {
            // Exception
            code = appException.ErrorCode.ToString(),
            title,
            description,

            app = new
            {
                name = appSetting.App.AppName,
                version = appSetting.App.AppVersion,
                date = DateTime.UtcNow.ToString()
            }
        };
        // json化
        var result = JsonConvert.SerializeObject(ex);

        // ステータスコード  通常は500:InternalServerError
        var statusCode = HttpStatusCode.InternalServerError;
        if (appException is AppNotfoundException) statusCode = HttpStatusCode.NotFound;
        if (appException is AppInvalidException) statusCode = HttpStatusCode.BadRequest;
        if (appException is AppAuthException) statusCode = HttpStatusCode.Unauthorized;
        //
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(result);
    }
}
