using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityUpdateFaxServiceCommandHandler(
    ILogger<FacilityUpdateFaxServiceCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IFacilityService facilityService
) : UpdateCommandHandlerBase<FacilityUpdateFaxServiceCommand, FacilityResponse>(unitOfWork, mapper)
{
    protected override async Task<FacilityResponse> HandleAsync(
        FacilityUpdateFaxServiceCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityCount = await facilityService.CountByIdsAsync([payload.Id ?? 0], cancellationToken);
        if (facilityCount is 0)
        {
            var requestCreate = new FacilityCreateRequest(
                payload.Id ?? 0
            )
            {
                CanOnLinePayment = false,
                SystemEMail = string.Empty,
                Memo = string.Empty,
                IsEnabled = false,
                Fax = payload.Fax,
                UseFax = payload.UseFax
            };
            var facilityResponse = await mediator.Send(
                new FacilityCreateCommand { Payload = requestCreate },
                cancellationToken
            );
            return facilityResponse;
        }

        var existingFacility = await facilityService.FindByIdAsync(
            payload.Id ?? 0,
            cancellationToken
        );

        try
        {
            existingFacility.Fax = payload.Fax;
            existingFacility.UseFax = payload.UseFax;
            var editFacility = await facilityService.UpdateFaxOfFacilityAsync(
                existingFacility,
                true,
                cancellationToken
            );

            var response = Mapper.Map<FacilityResponse>(editFacility);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update facility failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
