using System.Text;
using Dapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Utils;
using SqlKata;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class OptionItemAppDateService(
    ILogger<OptionItemAppDateService> logger,
    IOptionItemAppDateRepository optionItemAppDateRepository,
    QueryFactory queryFactory,
    IUnitOfWork unitOfWork
) : BaseServiceRelation<OptionItemAppDate>(logger, optionItemAppDateRepository), IOptionItemAppDateService
{
    private readonly EntityProperty _optionItemAppDateProp = unitOfWork.GetEntityProperty<OptionItemAppDate>();

    public async Task<IEnumerable<OptionItemAppDate>> FindAllByOptionItemIdsAsync(
        IEnumerable<long> optionItemIds,
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = optionItemAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => optionItemIds.Contains(x.OptionItemId))
            .Where(x => appDateIds.Contains(x.AppDateId));

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }

    public async Task BulkUpsertOptionItemAppDateAsync(
        List<OptionItemAppDate> optionItemAppDates
    )
    {
        if (optionItemAppDates.Count == 0)
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
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.OptionItemId)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.AppDateId)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.IsNotSelled)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.SellNumber)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.RecordMemo)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.IsEnabled)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.IsVisible)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.IsDeleted)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.DisplayOrder)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.CreatedAt)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.UpdatedAt))
        };

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var data = optionItemAppDates.Select(
                x => new object?[]
                {
                    x.OptionItemId,
                    x.AppDateId,
                    x.IsNotSelled,
                    x.SellNumber,
                    x.RecordMemo,
                    true, // IsEnabled
                    true, // IsVisible
                    false, // IsDeleted
                    now, // DisplayOrder
                    now, // CreatedAt
                    now // UpdatedAt
                }
            )
            .ToList();

        var insertQuery = new Query(_optionItemAppDateProp.TableName)
            .AsInsert(columns, data);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var updateFields = new[]
        {
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.IsNotSelled)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.SellNumber)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.RecordMemo)),
            _optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.UpdatedAt))
        };

        var sb = new StringBuilder();
        sb.AppendLine(compiled.Sql);
        sb.AppendLine("ON CONFLICT (");
        sb.Append("    ").Append(EscapeColumn(_optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.OptionItemId)))).Append(", ");
        sb.Append(EscapeColumn(_optionItemAppDateProp.ColumnName(nameof(OptionItemAppDate.AppDateId))));
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
}
