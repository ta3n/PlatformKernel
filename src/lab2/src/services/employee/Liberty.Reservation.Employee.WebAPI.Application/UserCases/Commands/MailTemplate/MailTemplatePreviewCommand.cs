using Liberty.ApplicationShared.Cqrs.BaseCommand;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;

public record MailTemplatePreviewCommand(
    MailTemplatePreviewRequest Payload
) : ICommandBase<MailTemplatePreviewResponse>;
