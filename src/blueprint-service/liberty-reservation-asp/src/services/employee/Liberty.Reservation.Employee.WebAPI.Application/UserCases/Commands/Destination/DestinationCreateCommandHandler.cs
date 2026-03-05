using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;

public class DestinationCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISiteService siteService
) : CreateCommandHandlerBase<DestinationCreateCommand, DestinationResponse>(unitOfWork, mapper)
{
    protected override async Task<DestinationResponse> HandleAsync(
        DestinationCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var destination = Mapper.Map<Site>(payload);
        var newDestination = await siteService.CreateAsync(destination, cancellationToken: cancellationToken);
        var response = Mapper.Map<DestinationResponse>(newDestination);

        return response;
    }
}
