using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllOptionItemsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IOptionItemRepository optionItemRepository
) : QueryPageBaseHandler<ReservationGetAllOptionItemsQuery, OptionItemOfPlanResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<OptionItemOfPlanResponse>)> HandleAsync(
        ReservationGetAllOptionItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;

        var reservation = await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.Id == request.Id && x.UserCode == userCode
                )
                .Select(
                    x => new
                    {
                        PlanId = x.Plan!.Id,
                        x.CheckInDate,
                        FacilityId = x.Facility!.Id
                    }
                )
                .SingleOrDefaultAsync(
                    cancellationToken
                )
            ?? throw new ReservationNotfoundException();

        var queryable = OptionItemQueryable(request, reservation.PlanId, reservation.FacilityId);

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
                x => x.IsEnabled
                    && x.PlanOptionItems!.Any(
                        t => t.PlanId == planId
                            && t.Plan!.UseFixedOptionItem
                    )
                    && x.FacilityOptionItems!.Any(
                        t => t.FacilityId == facilityId
                    )
                    && x.OptionItemAppDates!.Any(
                        t => t.AppDateId == request.AppDateId
                            && !t.IsNotSelled
                            && t.SellNumber > 0
                    )
            )
            .Select(
                x =>
                    new OptionItem
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        MaxSupplyNumber = x.MaxSupplyNumber,
                        OptionItemAppDates = x.OptionItemAppDates!
                            .Where(a => a.AppDateId == request.AppDateId)
                            .Select(
                                y =>
                                    new OptionItemAppDate { SellNumber = y.SellNumber }
                            )
                            .ToList(),
                        FileOptionItems = x.FileOptionItems!
                            .OrderBy(a => a.Index)
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
                        OptionItemQuestions = x.OptionItemQuestions!
                            .Where(t => t.Question!.IsEnabled)
                            .Select(
                                t =>
                                    new OptionItemQuestion
                                    {
                                        QuestionId = t.QuestionId,
                                        Question = new Question
                                        {
                                            Id = t.QuestionId,
                                            Name = t.Question!.Name,
                                            Description = t.Question!.Description,
                                            FormData = t.Question!.FormData,
                                            QuestionType = t.Question!.QuestionType,
                                            IsRequired = t.Question!.IsRequired
                                        }
                                    }
                            )
                            .ToList(),
                        ReservationRoomGroupAppDateOptionItems =
                            x.ReservationRoomGroupAppDateOptionItems!
                                .Where(a => a.BookingDateId == request.AppDateId)
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
