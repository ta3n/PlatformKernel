using Dapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Application.Domains.Repositories;
using SqlKata;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSitePriceDataService(
    ILogger<PlanRoomGroupSitePriceDataService> logger,
    IPlanRoomGroupSitePriceDataRepository repository,
    QueryFactory queryFactory,
    IUnitOfWork unitOfWork
)
    : BaseServiceRelation<PlanRoomGroupSitePriceData>(logger, repository),
        IPlanRoomGroupSitePriceDataService
{
    private readonly EntityProperty _planRoomGroupSiteAppDatePriceDataProp =
        unitOfWork.GetEntityProperty<PlanRoomGroupSiteAppDatePriceData>();

    private readonly EntityProperty _priceDataProp =
        unitOfWork.GetEntityProperty<PriceData>();

    private readonly EntityProperty _discountDataProp =
        unitOfWork.GetEntityProperty<DiscountData>();

    private readonly EntityProperty _planRoomGroupSiteDiscountDataProp =
        unitOfWork.GetEntityProperty<PlanRoomGroupSiteDiscountData>();

    private sealed record PriceDataKey(
        long PlanId,
        long RoomGroupId,
        long SiteId,
        int? PersonMin,
        int? PersonMax
    );

    public async Task<(List<PlanRoomGroupSitePriceData> addPriceDataOfSiteInPlanRooms, List<PlanRoomGroupSitePriceData>
        removeDataOfSiteInPlanRooms)> ChangeRangePersonOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSitePriceData> listPriceDataOfSiteInPlanRoom,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var rangePriceDataOfSiteInPlanRoom =
            await FindAllRangePriceByPlanIdAndRomTypeIdAsync(
                planId,
                roomTypeId,
                siteId,
                cancellationToken
            );

        var comparer = new DelegateEqualityComparer<PlanRoomGroupSitePriceData>(
            (
                    x,
                    y
                ) =>
                x!.PersonMin == y!.PersonMin
                && x.PersonMax == y.PersonMax,
            obj =>
                obj.PersonMin.GetHashCode()
                ^ obj.PersonMax.GetHashCode()
        );

        var removeDataOfSiteInPlanRooms = rangePriceDataOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        listPriceDataOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();

        var addPriceDataOfSiteInPlanRooms = listPriceDataOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(
                group =>
                    group.Skip(
                        rangePriceDataOfSiteInPlanRoom.Count(
                            updateItem => comparer.Equals(updateItem, group.Key)
                        )
                    )
            )
            .ToList();
        if (addPriceDataOfSiteInPlanRooms is { Count: > 0 })
        {
            _ = await CreateRangeAsync(addPriceDataOfSiteInPlanRooms, autoSave, cancellationToken);
        }

        if (removeDataOfSiteInPlanRooms is not { Count: > 0 })
        {
            return (
                addPriceDataOfSiteInPlanRooms,
                removeDataOfSiteInPlanRooms
            );
        }

        var concatList = addPriceDataOfSiteInPlanRooms.Concat(removeDataOfSiteInPlanRooms).ToList();

        await DeletePriceDataAsync(concatList);
        await DeleteDiscountAsync(concatList);

        await DeleteRangeAsync(
            removeDataOfSiteInPlanRooms,
            autoSave,
            cancellationToken
        );

        return (
            addPriceDataOfSiteInPlanRooms,
            removeDataOfSiteInPlanRooms
        );
    }

    private async Task DeletePriceDataAsync(
        List<PlanRoomGroupSitePriceData> concatList
    )
    {
        if (ExtractPriceDataKeysForDeletion(concatList, out var keysDelete))
        {
            return;
        }

        var query = new Query(_planRoomGroupSiteAppDatePriceDataProp.TableName).AsDelete();

        query.Where(
            q =>
            {
                foreach (var k in keysDelete)
                {
                    var local = k;

                    var subJoin = new Query(_priceDataProp.TableName)
                        .Select(_priceDataProp.ColumnName(nameof(PriceData.Id)))
                        .Where(_priceDataProp.ColumnName(nameof(PriceData.PersonMin)), local.PersonMin)
                        .Where(_priceDataProp.ColumnName(nameof(PriceData.PersonMax)), local.PersonMax);

                    q.OrWhere(
                        inner =>
                            inner
                                .Where(
                                    _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PlanId)),
                                    local.PlanId
                                )
                                .Where(
                                    _planRoomGroupSiteAppDatePriceDataProp.ColumnName(
                                        nameof(PlanRoomGroupSiteAppDatePriceData.RoomGroupId)
                                    ),
                                    local.RoomGroupId
                                )
                                .Where(
                                    _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.SiteId)),
                                    local.SiteId
                                )
                                .WhereIn(
                                    _planRoomGroupSiteAppDatePriceDataProp.ColumnName(
                                        nameof(PlanRoomGroupSiteAppDatePriceData.PriceDataId)
                                    ),
                                    subJoin
                                )
                    );
                }

                return q;
            }
        );

        var compiled = queryFactory.Compiler.Compile(query);
        await queryFactory.Connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
    }

    private async Task DeleteDiscountAsync(
        List<PlanRoomGroupSitePriceData> concatList
    )
    {
        if (ExtractPriceDataKeysForDeletion(concatList, out var keysDelete))
        {
            return;
        }

        var query = new Query(_planRoomGroupSiteDiscountDataProp.TableName).AsDelete();

        query.Where(
            q =>
            {
                foreach (var k in keysDelete)
                {
                    var local = k;

                    var subJoin = new Query(_discountDataProp.TableName)
                        .Select(_discountDataProp.ColumnName(nameof(DiscountData.Id)))
                        .Where(_discountDataProp.ColumnName(nameof(DiscountData.PersonMin)), local.PersonMin)
                        .Where(_discountDataProp.ColumnName(nameof(DiscountData.PersonMax)), local.PersonMax);

                    q.OrWhere(
                        inner =>
                            inner
                                .Where(
                                    _planRoomGroupSiteDiscountDataProp.ColumnName(nameof(PlanRoomGroupSiteDiscountData.PlanId)),
                                    local.PlanId
                                )
                                .Where(
                                    _planRoomGroupSiteDiscountDataProp.ColumnName(nameof(PlanRoomGroupSiteDiscountData.RoomGroupId)),
                                    local.RoomGroupId
                                )
                                .Where(
                                    _planRoomGroupSiteDiscountDataProp.ColumnName(nameof(PlanRoomGroupSiteDiscountData.SiteId)),
                                    local.SiteId
                                )
                                .WhereIn(
                                    _planRoomGroupSiteDiscountDataProp.ColumnName(nameof(PlanRoomGroupSiteDiscountData.DiscountDataId)),
                                    subJoin
                                )
                    );
                }

                return q;
            }
        );

        var compiled = queryFactory.Compiler.Compile(query);
        await queryFactory.Connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
    }

    private static bool ExtractPriceDataKeysForDeletion(
        List<PlanRoomGroupSitePriceData> concatList,
        out List<PriceDataKey> keysDelete
    )
    {
        keysDelete = concatList
            .Select(
                x => new PriceDataKey(
                    x.PlanId,
                    x.RoomGroupId,
                    x.SiteId,
                    x.PersonMin,
                    x.PersonMax
                )
            )
            .Distinct()
            .ToList();

        return keysDelete.Count == 0;
    }

    private async Task<
        List<PlanRoomGroupSitePriceData>
    > FindAllRangePriceByPlanIdAndRomTypeIdAsync(
        long planId,
        long roomTypeId,
        long siteId,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId && x.RoomGroupId == roomTypeId && x.SiteId == siteId)
            .ToListAsync(cancellationToken);
    }
}
