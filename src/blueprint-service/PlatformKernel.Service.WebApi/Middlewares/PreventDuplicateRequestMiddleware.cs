using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using PlatformKernel.ApplicationShared.Extensions;
using PlatformKernel.ApplicationShared.Settings;
using PlatformKernel.Cache.Services;
using PlatformKernel.ServiceDefaults.Middlewares;
using StackExchange.Redis;

namespace PlatformKernel.Service.WebApi.Middlewares;

public class PreventDuplicateRequestMiddleware(
    ICacheService cacheService
) : BaseMiddleware
{
    private readonly IDatabase _redis = cacheService.GetDatabase();
    private const int LockSeconds = 3;

    private static readonly string[] AllowedMethods =
    [
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    ];

    private static readonly DeduplicationExcludeRule[] ExcludeRules =
    [
        new()
        {
            Method = HttpMethods.Put,
            PathPattern = "*order*"
        },
        new()
        {
            Method = HttpMethods.Post,
            PathPattern = "*price-calendar*"
        },
        new()
        {
            Method = HttpMethods.Post,
            PathPattern = "*reservations/search*"
        },
        new()
        {
            Method = HttpMethods.Post,
            PathPattern = "*plan-prices/*/room-groups/*/destinations/*?"
        }
    ];

    protected override async Task HandleAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        var shouldBypass = !AllowedMethods.Contains(context.Request.Method)
            || Array.Exists(
                ExcludeRules,
                rule =>
                    (rule.Method == null || rule.Method == context.Request.Method)
                    && context.Request.Path.MatchesPattern(rule.PathPattern)
            );

        if (shouldBypass)
        {
            await next(context);
            return;
        }

        context.Request.EnableBuffering();
        using var hasher = SHA256.Create();
        var hashBytes = await hasher.ComputeHashAsync(context.Request.Body);
        context.Request.Body.Position = 0;
        var hash = Convert.ToHexString(hashBytes);
        var cacheKey = $"dedup:req:{context.Request.Path}{context.Request.QueryString}:{hash}";

        var locked = await _redis.StringSetAsync(
            cacheKey,
            "1",
            TimeSpan.FromSeconds(LockSeconds),
            When.NotExists
        );

        if (!locked)
        {
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            var appInfo = configuration.GetOptionsExt<AppInfo>("App");

            var problemDetails = new ProblemDetails
            {
                Title = "Duplicate request is being processed",
                Status = StatusCodes.Status429TooManyRequests,
                Detail = "The same request payload is already being processed. Please wait a moment before retrying.",
                Extensions =
                {
                    ["code"] = "DUPLICATE_REQUEST",
                    ["app"] = CreateAppInfo(appInfo)
                }
            };

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails);

            return;
        }

        await next(context);
    }

    private static object CreateAppInfo(
        AppInfo appInfo
    )
    {
        return new
        {
            name = appInfo.AppName ?? string.Empty,
            version = appInfo.AppVersion ?? string.Empty,
            dateUtc = DateTime.UtcNow.ToString("o")
        };
    }

    private sealed class DeduplicationExcludeRule
    {
        public string? Method { get; set; }
        public string PathPattern { get; set; } = string.Empty;
    }
}
