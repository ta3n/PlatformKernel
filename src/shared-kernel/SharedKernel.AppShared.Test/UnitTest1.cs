using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedKernel.AppShared.Extensions;
using SharedKernel.AppShared.Utils;
using SharedKernel.AppShared.Utils.Validation;

namespace SharedKernel.AppShared.Test;

public class UnitTest1
{
    [Fact]
    public void GetEnumDescriptions_ReturnsDescriptionAttributeValue()
    {
        var description = SampleStatus.InProgress.GetEnumDescriptions();

        Assert.Equal("In progress", description);
    }

    [Fact]
    public void SplitBatch_SplitsListIntoExpectedChunks()
    {
        var data = new List<string> { "a", "b", "c", "d", "e" };

        var batches = data.SplitBatch(2).Select(batch => batch.ToArray()).ToArray();

        Assert.Equal(3, batches.Length);
        Assert.Equal(["a", "b"], batches[0]);
        Assert.Equal(["c", "d"], batches[1]);
        Assert.Equal(["e"], batches[2]);
    }

    [Fact]
    public void MatchesPattern_HandlesWildcardPatterns()
    {
        Assert.True(new PathString("/api/users").MatchesPattern("/api/*"));
        Assert.False(new PathString("/static/js/app.js").MatchesPattern("/static/*.css"));
    }

    [Fact]
    public void GetOptionsExt_BindsConfigurationSection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Sample:Name", "kernel"),
                new KeyValuePair<string, string?>("Sample:Enabled", "true")
            ])
            .Build();

        var options = configuration.GetOptionsExt<SampleOptions>("Sample");

        Assert.Equal("kernel", options.Name);
        Assert.True(options.Enabled);
    }

    [Fact]
    public void StringUtilities_TransformAndSanitizeValues()
    {
        var sanitized = "<script>alert('x')</script><b>Hello</b>".HtmlSanitize();

        Assert.Equal("Kernel", StringUtil.ToFirstUpper("kernel"));
        Assert.Equal("kernel", "Kernel".ToCamelCase());
        Assert.NotNull(sanitized);
        Assert.DoesNotContain("<script", sanitized, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<b>Hello</b>", sanitized, StringComparison.Ordinal);
    }

    [Fact]
    public void FileSizeAndValidationUtilities_ReturnExpectedResults()
    {
        TimeSpan? time = TimeSpan.FromHours(26.5);
        var fileSize = FileSizeConverter.ToHumanReadable(1024);

        Assert.Equal("1.00 KB", fileSize.Replace(',', '.'));
        Assert.True(ValidMail.BeValidEmail("dev@example.com"));
        Assert.True(ValidUrl.BeValidUrl("https://example.com"));
        Assert.Equal("26:30", time.To24HourFormat());
    }

    [Fact]
    public void EntityUtil_CreateNoticeNumberUsesPrefixAndHexSuffix()
    {
        var noticeNumber = EntityUtil.CreateNoticeNumber("PK");

        Assert.StartsWith("PK", noticeNumber, StringComparison.Ordinal);
        Assert.Matches("^PK[0-9A-F]{6}$", noticeNumber);
    }

    private enum SampleStatus
    {
        [Description("In progress")]
        InProgress
    }

    private sealed class SampleOptions
    {
        public string? Name { get; init; }

        public bool Enabled { get; init; }
    }
}
