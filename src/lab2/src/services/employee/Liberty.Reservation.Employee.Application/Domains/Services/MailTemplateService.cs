using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class MailTemplateService(
    IOptions<SmtpMailSetting> smtpMailSettingOption,
    ILogger<MailTemplateService> logger,
    ISystemConfigRepository systemConfigRepository
) : IMailTemplateService
{
    public async Task<SystemConfig> UpdateAsync(
        SystemConfig mailTemplateToUpdate,
        CancellationToken cancellationToken = default
    )
    {
        var existingMailTemplate = await systemConfigRepository
                .GetQueryableWithAsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.Id == mailTemplateToUpdate.Id,
                    cancellationToken
                )
            ?? throw new SystemConfigNotfoundException();

        existingMailTemplate.TemplateFormatData = mailTemplateToUpdate.TemplateFormatData;
        var mailTemplateEdit = await systemConfigRepository.UpdateAsync(
            existingMailTemplate,
            true,
            cancellationToken
        );

        logger.LogInformation(
            "{MailTemplateServiceName}.{UpdateAsyncName} - Successfully updated mail template",
            nameof(MailTemplateService),
            nameof(UpdateAsync)
        );

        return mailTemplateEdit;
    }

    public async Task<TemplateFormatData> FindOneAsync(
        CancellationToken cancellationToken = default
    )
    {
        var systemConfig = await systemConfigRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.IsEnabled)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new SystemConfigNotfoundException();

        var templateFormatData = systemConfig.TemplateFormatData ?? new(smtpMailSettingOption.Value);

        logger.LogInformation(
            "{MailTemplateServiceName}.{FindOneAsyncName} - Successfully found template format data",
            nameof(MailTemplateService),
            nameof(FindOneAsync)
        );

        return templateFormatData;
    }
}
