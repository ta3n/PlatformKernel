using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.User.Application.Domains.Services;

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
