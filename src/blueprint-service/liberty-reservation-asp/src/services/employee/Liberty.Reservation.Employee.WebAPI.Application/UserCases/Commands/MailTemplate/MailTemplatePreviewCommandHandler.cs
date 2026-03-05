using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Templates;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Options;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;

public class MailTemplatePreviewCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<SmtpMailSetting> smtpMailSettingOption
) : CommandBaseHandler<MailTemplatePreviewCommand, MailTemplatePreviewResponse>(unitOfWork, mapper)
{
    protected override Task<MailTemplatePreviewResponse> HandleAsync(
        MailTemplatePreviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var templateFormatData = new TemplateFormatData(smtpMailSettingOption.Value);
        templateFormatData.SetFormat(payload.IoType!, payload.Format);
        templateFormatData.SetUrl(smtpMailSettingOption.Value.UrlMock ?? string.Empty);
        var mailTemplate = templateFormatData.GetTemplate(
                payload.IoType!
            )
            ?? throw new MailTemplateNotfoundException(payload.IoType ?? string.Empty);

        mailTemplate.SetSample();
        string subject;
        try
        {
            subject = mailTemplate.Subject;
        }
        catch (Exception)
        {
            throw new MailTemplateSubjectFormatInvalidException();
        }

        string body;
        try
        {
            body = mailTemplate.Body;
        }
        catch (Exception)
        {
            throw new MailTemplateBodyFormatInvalidException();
        }

        var response = new MailTemplatePreviewResponse(subject, body);

        return Task.FromResult(response);
    }
}
