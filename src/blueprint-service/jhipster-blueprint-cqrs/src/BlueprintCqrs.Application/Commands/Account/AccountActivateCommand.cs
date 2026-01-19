using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountActivateCommand : IRequest<Domain.Entities.User>
{
    public string Key { get; set; }
}
