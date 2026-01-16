namespace PlatformKernel.Service.WebApi.Application.Settings;

public class IdentitySetting
{
    public IdentityJwtConfig? Jwt { get; set; }
    public IdentityAccessCodeConfig? AccessCode { get; set; }
}

public class IdentityJwtConfig
{
    public string? Authority { get; set; }
}

public class IdentityAccessCodeConfig
{
    public string? Code { get; set; }
}
