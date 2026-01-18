using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;

public class MailTemplateSendCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMailService mailService
) : CommandBaseHandler<MailTemplateSendCommand, string>(unitOfWork, mapper)
{
    protected override async Task<string> HandleAsync(
        MailTemplateSendCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        await mailService.SendAsync(
            [payload.ToEmail],
            payload.Subject,
            payload.Body
        );

        return payload.ToEmail;
    }
}
