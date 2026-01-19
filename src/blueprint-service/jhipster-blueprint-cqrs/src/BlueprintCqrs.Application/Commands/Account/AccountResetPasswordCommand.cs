using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountResetPasswordCommand : IRequest<Unit>
{
    public string Mail { get; set; }
}
