using System.Text;
using Liberty.Reservation.Mail.Worker.Application.Services;

namespace Liberty.Reservation.Mail.Worker.Test.UnitTests;

public class ConvertHtmlTableToAsciiUnitTest
{
    private readonly IConvertHtmlToAsciiService _convertHtmlToAsciiService = new ConvertHtmlToAsciiService();

    [Fact]
    public void ContentIncludingHtmlTable_ShouldReturnAsciiTable()
    {
        var pathInput = Path.Combine(AppContext.BaseDirectory, "Samples", "Inputs", "IncludeHtmlTable.txt");
        var pathResult = Path.Combine(AppContext.BaseDirectory, "Samples", "Results", "ResultIncludeAsciiTable.txt");
        var contentsInput = File.ReadAllText(pathInput, Encoding.Default);
        var contentsResult = File.ReadAllText(pathResult, Encoding.Default);
        var result = _convertHtmlToAsciiService.ConvertAndReplaceTables(contentsInput);
        Assert.NotEmpty(result);
        Assert.Equal(contentsResult, result);
    }

    [Fact]
    public void ContentNotIncludingHtmlTable_ShouldReturnInputText()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Samples", "Inputs", "NotIncludeHtmlText.txt");
        var contents = File.ReadAllText(path);
        var result = _convertHtmlToAsciiService.ConvertAndReplaceTables(contents);
        Assert.NotEmpty(result);
        Assert.Equal(contents, result);
    }
}
