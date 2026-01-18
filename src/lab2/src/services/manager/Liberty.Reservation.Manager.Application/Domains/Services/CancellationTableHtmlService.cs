using System.Text;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class CancellationTableHtmlService : ICancellationTableHtmlService
{
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["ja"] = new()
        {
            ["daysUntilCheckIn"] = "宿泊日までの日数",
            ["cancellationFee"] = "キャンセル料(％)",
            ["description"] = "説明",
            ["day"] = "日",
            ["sameDay"] = "当日",
            ["dayBefore"] = "日前"
        },
        ["en"] = new()
        {
            ["daysUntilCheckIn"] = "Days Until Check-In",
            ["cancellationFee"] = "Cancellation Fee(％)",
            ["description"] = "Description",
            ["day"] = "day",
            ["sameDay"] = "On the day",
            ["dayBefore"] = "days before"
        }
    };

    private readonly string _thStyle = "height: 35px; border: 1px solid #c3c3c3;padding: 0 4px; min-width: 100px";
    private readonly string _tdStyle = "font-weight: normal; border: 1px solid #c3c3c3; height: 35px;padding: 0 4px";

    public string GenerateHtmlTable(
        List<CancellationData> data,
        string lang
    )
    {
        var t = _translations[lang];
        var sb = new StringBuilder();

        sb.Append("<table style='min-width: 50%; border-collapse: collapse;'>");
        sb.Append("<thead style='background-color: #efefef;'>");
        sb.Append("<tr>");
        sb.Append($"<th style='{_thStyle}'>{t["daysUntilCheckIn"]}</th>");
        sb.Append($"<th style='{_thStyle}'>{t["cancellationFee"]}</th>");
        sb.Append($"<th style='{_thStyle}'>{t["description"]}</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");

        foreach (var item in data)
        {
            sb.Append("<tr>");

            string dayDisplay;
            if (item.DayStart == 0 && item.DayEnd == 0)
            {
                dayDisplay = t["sameDay"];
            }
            else if (item.DayStart == item.DayEnd)
            {
                dayDisplay = $"{item.DayStart} {t["day"]}";
            }
            else
            {
                var max = Math.Max(item.DayStart, item.DayEnd);
                var min = Math.Min(item.DayStart, item.DayEnd);
                dayDisplay = $"{max}～{min} {t["dayBefore"]}";
            }

            sb.Append($"<td style='{_tdStyle}' class='text-center'>{dayDisplay}</td>");
            sb.Append($"<td style='{_tdStyle}' class='text-center'>{item.Rate}%</td>");
            sb.Append($"<td style='{_tdStyle}'>{item.Description?.GetValueByCode(lang)}</td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }
}
