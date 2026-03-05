using Liberty.Cache.Services;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class MailTypeService(
    ICacheService cacheService
) : IMailTypeService
{
    private readonly IEnumerable<(string IoType, string Name)> _mailTypes =
    [
        new(IoType.IO10001, "IO10001 一般向け 予約確認"),
        new(IoType.IO10001En, "IO10001En 一般向け 予約確認"),
        new(IoType.IO10003, "IO10003 予約成立(施設)"),
        new(IoType.IO10004, "IO10004 予約成立(ユーザ)"),
        new(IoType.IO10004En, "IO10004En 予約成立(ユーザ)"),
        new(IoType.IO10005, "IO10005 予約キャンセル(施設)"),
        new(IoType.IO10006, "IO10006 予約キャンセル(ユーザ)"),
        new(IoType.IO10006En, "IO10006En 予約キャンセル(ユーザ)"),
        new(IoType.IO10007, "IO10007 予約変更(施設)"),
        new(IoType.IO10008, "IO10008 予約変更(ユーザ)"),
        new(IoType.IO10008En, "IO10008En 予約変更(ユーザ)"),
        new(IoType.IO10010, "IO10010 予約変更（ゲスト)"),
        new(IoType.IO10010En, "IO10010En 予約変更（ゲスト)"),
        new(IoType.IO10011, "IO10011 予約キャンセル（ゲスト"),
        new(IoType.IO10011En, "IO10011En 予約キャンセル（ゲスト"),
        new(IoType.IO10012, "IO10012 予約成立（ゲスト）"),
        new(IoType.IO10012En, "IO10012En 予約成立（ゲスト）"),
        new(IoType.IO10013, "IO10013 Noshow(施設)"),
        new(IoType.IO10014, "IO10014 Noshow(ユーザ)"),
        new(IoType.IO10014En, "IO10014En Noshow(ユーザ)"),
        new(IoType.IO10015, "IO10015 Noshow（ゲスト)"),
        new(IoType.IO10015En, "IO10015En Noshow（ゲスト)"),
        new(IoType.IO10101, "IO10101 予約リマインダ(ユーザ) "),
        new(IoType.IO10101En, "IO10101En 予約リマインダ(ユーザ) "),
        new(IoType.IO10102, "IO10102 キャンセル料発生リマインダ(ユーザ)"),
        new(IoType.IO10102En, "IO10102En キャンセル料発生リマインダ(ユーザ)"),
        new(IoType.IO10103, "IO10103 予約リマインダ(ゲスト)"),
        new(IoType.IO10103En, "IO10103En 予約リマインダ(ゲスト)"),
        new(IoType.IO10104, "IO10104 キャンセル料発生リマインダ(ゲスト)"),
        new(IoType.IO10104En, "IO10104En キャンセル料発生リマインダ(ゲスト)")
    ];

    public IEnumerable<(string IoType, string Name)> FindAll()
    {
        const string cacheKey = "MailType";
        var mailTypes = cacheService.Get<IEnumerable<(string IoType, string Name)>>(
            cacheKey
        );

        if (mailTypes is not null)
        {
            return mailTypes;
        }

        mailTypes = _mailTypes;

        cacheService.Set(
            cacheKey,
            _mailTypes,
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(30) }
        );

        return mailTypes;
    }
}
