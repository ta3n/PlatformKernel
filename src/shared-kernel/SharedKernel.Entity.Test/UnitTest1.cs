using SharedKernel.Entity;
using SharedKernel.Entity.Utils;
using SharedKernel.Entity.ValueObjects;

namespace SharedKernel.Entity.Test;

public class UnitTest1
{
    [Fact]
    public void GetValueByCode_FallsBackToFirstAvailableValue()
    {
        var multilingualText = new MultilingualText(
            new Dictionary<string, string>
            {
                ["ja"] = "こんにちは",
                ["en"] = "Hello"
            });

        var result = multilingualText.GetValueByCode("vi");

        Assert.Equal("こんにちは", result);
    }

    [Fact]
    public void UpdateLocalized_AddsOrUpdatesLanguageEntry()
    {
        var multilingualText = new MultilingualText(
            new Dictionary<string, string>
            {
                ["en"] = "Hello"
            });

        multilingualText.UpdateLocalized(
            new Dictionary<string, string>
            {
                ["vi"] = "Xin chao"
            },
            "vi");

        multilingualText.UpdateLocalized(
            new Dictionary<string, string>
            {
                ["en"] = "Hi"
            },
            "en");

        Assert.Equal("Xin chao", multilingualText["vi"]);
        Assert.Equal("Hi", multilingualText["en"]);
    }

    [Fact]
    public void BaseEntityClone_CreatesSeparateCopyWithSameValues()
    {
        var entity = new BaseEntity
        {
            IsEnabled = true,
            DisplayOrder = 7,
            RecordMemo = "memo"
        };

        var clone = entity.Clone<BaseEntity>();

        Assert.NotSame(entity, clone);
        Assert.True(clone.IsEnabled);
        Assert.Equal(7, clone.DisplayOrder);
        Assert.Equal("memo", clone.RecordMemo);
    }

    [Fact]
    public void LanguageHeaderUtil_UsesDefaultLanguageCodeWhenNoRequestContextExists()
    {
        var languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();

        Assert.Equal(LanguageHeaderUtil.DefaultLanguageCode, languageCode);
    }

    [Fact]
    public void EntityData_DefaultsCodeAndDisabledState()
    {
        var entity = new TestEntityData();

        Assert.False(entity.IsEnabled);
        Assert.False(string.IsNullOrWhiteSpace(entity.Code));
    }

    [Fact]
    public void EntityRelation_DefaultsToEnabled()
    {
        var relation = new TestEntityRelation();

        Assert.True(relation.IsEnabled);
    }

    private sealed class TestEntityData : EntityData
    {
        public string Marker => Code ?? string.Empty;
    }

    private sealed class TestEntityRelation : EntityRelation
    {
        public bool Marker => IsEnabled;
    }
}
