namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;

public class IdentitySetting
{
    public IdentityJwtConfig? Jwt { get; set; }
    public IdentityAccessCodeConfig? AccessCode { get; set; }
}

public class IdentityJwtConfig
{
    public string? Authority { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Scope { get; set; }
}

public class IdentityAccessCodeConfig
{
    public string? Code { get; set; }
}
