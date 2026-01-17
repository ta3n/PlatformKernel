namespace Blueprint.Service.Base.Application.Settings;

public record SecretKeySetting
{
    public string? HmacSecretKey { get; set; }
}
