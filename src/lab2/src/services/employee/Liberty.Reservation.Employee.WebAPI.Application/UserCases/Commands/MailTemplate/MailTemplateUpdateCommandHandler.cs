using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;

public class MailTemplateUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISystemConfigService systemConfigService
) : CreateCommandHandlerBase<MailTemplateUpdateCommand, string>(unitOfWork, mapper)
{
    protected override async Task<string> HandleAsync(
        MailTemplateUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var mailTemplatePage = await systemConfigService.FindAllAsync(
            PageableConstants.UnPaged,
            cancellationToken
        );
        var mailTemplate = mailTemplatePage.Content.FirstOrDefault() ?? throw new SystemConfigNotfoundException();
        var templateFormatData = mailTemplate.TemplateFormatData ?? throw new SystemConfigNotfoundException();
        templateFormatData.SetFormat(
            payload.IoType ?? string.Empty,
            payload.Format
        );
        mailTemplate.TemplateFormatData = templateFormatData;

        _ = await systemConfigService.UpdateAsync(mailTemplate, cancellationToken: cancellationToken);

        return payload.IoType ?? string.Empty;
    }
}
