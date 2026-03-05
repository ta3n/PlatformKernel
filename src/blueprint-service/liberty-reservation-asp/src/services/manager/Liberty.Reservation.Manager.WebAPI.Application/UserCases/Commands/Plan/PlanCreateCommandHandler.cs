using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanCreateCommandHandler(
    ILogger<PlanCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IPlanService planService,
    IFacilityPlanService facilityPlanService,
    IFacilityService facilityService
) : CreateCommandHandlerBase<PlanCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var facility = await facilityService.FindByIdAsync(
                    securityContextAccessor.FacilityKey,
                    cancellationToken
                )
                ?? throw new FacilityNotfoundException();
            var plan = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.Plan>(request.Payload);
            plan.ReceptionDayLimit = 1;
            plan.ReceptionLimit = TimeSpan.Zero;
            if (request.Payload.DayUse == true)
            {
                plan.NumberOfStayLimitMax = 1;
                plan.NumberOfStayLimitMin = 1;
            }

            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newPlan = await planService.CreateAsync(plan, false, cancellationToken);

            var facilityPlan = new FacilityPlan
            {
                FacilityId = facility.Id,
                Plan = plan
            };
            await facilityPlanService.CreateAsync(facilityPlan, false, cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            return newPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanCreateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
