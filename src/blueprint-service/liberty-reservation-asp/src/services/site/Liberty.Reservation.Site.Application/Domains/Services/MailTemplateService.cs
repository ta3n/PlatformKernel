using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class MailTemplateService(
    ISystemConfigRepository systemConfigRepository
) : IMailTemplateService
{
    public async Task<TemplateFormatData?> FindMailTemplateAsync(
        CancellationToken cancellationToken = default
    )
    {
        var systemConfig = await systemConfigRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .FirstOrDefaultAsync(cancellationToken);

        var templateFormatData = systemConfig?.TemplateFormatData;

        return templateFormatData;
    }
}
