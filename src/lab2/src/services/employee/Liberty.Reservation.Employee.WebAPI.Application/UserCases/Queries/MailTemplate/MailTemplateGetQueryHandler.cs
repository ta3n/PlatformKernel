using System.Linq.Expressions;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;

public class MailTemplateGetQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISystemConfigRepository systemConfigRepository
) : QuerySingleBaseHandler<MailTemplateGetQuery, MailTemplateResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        MailTemplateGetQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.SystemConfig),
            CacheHelper.ComputeHash(
                [
                    nameof(MailTemplateGetQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, MailTemplateResponse)> HandleAsync(
        MailTemplateGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = systemConfigRepository.GetQueryableWithAsNoTracking();
        var response = await GetTemplateMailResponse(query, request.IoType);

        return (new HeaderDictionary(), response);
    }

    private static async Task<MailTemplateResponse> GetTemplateMailResponse(
        IQueryable<Reservation.Application.Contexts.DataContexts.Entities.Data.SystemConfig> query,
        string type
    )
    {
        var templateSelectors = new Dictionary<
            string,
            Expression<Func<Reservation.Application.Contexts.DataContexts.Entities.Data.SystemConfig, MailTemplateResponse>>
        >
        {
            [IoType.IO10001] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10001TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10001TemplateFormat!.Body,
                    x.TemplateFormatData!.Io10001TemplateFormat!.Url
                ),
            [IoType.IO10001En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10001EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10001EnTemplateFormat!.Body,
                    x.TemplateFormatData!.Io10001EnTemplateFormat!.Url
                ),
            [IoType.IO10002] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10002TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10002TemplateFormat!.Body,
                    x.TemplateFormatData!.Io10002TemplateFormat!.Url
                ),
            [IoType.IO10003] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10003TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10003TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10004] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10004TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10004TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10004En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10004EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10004EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10005] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10005TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10005TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10006] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10006TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10006TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10006En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10006EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10006EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10007] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10007TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10007TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10008] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10008TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10008TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10008En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10008EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10008EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10009] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10009TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10009TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10010] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10010TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10010TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10010En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10010EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10010EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10011] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10011TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10011TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10011En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10011EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10011EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10012] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10012TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10012TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10012En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10012EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10012EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10013] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10013TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10013TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10014] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10014TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10014TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10014En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10014EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10014EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10015] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10014TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10014TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10015En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10015EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10015EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10101] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10101TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10101TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10101En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10101EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10101EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10102] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10102TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10102TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10102En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10102EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10102EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10103] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10103TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10103TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10103En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10103EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10103EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10104] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10104TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10104TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10104En] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10104EnTemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10104EnTemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10201] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10201TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10201TemplateFormat!.Body,
                    x.TemplateFormatData!.Io10201TemplateFormat!.Url
                ),
            [IoType.IO10202] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10001TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10001TemplateFormat!.Body,
                    x.TemplateFormatData!.Io10001TemplateFormat!.Url
                ),
            [IoType.IO10203] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10203TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10203TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10204] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10001TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10001TemplateFormat!.Body,
                    x.TemplateFormatData!.Io10001TemplateFormat!.Url
                ),
            [IoType.IO10205] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10205TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10205TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10206] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10206TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10206TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO10207] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io10207TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io10207TemplateFormat!.Body,
                    x.TemplateFormatData!.Io10207TemplateFormat!.Url
                ),
            [IoType.IO20001] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io20001TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io20001TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO20002] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io20002TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io20002TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO20003] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io20003TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io20003TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO20004] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io20004TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io20004TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO30001] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io30001TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io30001TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO30002] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io30002TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io30002TemplateFormat!.Body,
                    string.Empty
                ),
            [IoType.IO30003] = x =>
                new MailTemplateResponse(
                    x.TemplateFormatData!.Io30003TemplateFormat!.Subject,
                    x.TemplateFormatData!.Io30003TemplateFormat!.Body,
                    string.Empty
                )
        };

        if (!templateSelectors.TryGetValue(type, out var selector))
        {
            throw new SystemConfigNotfoundException();
        }

        var mailTemplate = await query
                .Select(selector)
                .SingleOrDefaultAsync()
            ?? throw new SystemConfigNotfoundException();

        mailTemplate.IoType = type;
        mailTemplate.Parameters = BaseMailParameter.GetTemplateMailParameters(type);
        return mailTemplate;
    }
}
