using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;
using OptionItemEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.OptionItem;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllOptionItemsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IOptionItemRepository optionItemRepository
) : QueryPageBaseHandler<ReservationGetAllOptionItemsQuery, OptionItemOfPlanResponse>(mapper)
{
    protected override async Task<(
        IHeaderDictionary,
        IEnumerable<OptionItemOfPlanResponse>
        )> HandleAsync(
        ReservationGetAllOptionItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var planId =
            await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.Id == request.Id && x.Facility!.Id == facilityId)
                .Select(x => x.PlanId)
                .SingleOrDefaultAsync(cancellationToken);

        if (planId is 0)
        {
            throw new ReservationNotfoundException();
        }

        var queryable = OptionItemQueryable(request, planId, facilityId);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var response = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        var optionItemOfPlanResponse = response.ToList();

        optionItemOfPlanResponse = [.. optionItemOfPlanResponse.Where(x => x.Number > 0)];

        return (headers, optionItemOfPlanResponse);
    }

    private IQueryable<OptionItemOfPlanResponse> OptionItemQueryable(
        ReservationGetAllOptionItemsQuery request,
        long planId,
        long facilityId
    )
    {
        return optionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x =>
                    x.IsEnabled
                    && x.PlanOptionItems!.Any(
                        t => t.PlanId == planId && t.Plan!.UseFixedOptionItem
                    )
                    && x.FacilityOptionItems!.Any(t => t.FacilityId == facilityId)
                    && x.OptionItemAppDates!.Any(t => t.AppDateId == request.AppDateId && t.SellNumber > 0)
            )
            .Select(
                x =>
                    new OptionItemEntity
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        OptionItemAppDates = x.OptionItemAppDates!
                            .Where(x => x.AppDateId == request.AppDateId)!
                            .Select(
                                y =>
                                    new OptionItemAppDate { SellNumber = y.SellNumber }
                            )
                            .ToList(),
                        FileOptionItems = x.FileOptionItems!
                            .OrderBy(x => x.Index)
                            .Select(
                                f =>
                                    new FileOptionItem
                                    {
                                        File = new File
                                        {
                                            Code = f.File!.Code,
                                            ContentType = f.File!.ContentType
                                        }
                                    }
                            )
                            .ToList(),
                        ReservationRoomGroupAppDateOptionItems =
                            x.ReservationRoomGroupAppDateOptionItems!
                                .Where(x => x.BookingDateId == request.AppDateId)!
                                .Select(
                                    z =>
                                        new ReservationRoomGroupAppDateOptionItem
                                        {
                                            Number = z.Number,
                                            Reservation = new ReservationEntity { ReservationState = z.Reservation!.ReservationState }
                                        }
                                )
                                .ToList()
                    }
            )
            .ProjectTo<OptionItemOfPlanResponse>(Mapper.ConfigurationProvider);
    }
}
