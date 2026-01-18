using System.Data;
using System.Text;
using Dapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Utils;
using SqlKata;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupSiteAppDateService(
    ILogger<PlanRoomGroupSiteAppDateService> logger,
    IPlanRoomGroupSiteAppDateRepository repository,
    QueryFactory queryFactory,
    IUnitOfWork unitOfWork
) : BaseServiceRelation<PlanRoomGroupSiteAppDate>(logger, repository),
    IPlanRoomGroupSiteAppDateService
{
    private readonly EntityProperty _planRoomGroupSiteAppDateProp =
        unitOfWork.GetEntityProperty<PlanRoomGroupSiteAppDate>();

    public async Task<(
        List<PlanRoomGroupSiteAppDate> addDateOfSiteInPlanRooms,
        List<PlanRoomGroupSiteAppDate> updateDateOfSiteInPlanRooms
        )> ChangeDateDataOfSiteInPlanRoom(
        long planId,
        long roomTypeId,
        long siteId,
        List<PlanRoomGroupSiteAppDate> listAppDateOfSiteInPlanRoom,
        bool autoSave = true,
        bool isAdd = true,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        var startDate = listAppDateOfSiteInPlanRoom.Select(a => a.DateCalendar).Min();
        var endDate = listAppDateOfSiteInPlanRoom.Select(a => a.DateCalendar).Max();

        var appDateOfSiteInPlanRooms = await FindAllByPlanIdAndRomTypeIdAsync(
            planId,
            roomTypeId,
            siteId,
            startDate,
            endDate,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<PlanRoomGroupSiteAppDate>(
            (
                x,
                y
            ) => x!.DateCalendar == y?.DateCalendar,
            obj => obj.DateCalendar.GetHashCode()
        );

        var updateDateOfSiteInPlanRooms = appDateOfSiteInPlanRooms
            .GroupBy(item => item, comparer)
            .SelectMany(group =>
                group.Take(
                    listAppDateOfSiteInPlanRoom.Count(updateItem => comparer.Equals(updateItem, group.Key)
                    )
                )
            )
            .ToList();

        var addDateOfSiteInPlanRooms = listAppDateOfSiteInPlanRoom
            .GroupBy(item => item, comparer)
            .SelectMany(group =>
                group.Skip(
                    updateDateOfSiteInPlanRooms.Count(updateItem => comparer.Equals(updateItem, group.Key)
                    )
                )
            )
            .ToList();

        foreach (var dateData in updateDateOfSiteInPlanRooms)
        {
            var matchingDate = listAppDateOfSiteInPlanRoom.Find(p => comparer.Equals(p, dateData));
            if (matchingDate is not null)
            {
                dateData.UseAutoDiscount = matchingDate.UseAutoDiscount;
                dateData.PointRate = matchingDate.PointRate;
                listAppDateOfSiteInPlanRoom.Remove(matchingDate);
            }
        }

        if (addDateOfSiteInPlanRooms.Count > 0)
        {
            await BulkUpsertPlanRoomSiteAppDateDataPriceAsync(addDateOfSiteInPlanRooms, transaction);
        }

        if (updateDateOfSiteInPlanRooms.Count > 0)
        {
            await UpdatePriceAsync(addDateOfSiteInPlanRooms, transaction, cancellationToken);
        }

        return (isAdd ? addDateOfSiteInPlanRooms : [], updateDateOfSiteInPlanRooms);
    }

    public async Task<List<PlanRoomGroupSiteAppDate>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
        long startDate,
        long endDate,
        CancellationToken cancellationToken = default
    )
    {
        const string mainTable = "plan_room_group_site_app_date";
        var query = queryFactory.Query($"{mainTable} as p")
            .Where("p.plan_id", planId)
            .Where("p.room_group_id", romTypeId)
            .Where("p.site_id", siteId)
            .WhereBetween("p.date_calendar", startDate, endDate)
            .Select(
                "p.*"
            );
        var compiled = queryFactory.Compiler.Compile(query);
        var result = await queryFactory.Connection.QueryAsync<PlanRoomGroupSiteAppDate>(compiled.Sql, compiled.NamedBindings);
        return [.. result];
    }

    public async Task UpdatePriceAsync(
        List<PlanRoomGroupSiteAppDate> update,
        IDbTransaction? transaction,
        CancellationToken cancellationToken
    )
    {
        var list = update.ToList();
        if (list.Count == 0) return;
        var table = _planRoomGroupSiteAppDateProp.TableName;

        var planIdCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.PlanId));
        var roomGroupIdCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.RoomGroupId));
        var siteIdCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.SiteId));
        var dateCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.DateCalendar));

        var useAutoDiscountCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.UseAutoDiscount));
        var pointRateCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.PointRate));
        var updatedAtCol = _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.UpdatedAt));

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var sb = new StringBuilder();
        var bindings = new Dictionary<string, object?>(capacity: list.Count * 7);

        // UPDATE ... FROM (VALUES ...) (Postgres bulk update)
        sb.AppendLine(
            $"""
            UPDATE {table} AS p
            SET
              {useAutoDiscountCol} = v.use_auto_discount,
              {pointRateCol}       = v.point_rate::real,
              {updatedAtCol}       = v.updated_at
            FROM (VALUES
            """
        );

        for (var i = 0; i < list.Count; i++)
        {
            if (i > 0) sb.AppendLine(",");

            // VALUES: (plan_id, room_group_id, site_id, date_calendar, use_auto_discount, point_rate, updated_at)
            sb.Append($"(@PlanId{i}, @RoomGroupId{i}, @SiteId{i}, @Date{i}, @UseAutoDiscount{i}, @PointRate{i}, @UpdatedAt{i})");

            var row = list[i];

            bindings[$"PlanId{i}"] = row.PlanId;
            bindings[$"RoomGroupId{i}"] = row.RoomGroupId;
            bindings[$"SiteId{i}"] = row.SiteId;
            bindings[$"Date{i}"] = row.DateCalendar;

            bindings[$"UseAutoDiscount{i}"] = row.UseAutoDiscount;
            bindings[$"PointRate{i}"] = row.PointRate;
            bindings[$"UpdatedAt{i}"] = now;
        }

        sb.AppendLine(
            $"""
            ) AS v(plan_id, room_group_id, site_id, date_calendar, use_auto_discount, point_rate, updated_at)
            WHERE
              p.{planIdCol}      = v.plan_id
              AND p.{roomGroupIdCol} = v.room_group_id
              AND p.{siteIdCol}  = v.site_id
              AND p.{dateCol}    = v.date_calendar;
            """
        );

        var sql = sb.ToString();

        var cmd = new CommandDefinition(
            sql,
            bindings,
            transaction: transaction,
            cancellationToken: CancellationToken.None
        );

        await queryFactory.Connection.ExecuteAsync(cmd);
    }

    private async Task BulkUpsertPlanRoomSiteAppDateDataPriceAsync(
        List<PlanRoomGroupSiteAppDate> planRoomGroupSiteAppDate,
        IDbTransaction? transaction
    )
    {
        if (planRoomGroupSiteAppDate.Count == 0)
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
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.PlanId)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.RoomGroupId)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.SiteId)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.DateCalendar)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.UseAutoDiscount)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.PointRate)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.IsEnabled)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.IsVisible)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.IsDeleted)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.DisplayOrder)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.CreatedAt)),
            _planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.UpdatedAt))
        };

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var dataParams = planRoomGroupSiteAppDate.Select(x => new object?[]
                {
                    x.PlanId,
                    x.RoomGroupId,
                    x.SiteId,
                    x.DateCalendar,
                    x.UseAutoDiscount,
                    x.PointRate,
                    true, // IsEnabled
                    true, // IsVisible
                    false, // IsDeleted
                    now, // DisplayOrder
                    now, // CreatedAt
                    now // UpdatedAt
                }
            )
            .ToList();

        var insertQuery = new Query(_planRoomGroupSiteAppDateProp.TableName)
            .AsInsert(columns, dataParams);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var updateFields = new[] {_planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.UpdatedAt))};

        var sb = new StringBuilder();
        sb.AppendLine(compiled.Sql);
        sb.AppendLine("ON CONFLICT (");
        sb.Append("    ")
            .Append(EscapeColumn(_planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.PlanId))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.RoomGroupId))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.SiteId))))
            .Append(", ");
        sb.Append(EscapeColumn(_planRoomGroupSiteAppDateProp.ColumnName(nameof(PlanRoomGroupSiteAppDate.DateCalendar))));
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

        var cmd = new CommandDefinition(
            sql,
            compiled.NamedBindings,
            transaction: transaction,
            cancellationToken: CancellationToken.None
        );

        await queryFactory.Connection.ExecuteAsync(cmd);
    }
}
