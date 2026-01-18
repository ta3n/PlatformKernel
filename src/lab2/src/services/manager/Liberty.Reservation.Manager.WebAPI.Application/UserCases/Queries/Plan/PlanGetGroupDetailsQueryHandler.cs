using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Models;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

public class PlanGetGroupDetailsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRepository planRepository
) : QuerySingleBaseHandler<PlanGetGroupDetailsQuery, object>(mapper)
{
    protected override async Task<(IHeaderDictionary, object)> HandleAsync(
        PlanGetGroupDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityPlans!.Any(y => y.FacilityId == facilityId))
            .Where(x => x.Id == request.Id);

        var queryableByGroup = QueryableByGroup(
            request.Group,
            queryable
        );

        var data = await queryableByGroup.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new PlanNotfoundException();

        return (new HeaderDictionary(), data);
    }

    private IQueryable<object> QueryableByGroup(
        GroupOfPlan group,
        IQueryable<Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Plan> queryable
    )
    {
        IQueryable<object> queryableByGroup = group switch
        {
            GroupOfPlan.BasicSetting => queryable.ProjectTo<PlanBasicSettingResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Cancel => queryable.ProjectTo<PlanCancelResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Display => queryable.ProjectTo<PlanDisplayResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.ImportantNote => queryable.ProjectTo<PlanImportantNoteResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Meal => queryable.ProjectTo<PlanMealResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Option => queryable.ProjectTo<PlanOptionResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.PaymentMethod => queryable.ProjectTo<PlanPaymentMethodResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.PublishAccept => queryable.ProjectTo<PlanPublishAcceptResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Question => queryable.ProjectTo<PlanQuestionResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.RoomType => queryable.ProjectTo<PlanRoomTypeResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Sale => queryable.ProjectTo<PlanSaleResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfPlan.Special => queryable.ProjectTo<PlanSpecialResponse>(
                Mapper.ConfigurationProvider
            ),
            _ => throw new NotImplementedException()
        };

        return queryableByGroup;
    }
}
