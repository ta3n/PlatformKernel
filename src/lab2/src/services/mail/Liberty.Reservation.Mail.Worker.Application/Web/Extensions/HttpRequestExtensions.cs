using System.Text;
using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Mail.Worker.Application.Web.Extensions;

public static class HttpRequestExtensions
{
    public static async Task<string> BodyAsStringAsync(
        this HttpRequest request,
        Encoding? encoding = null
    )
    {
        encoding ??= Encoding.UTF8;

        using var reader = new StreamReader(request.Body, encoding);
        return await reader.ReadToEndAsync();
    }
}
