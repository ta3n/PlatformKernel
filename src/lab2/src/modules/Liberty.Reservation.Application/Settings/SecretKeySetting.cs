namespace Liberty.Reservation.Application.Settings;

public record SecretKeySetting
{
    public string? HmacSecretKey { get; set; }
}
