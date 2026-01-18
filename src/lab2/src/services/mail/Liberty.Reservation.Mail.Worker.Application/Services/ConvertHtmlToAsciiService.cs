using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Liberty.Reservation.Mail.Worker.Application.Services;

public class ConvertHtmlToAsciiService : IConvertHtmlToAsciiService
{
    public string ConvertAndReplaceTables(
        string input
    )
    {
        const string pattern = "<table.*?>.*?</table>";
        var match = Regex.Match(
            input,
            pattern,
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.NonBacktracking
        );
        if (match.Success)
        {
            return Regex.Replace(
                input,
                pattern,
                data =>
                {
                    var htmlTable = data.Value;
                    var asciiTable = ConvertHtmlTableToAscii(htmlTable);
                    return asciiTable;
                },
                RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.NonBacktracking
            );
        }

        return input;
    }

    private static string ConvertHtmlTableToAscii(
        string html
    )
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var rows = new List<List<string>>();

        foreach (var row in doc.DocumentNode.SelectNodes("//tr")!)
        {
            var cells = row.Elements("th")
                .Concat(row.Elements("td"))
                .Select(cell => HtmlEntity.DeEntitize(cell.InnerText.Trim()))
                .ToList();
            rows.Add(cells!);
        }

        var columnCount = rows.Max(r => r.Count);
        var columnWidths = new int[columnCount];

        foreach (var row in rows)
        {
            for (var i = 0; i < row.Count; i++)
            {
                var width = GetDisplayWidth(row[i]);
                columnWidths[i] = Math.Max(columnWidths[i], width);
            }
        }

        var maxWidth = columnWidths.Max();
        for (var i = 0; i < columnWidths.Length; i++)
        {
            columnWidths[i] = maxWidth;
        }

        var sb = new StringBuilder();
        var separator = "+" + string.Join("+", columnWidths.Select(w => new string('-', w + 2))) + "+";

        sb.AppendLine(separator);

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            sb.Append('|');

            for (var j = 0; j < columnCount; j++)
            {
                var cell = j < row.Count ? row[j] : "";
                var padding = columnWidths[j] - GetDisplayWidth(cell);
                sb.Append(" " + cell + new string(' ', padding) + " |");
            }

            sb.AppendLine();

            if (i == 0)
            {
                sb.AppendLine(separator);
            }
        }

        sb.AppendLine(separator);
        return sb.ToString();
    }

    private static int GetDisplayWidth(
        string s
    )
    {
        var width = 0;
        foreach (var c in s)
        {
            width += IsJapaneseWideChar(c) ? 2 : 1;
        }

        return width;
    }

    private static bool IsJapaneseWideChar(
        char c
    )
    {
        return
            c is >= '\u3000' and <= '\u30FF'
                or // punctuation + Katakana
                >= '\u4E00' and <= '\u9FFF'
                or // Kanji
                >= '\uFF01' and <= '\uFF60'; // Full-width ASCII
    }
}
