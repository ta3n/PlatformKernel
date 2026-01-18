using System.Text;
using Dapper;
using Liberty.ApplicationShared.Utils;
using System.Collections.Frozen;
using System.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Utils;
using SqlKata;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteAppDatePriceDataService(
    ILogger<PlanRoomGroupSiteAppDatePriceDataService> logger,
    IPlanRoomGroupSiteAppDatePriceDataRepository repository,
    QueryFactory queryFactory,
    IAppDateService appDateService,
    IBookingRoomAppDateService bookingRoomAppDateService,
    IUnitOfWork unitOfWork
) : BaseServiceRelation<PlanRoomGroupSiteAppDatePriceData>(logger, repository),
    IPlanRoomGroupSiteAppDatePriceDataService
{
    private readonly EntityProperty _planRoomGroupSiteAppDatePriceDataProp =
        unitOfWork.GetEntityProperty<PlanRoomGroupSiteAppDatePriceData>();

    private readonly EntityProperty _priceDataProp =
        unitOfWork.GetEntityProperty<PriceData>();

    private readonly struct PriceKey(
        long dateCalendar,
        int? personMin,
        int? personMax
    ) : IEquatable<PriceKey>
    {
        private readonly long _dateCalendar = dateCalendar;
        private readonly int? _personMin = personMin;
        private readonly int? _personMax = personMax;

        public bool Equals(
            PriceKey other
        )
        {
            return _dateCalendar == other._dateCalendar && _personMin == other._personMin && _personMax == other._personMax;
        }

        public override bool Equals(
            object? obj
        )
        {
            return obj is PriceKey k && Equals(k);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_dateCalendar, _personMin, _personMax);
        }
    }

    public async Task<(
        List<PlanRoomGroupSiteAppDatePriceData> addPriceOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDatePriceData> updatePriceOfSiteInPlanRooms
        )> ChangePriceDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteAppDatePriceData> listPriceOfSiteInPlanRoom,
        bool autoSave = true,
        bool isAdd = true,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!HasInput(listPriceOfSiteInPlanRoom))
        {
            return ([], []);
        }

        var (startDate, endDate) = GetDateRange(listPriceOfSiteInPlanRoom);

        var existing = await FindAllByPlanIdAndRomTypeIdAsync(
            planId,
            roomTypeId,
            siteId,
            startDate,
            endDate,
            transaction
        );

        var planCalendarDict = BuildDictionary(existing);

        ClassifyChanges(
            listPriceOfSiteInPlanRoom,
            planCalendarDict,
            out var adds,
            out var updates
        );

        await PersistChanges(
            adds,
            updates,
            isAdd,
            transaction,
            cancellationToken
        );

        return (isAdd ? adds : [], updates);
    }

    private static bool HasInput(
        List<PlanRoomGroupSiteAppDatePriceData>? input
    )
    {
        return input is not null && input.Count > 0;
    }

    private static (long startDate, long endDate) GetDateRange(
        List<PlanRoomGroupSiteAppDatePriceData> input
    )
    {
        var start = input.Min(a => a.DateCalendar);
        var end = input.Max(a => a.DateCalendar);
        return (start, end);
    }

    private static FrozenDictionary<PriceKey, PlanRoomGroupSiteAppDatePriceData> BuildDictionary(
        List<PlanRoomGroupSiteAppDatePriceData> existing
    )
    {
        return existing
            .Where(e => e.PriceData != null)
            .GroupBy(x => new PriceKey(x.DateCalendar, x.PriceData!.PersonMin, x.PriceData!.PersonMax))
            .ToFrozenDictionary(
                e => e.Key,
                e => e.First()
            );
    }

    private static void ClassifyChanges(
        List<PlanRoomGroupSiteAppDatePriceData> incomingList,
        FrozenDictionary<PriceKey, PlanRoomGroupSiteAppDatePriceData> existingLookup,
        out List<PlanRoomGroupSiteAppDatePriceData> adds,
        out List<PlanRoomGroupSiteAppDatePriceData> updates
    )
    {
        adds = new List<PlanRoomGroupSiteAppDatePriceData>(incomingList.Count);
        updates = new List<PlanRoomGroupSiteAppDatePriceData>(incomingList.Count);

        foreach (var incoming in incomingList)
        {
            if (incoming.PriceData is null)
            {
                continue;
            }

            var key = new PriceKey(
                incoming.DateCalendar,
                incoming.PriceData.PersonMin,
                incoming.PriceData.PersonMax
            );

            if (existingLookup.TryGetValue(key, out var target))
            {
                if (target.PriceData!.Price == incoming.PriceData.Price)
                {
                    continue;
                }

                target.PriceData.Price = incoming.PriceData.Price;
                updates.Add(target);
            }
            else
            {
                adds.Add(incoming);
            }
        }
    }

    private async Task PersistChanges(
        List<PlanRoomGroupSiteAppDatePriceData> adds,
        List<PlanRoomGroupSiteAppDatePriceData> updates,
        bool isAdd,
        IDbTransaction? transaction,
        CancellationToken cancellationToken
    )
    {
        if (isAdd && adds.Count > 0)
        {
            var appDateIds = adds
                .Select(x => x.DateCalendar)
                .Distinct()
                .ToArray();

            var notCreatedAppDateIds = await appDateService.FindNotCreatedAppDatesAsync(
                appDateIds,
                cancellationToken
            );

            if (notCreatedAppDateIds is {Count: > 0})
            {
                await bookingRoomAppDateService.BulkUpsertAppDateAsync(
                    notCreatedAppDateIds
                );
            }

            var timeNow = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

            var priceDatas = adds
                .Select(x => x.PriceData)
                .Distinct()
                .ToList();

            priceDatas.ForEach(x => x!.Code = $"{x.Code}{timeNow}");

            var priceDataInsert = await BulkUpsertPriceDataAsync([.. priceDatas.OfType<PriceData>()]);

            var dataPriceOfSiteInPlanRooms = BulkUpsert(adds, [.. priceDataInsert!], timeNow);

            await BulkUpsertPlanRoomSiteAppDateDataPriceAsync(dataPriceOfSiteInPlanRooms, transaction);
        }

        if (updates.Count > 0)
        {
            await UpdatePriceAsync(updates, transaction, cancellationToken);
        }
    }

    private async Task<List<PlanRoomGroupSiteAppDatePriceData>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
        long startDate,
        long endDate,
        IDbTransaction? transaction
    )
    {
        const string mainTable = "plan_room_group_site_app_date_price_data";
        const string priceTable = "price_data";

        var query = queryFactory.Query($"{mainTable} as p")
            .Join($"{priceTable} as pd", "pd.id", "p.price_data_id")
            .Where("p.plan_id", planId)
            .Where("p.room_group_id", romTypeId)
            .Where("p.site_id", siteId)
            .WhereBetween("p.date_calendar", startDate, endDate)
            .OrderByDesc("p.price_data_id")
            .Select(
                "p.*",
                "pd.id as pd_id",
                "pd.*"
            );

        var compiled = queryFactory.Compiler.Compile(query);

        var result = await queryFactory.Connection.QueryAsync<
            PlanRoomGroupSiteAppDatePriceData,
            PriceData,
            PlanRoomGroupSiteAppDatePriceData
        >(
            compiled.Sql,
            (
                p,
                price
            ) =>
            {
                p.PriceData = price;
                return p;
            },
            compiled.NamedBindings,
            splitOn: "pd_id",
            transaction: transaction
        );

        return [.. result];
    }

    private async Task BulkUpsertPlanRoomSiteAppDateDataPriceAsync(
        List<PlanRoomGroupSiteAppDatePriceData> planRoomGroupSiteAppDatePriceData,
        IDbTransaction? transaction
    )
    {
        if (planRoomGroupSiteAppDatePriceData.Count == 0)
        {
            return;
        }

        static string EscapeColumn(
            string column
        )
        {
            return "\"" + column.Replace("\"", "\"\"") + "\"";
        }

        var columns = new[]
        {
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PlanId)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.RoomGroupId)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.SiteId)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.DateCalendar)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PriceDataId)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.IsEnabled)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.IsVisible)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.IsDeleted)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.DisplayOrder)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.CreatedAt)),
            _planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.UpdatedAt))
        };

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var dataParams = planRoomGroupSiteAppDatePriceData.Select(x => new object?[]
                {
                    x.PlanId,
                    x.RoomGroupId,
                    x.SiteId,
                    x.DateCalendar,
                    x.PriceDataId,
                    true, // IsEnabled
                    true, // IsVisible
                    false, // IsDeleted
                    now, // DisplayOrder
                    now, // CreatedAt
                    now // UpdatedAt
                }
            )
            .ToList();

        var insertQuery = new Query(_planRoomGroupSiteAppDatePriceDataProp.TableName)
            .AsInsert(columns, dataParams);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var updateFields = new[] {_planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.UpdatedAt))};

        var sb = new StringBuilder();
        sb.AppendLine(compiled.Sql);
        sb.AppendLine("ON CONFLICT (");
        sb.Append("    ")
            .Append(EscapeColumn(_planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PlanId))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.RoomGroupId))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.SiteId))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.DateCalendar))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDatePriceDataProp.ColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PriceDataId))));
        sb.AppendLine(")");
        sb.AppendLine("DO UPDATE SET");
        sb.AppendLine(
            string.Join(
                ",\n    ",
                updateFields.Select(f => $"{EscapeColumn(f)} = EXCLUDED.{EscapeColumn(f)}")
            )
        );
        sb.Append(';');

        var sql = sb.ToString();

        await queryFactory.Connection.ExecuteAsync(sql, compiled.NamedBindings, transaction);
    }

    private static List<PlanRoomGroupSiteAppDatePriceData> BulkUpsert(
        List<PlanRoomGroupSiteAppDatePriceData> planRoomGroupSiteAppDatePriceData,
        List<PriceData> priceDatas,
        long timeNow
    )
    {
        foreach (var appDatePriceData in planRoomGroupSiteAppDatePriceData)
        {
            var key =
                $"{appDatePriceData.PlanId}{appDatePriceData.RoomGroupId}{appDatePriceData.SiteId}{appDatePriceData.DateCalendar}{appDatePriceData.PriceData!.PersonMin}{appDatePriceData.PriceData!.PersonMax}{appDatePriceData.PriceData!.Price}{timeNow}";
            var priceData = priceDatas.Find(x => x.Code == key);
            appDatePriceData.PriceDataId = priceData?.Id ?? 0;
        }

        return planRoomGroupSiteAppDatePriceData;
    }

    private async Task<List<PriceData>?> BulkUpsertPriceDataAsync(
        List<PriceData> priceDatas
    )
    {
        if (priceDatas.Count == 0)
        {
            return null;
        }

        var columns = new[]
        {
            _priceDataProp.ColumnName(nameof(PriceData.PersonMax)),
            _priceDataProp.ColumnName(nameof(PriceData.PersonMin)),
            _priceDataProp.ColumnName(nameof(PriceData.Price)),
            _priceDataProp.ColumnName(nameof(PriceData.IsEnabled)),
            _priceDataProp.ColumnName(nameof(PriceData.IsVisible)),
            _priceDataProp.ColumnName(nameof(PriceData.IsDeleted)),
            _priceDataProp.ColumnName(nameof(PriceData.DisplayOrder)),
            _priceDataProp.ColumnName(nameof(PriceData.CreatedAt)),
            _priceDataProp.ColumnName(nameof(PriceData.UpdatedAt)),
            _priceDataProp.ColumnName(nameof(PriceData.Code))
        };

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var dataParams = priceDatas.Select(x => new object?[]
                {
                    x.PersonMax,
                    x.PersonMin,
                    x.Price,
                    true, // IsEnabled
                    true, // IsVisible
                    false, // IsDeleted
                    now, // DisplayOrder
                    now, // CreatedAt
                    now, // UpdatedAt
                    x.Code
                }
            )
            .ToList();

        var insertQuery = new Query(_priceDataProp.TableName)
            .AsInsert(columns, dataParams);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var sql = $"{compiled.Sql} RETURNING *";

        var cmd = new CommandDefinition(
            commandText: sql,
            parameters: compiled.NamedBindings,
            transaction: null,
            cancellationToken: CancellationToken.None
        );

        var ids = (await queryFactory.Connection.QueryAsync<PriceData>(cmd)).ToList();
        return ids;
    }

    public async Task UpdatePriceAsync(
        List<PlanRoomGroupSiteAppDatePriceData> update,
        IDbTransaction? transaction,
        CancellationToken cancellationToken
    )
    {
        var priceDatas = update
            .Select(x => x.PriceData)
            .Distinct()
            .ToList();



        var table = _priceDataProp.TableName;
        var idCol = _priceDataProp.ColumnName(nameof(PriceData.Id));
        var priceCol = _priceDataProp.ColumnName(nameof(PriceData.Price));
        var updatedAtCol = _priceDataProp.ColumnName(nameof(PriceData.UpdatedAt));

        var sb = new StringBuilder();
        var bindings = new Dictionary<string, object?>();

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));
        sb.AppendLine($"""
            UPDATE {table} AS pd
            SET
              {priceCol} = v.price,
              {updatedAtCol} = v.updated_at
            FROM (VALUES
            """);

        for (var i = 0; i < priceDatas.Count; i++)
        {
            if (i > 0) sb.AppendLine(",");
            sb.Append($"(@Id{i}, @Price{i}, @UpdatedAt{i})");

            bindings[$"Id{i}"] = priceDatas[i]!.Id;
            bindings[$"Price{i}"] = priceDatas[i]!.Price;
            bindings[$"UpdatedAt{i}"] = now;
        }

        sb.AppendLine($"""
            ) AS v(id, price, updated_at)
            WHERE pd.{idCol} = v.id;
            """);

        var sql = sb.ToString();
        var cmd = new CommandDefinition(
            sql,
            bindings,
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        await queryFactory.Connection.ExecuteAsync(cmd);
    }
}
