using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;

public record MailTemplateSendCommand(
    MailTemplateSendRequest Payload
) : CommandBase<string>;
