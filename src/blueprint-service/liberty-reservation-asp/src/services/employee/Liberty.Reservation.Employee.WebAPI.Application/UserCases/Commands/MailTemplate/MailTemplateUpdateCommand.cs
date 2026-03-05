using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;

public record MailTemplateUpdateCommand : UpdateCommandBase<MailTemplateUpdateRequest, string>;
