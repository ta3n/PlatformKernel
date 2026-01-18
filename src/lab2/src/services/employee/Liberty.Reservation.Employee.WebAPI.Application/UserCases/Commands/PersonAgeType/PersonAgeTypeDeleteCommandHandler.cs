using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;

public class PersonAgeTypeDeleteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPersonAgeTypeService personAgeTypeService
) : DeleteCommandHandlerBase<PersonAgeTypeDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PersonAgeTypeDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var exsistingPersonAgeType = await personAgeTypeService.FindByIdAsync(
            payload.Id ?? 0,
            cancellationToken
        );

        if (exsistingPersonAgeType == null || exsistingPersonAgeType.IsMain)
        {
            throw new PersonAgeTypeNotfoundException();
        }

        var personAgeTypeId = await personAgeTypeService.DeleteAsync(
            payload.Id ?? 0,
            true,
            cancellationToken
        );

        return personAgeTypeId.Id;
    }
}
