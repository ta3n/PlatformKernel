using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdatePublishAcceptCommandHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ISecurityContextAccessor securityContextAccessor,
    IPlanDistributionService planService,
    ICheckFacilityService checkFacilityService,
    IFacilityService facilityService
)
    : UpdateCommandHandlerBase<PlanUpdatePublishAcceptCommand, BaseDataResponse<UpdatePublishAcceptPlanResponse>>(unitOfWork, mapper)
{
    private async Task<bool> IsValidRequest(
        string facilityCode,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey ?? string.Empty;
        var facilityCodesIsValid = await checkFacilityService.CheckUserManagedFacilityAsync(
            facilityCode,
            userCode,
            cancellationToken
        );

        return facilityCodesIsValid;
    }

    protected override async Task<BaseDataResponse<UpdatePublishAcceptPlanResponse>> HandleAsync(
        PlanUpdatePublishAcceptCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityId = await facilityService.GetFacilityIdByCodeAsync(payload.ScAgtFacilityCode, cancellationToken);
        if (facilityId <= 0)
        {
            throw new FieldExternalPlanInvalidException(
                nameof(UpdatePublishAcceptPlanRequest.ScAgtFacilityCode),
                payload.ScAgtFacilityCode!
            );
        }

        var isValidFacility = await IsValidRequest(payload.ScAgtFacilityCode!, cancellationToken);
        if (!isValidFacility)
        {
            throw new FacilityForbiddenException();
        }

        var existingPlan = await planService.FindPlanByCode(
                request.Id,
                cancellationToken
            )
            ?? throw new FieldExternalPlanInvalidException(nameof(UpdatePublishAcceptPlanRequest.ScAgtPlanCode), payload.ScAgtPlanCode!);

        Mapper.Map(payload, existingPlan);
        try
        {
            var editPlan = await planService.UpdatePlanAsync(
                existingPlan,
                request.FacilityId,
                cancellationToken: cancellationToken
            );

            var updatePublishAcceptPlanResponse = Mapper.Map<UpdatePublishAcceptPlanResponse>(editPlan);

            var response = new BaseDataResponse<UpdatePublishAcceptPlanResponse> { Data = updatePublishAcceptPlanResponse };

            return response;
        }
        catch (Exception ex)
        {
            throw new AppLibertyException(ex.Message);
        }
    }
}
