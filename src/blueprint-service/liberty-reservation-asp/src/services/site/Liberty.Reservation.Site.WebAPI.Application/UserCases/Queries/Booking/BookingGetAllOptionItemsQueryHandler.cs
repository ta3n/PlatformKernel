using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingGetAllOptionItemsQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemRepository optionItemRepository,
    IFacilityExternalRepository facilityExternalRepository
) : QueryPageBaseHandler<BookingGetAllOptionItemsQuery, OptionItemOfBookingResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingGetAllOptionItemsQuery request
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingDetailPrefixKey,
                facilityId,
                siteId,
                request.PlanId,
                0
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingGetAllOptionItemsQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<OptionItemOfBookingResponse>)> HandleAsync(
        BookingGetAllOptionItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityCode = securityContextAccessor.GetFacilityCodeSelected();

        var isFacilityExisting = await facilityExternalRepository.CheckFacilityAvailableAsync(
            facilityCode,
            cancellationToken
        );

        if (!isFacilityExisting)
        {
            throw new FacilityNotfoundException();
        }

        var queryable = optionItemRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x =>
                    x.IsEnabled
                    && x.PlanOptionItems!.Any(
                        t => t.PlanId == request.PlanId && t.Plan!.UseFixedOptionItem
                    )
                    && x.OptionItemAppDates!.Any(
                        t => t.AppDateId == payload.AppDate && !t.IsNotSelled && t.SellNumber > 0
                    )
            )
            .Select(
                x => new OptionItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    OptionItemAppDates = x.OptionItemAppDates!
                        .Where(t => t.AppDateId == payload.AppDate)
                        .Select(
                            t => new OptionItemAppDate { SellNumber = t.SellNumber }
                        )
                        .ToList(),
                    FileOptionItems = x.FileOptionItems!
                        .Where(t => t.File!.IsEnabled)
                        .OrderBy(t => t.Index)
                        .Select(
                            t => new FileOptionItem
                            {
                                File = new File
                                {
                                    Code = t.File!.Code,
                                    ContentType = t.File!.ContentType
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
                                        QuestionType = t.Question!.QuestionType
                                    }
                                }
                        )
                        .ToList(),
                    UpdatedAt = x.UpdatedAt,
                    ReservationRoomGroupAppDateOptionItems = x.ReservationRoomGroupAppDateOptionItems!
                        .Where(t => t.BookingDateId == payload.AppDate)
                        .Select(
                            t =>
                                new ReservationRoomGroupAppDateOptionItem
                                {
                                    Number = t.Number,
                                    Reservation = new ReservationEntity { ReservationState = t.Reservation!.ReservationState }
                                }
                        )
                        .ToList()
                }
            )
            .ProjectTo<OptionItemOfBookingResponse>(Mapper.ConfigurationProvider)
            .AsSingleQuery();

        var page = await queryable.UsePageableAsync(
            PageableConstants.UnPaged,
            cancellationToken: cancellationToken
        );

        var response = page.Content;

        response = response.Where(x => x.Number > 0);

        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, response);
    }
}
