using System.Text;
using Dapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Utils;
using SqlKata;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CalendarAppDateAppDateTypeService(
    ILogger<CalendarAppDateAppDateTypeService> logger,
    ICalendarAppDateAppDateTypeRepository repository,
    QueryFactory queryFactory,
    IUnitOfWork unitOfWork
)
    : BaseServiceRelation<CalendarAppDateAppDateType>(logger, repository),
        ICalendarAppDateAppDateTypeService
{
    private readonly EntityProperty _calendarAppDateAppDateTypeProp = unitOfWork.GetEntityProperty<CalendarAppDateAppDateType>();

    public async Task<(
        List<CalendarAppDateAppDateType> addAppDateOfCalendars,
        List<CalendarAppDateAppDateType> removeAppDateOfCalendars
        )> ChangeCalendarAppDate(
        long calendarId,
        List<CalendarAppDateAppDateType> listCalendarAppDateAppDateType,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var listDateCalendar = listCalendarAppDateAppDateType
            .Select(x => x.DateCalendar)
            .ToList();

        var appDateOfCalendars = await GetCalendarAppDateByCalendarId(
            calendarId,
            listDateCalendar,
            cancellationToken
        );

        var comparer = new DelegateEqualityComparer<CalendarAppDateAppDateType>(
            (
                    x,
                    y
                ) =>
                x?.AppDateTypeId == y?.AppDateTypeId
                && x!.DateCalendar == y?.DateCalendar
                && x.IsDeleted == y.IsDeleted,
            obj =>
                obj.AppDateTypeId.GetHashCode()
                ^ obj.DateCalendar.GetHashCode()
                ^ obj.IsDeleted.GetHashCode()
        );

        var filteredListAdd = listCalendarAppDateAppDateType
            .Where(x => !x.IsDeleted)
            .ToList();

        var addAppDateOfCalendars = filteredListAdd
            .Except(appDateOfCalendars, comparer)
            .ToList();

        var removeAppDateOfCalendars = appDateOfCalendars
            .Except(listCalendarAppDateAppDateType, comparer)
            .ToList();
        if (addAppDateOfCalendars is { Count: > 0 })
        {
           await BulkUpsertCalendarAppDateAppDateTypeAsync(addAppDateOfCalendars);
        }

        if (removeAppDateOfCalendars is { Count: > 0 })
        {
           await BulkHardDeleteCalendarAppDateAppDateTypeAsync(removeAppDateOfCalendars);
        }

        return (addAppDateOfCalendars, removeAppDateOfCalendars);
    }

    /// <summary>
    /// Retrieves a list of CalendarAppDateAppDateType records associated with a specific calendar ID and a list of date calendar values.
    /// </summary>
    /// <param name="calendarId">
    /// The unique identifier of the calendar for which associated records are to be retrieved.
    /// </param>
    /// <param name="dateCalendars">
    /// A list of integer date calendar values to filter the records.
    /// </param>
    /// <param name="cancellationToken">
    /// A CancellationToken that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A Task representing the asynchronous operation, containing a list of CalendarAppDateAppDateType records matching the specified criteria.
    /// </returns>
    private async Task<List<CalendarAppDateAppDateType>> GetCalendarAppDateByCalendarId(
        long calendarId,
        List<int> dateCalendars,
        CancellationToken cancellationToken = default
    )
    {
        return await repository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CalendarId == calendarId)
            .Where(x => dateCalendars.Contains(x.DateCalendar))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Inserts or updates a list of CalendarAppDateAppDateType records in bulk using an upsert operation.
    /// </summary>
    /// <param name="calendarAppDateAppDateTypes">
    /// A list of CalendarAppDateAppDateType objects to be inserted or updated in the database.
    /// The method performs an "on conflict" resolution to update existing records with new data.
    /// </param>
    /// <returns>
    /// A Task representing the asynchronous operation. Does not return any value on completion.
    /// </returns>
    private async Task BulkUpsertCalendarAppDateAppDateTypeAsync(
        List<CalendarAppDateAppDateType> calendarAppDateAppDateTypes
    )
    {
        if (calendarAppDateAppDateTypes is { Count: 0 })
        {
            return;
        }

        var columns = new[]
        {
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.CalendarId)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.AppDateTypeId)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.DateCalendar)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.IsEnabled)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.IsVisible)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.IsDeleted)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.DisplayOrder)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.CreatedAt)),
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.UpdatedAt))
        };

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var data = calendarAppDateAppDateTypes.Select(
                x => new object?[] { x.CalendarId, x.AppDateTypeId, x.DateCalendar, true, true, false, now, now, now }
            )
            .ToList();

        var insertQuery = new Query(_calendarAppDateAppDateTypeProp.TableName)
            .AsInsert(columns, data);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var updateFields = new[] { _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.UpdatedAt)) };

        var sb = new StringBuilder();
        sb.AppendLine(compiled.Sql);
        sb.AppendLine("ON CONFLICT (");
        sb.Append("    ")
            .Append(EscapeColumn(_calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.CalendarId))))
            .Append(", ");
        sb.Append(EscapeColumn(_calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.AppDateTypeId)))).Append(", ");
        sb.Append(EscapeColumn(_calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.DateCalendar))));
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

        await queryFactory.Connection.ExecuteAsync(sql, compiled.NamedBindings);
    }

    /// <summary>
    /// Hard delete a list of CalendarAppDateAppDateType records using a bulk
    /// set-based operation.
    /// </summary>
    /// <remarks>
    /// This method does not perform any prior existence checks.
    /// Records that do not exist will simply not be affected,
    /// making the operation idempotent and safe for concurrent execution.
    /// </remarks>
    /// <param name="calendarAppDateAppDateTypes">
    /// The list of CalendarAppDateAppDateType records to be deleted.
    /// </param>
    /// <returns>
    /// A Task representing the asynchronous operation.
    /// </returns>
    private async Task BulkHardDeleteCalendarAppDateAppDateTypeAsync(
        List<CalendarAppDateAppDateType> calendarAppDateAppDateTypes
    )
    {
        if (calendarAppDateAppDateTypes is { Count: 0 })
        {
            return;
        }

        var table = _calendarAppDateAppDateTypeProp.TableName;

        var calendarIdCol =
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.CalendarId));
        var appDateTypeIdCol =
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.AppDateTypeId));
        var dateCalendarCol =
            _calendarAppDateAppDateTypeProp.ColumnName(nameof(CalendarAppDateAppDateType.DateCalendar));

        var sb = new StringBuilder();
        sb.AppendLine($"DELETE FROM {EscapeColumn(table)}");
        sb.AppendLine("WHERE (");
        sb.AppendLine($"    {EscapeColumn(calendarIdCol)},");
        sb.AppendLine($"    {EscapeColumn(appDateTypeIdCol)},");
        sb.AppendLine($"    {EscapeColumn(dateCalendarCol)}");
        sb.AppendLine(") IN (");

        var parameters = new DynamicParameters();

        for (int i = 0; i < calendarAppDateAppDateTypes.Count; i++)
        {
            sb.AppendLine(
                $"(@CalendarId{i}, @AppDateTypeId{i}, @DateCalendar{i})" +
                (i < calendarAppDateAppDateTypes.Count - 1 ? "," : string.Empty)
            );

            parameters.Add($"CalendarId{i}", calendarAppDateAppDateTypes[i].CalendarId);
            parameters.Add($"AppDateTypeId{i}", calendarAppDateAppDateTypes[i].AppDateTypeId);
            parameters.Add($"DateCalendar{i}", calendarAppDateAppDateTypes[i].DateCalendar);
        }

        sb.AppendLine(");");

        await queryFactory.Connection.ExecuteAsync(sb.ToString(), parameters);
    }

    private static string EscapeColumn(
           string column
       )
    {
        return "\"" + column.Replace("\"", "\"\"") + "\"";
    }
}
