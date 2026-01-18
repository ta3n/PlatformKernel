using System.Collections.Frozen;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.BookingReservation;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Models;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;

public class BookingGetAllQueryHandler(
    IMapper mapper,
    IMediator mediator,
    ICacheService cacheService,
    IFacilityService facilityService,
    IFacilitySiteService facilitySiteService,
    IOptions<ServiceSetting> serviceSettingOption
) : QuerySingleBaseHandler<BookingGetAllQuery, SyndicationFeed>(mapper)
{
    private static async Task<(IHeaderDictionary, SyndicationFeed)?> TryValidateRequestAsync(
        BookingGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var validator = new BookingGetAllQueryValidator();
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken
        );

        if (validationResult.IsValid)
        {
            return null;
        }

        var errorMsg = string.Join(
            Environment.NewLine,
            validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
        );
        var headersError = new HeaderDictionary { { "ErrorCode", StatusCodes.Status400BadRequest.ToString() } };
        return (
            headersError,
            BuildErrorFeed(StatusCodes.Status400BadRequest.ToString(), errorMsg)
        );
    }

    private static SyndicationFeed BuildErrorFeed(
        string code,
        string message
    )
    {
        var feedError = new SyndicationFeed();
        var errorData = new XElement(
            "error",
            new XElement("code", code),
            new XElement("message", message)
        );
        feedError.ElementExtensions.Add(errorData);
        return feedError;
    }

    protected override async Task<(IHeaderDictionary, SyndicationFeed)> HandleAsync(
        BookingGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var validateResult = await TryValidateRequestAsync(
            request,
            cancellationToken
        );
        if (validateResult != null)
        {
            return validateResult.Value;
        }

        var facilityCodes = request.Request.FacilityIds;

        var facilities = await facilityService.GetFacilityInfoBaseAsync(
            facilityCodes,
            cancellationToken
        );
        var facilitiesList = facilities.ToList();
        var facilityIds = facilitiesList.Select(x => x.Id).ToArray();
        if (facilitiesList.Count == 0)
        {
            var headersError = new HeaderDictionary { { "ErrorCode", StatusCodes.Status400BadRequest.ToString() } };
            return (headersError, BuildErrorFeed(StatusCodes.Status400BadRequest.ToString(), "No facilities found"));
        }

        var facilityDictionary = facilitiesList.ToFrozenDictionary(
            x => x.Id
        );

        var facilitySites = await facilitySiteService.GetAllFacilitySitesByFacilityIdsAsync(
            facilityIds,
            cancellationToken
        );
        var facilitySitesList = facilitySites.ToList();

        var adultByFacilityIdsAsync = await facilityService.GetAllAdultByFacilityIdsAsync(
            facilityIds,
            cancellationToken
        );
        var personAgeTypesDictionary = adultByFacilityIdsAsync.ToFrozenDictionary(
            x => x.FacilityId
        );

        var headers = new HeaderDictionary();
        var items = new List<SyndicationItem>();

        foreach (var (facilityId, siteId) in facilitySitesList)
        {
            var facilityDto = facilityDictionary[facilityId];
            var personAgeType = personAgeTypesDictionary.GetValueOrDefault(facilityId);

            var searchByPlanQuery = CreateBookingSearchByPlanQuery(
                facilityId,
                siteId,
                request,
                personAgeType!
            );

            var item = await GetBookingItemFromCacheOrQueryAsync(
                facilityId,
                siteId,
                searchByPlanQuery,
                facilityDto,
                cancellationToken
            );

            items.Add(item);
        }

        var feed = new SyndicationFeed
        {
            Items = items,
            AttributeExtensions = { [new XmlQualifiedName("dc", "http://www.w3.org/2000/xmlns/")] = "http://purl.org/dc/elements/1.1/" },
            Language = "ja"
        };

        return (headers, feed);
    }

    private async Task<SyndicationItem> GetBookingItemFromCacheOrQueryAsync(
        long facilityId,
        long siteId,
        BookingSearchByPlanQuery searchByPlanQuery,
        FacilityBaseInfoData facilityDto,
        CancellationToken cancellationToken
    )
    {
        var cacheKey = CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingSearchPrefixKey,
                facilityId,
                siteId
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingSearchByPlanQuery),
                    JsonConvert.SerializeObject(searchByPlanQuery.Payload, JsonSettings.Optimized)
                ]
            )
        );
        var cacheKeyPattern = $"{serviceSettingOption.Value.MembershipFacilityService!.CacheInstanceName}{cacheKey}*";

        var bookingSearchCache = cacheService.GetByPattern(cacheKeyPattern);
        var bookingSearchCaches = bookingSearchCache.ToList();

        if (bookingSearchCaches.Count != 0)
        {
            var value = ConvertJsonToTuple<IEnumerable<BookingSearchByPlanResponse>>(
                bookingSearchCaches[0]
            );
            return BuildBookingXmlStructure(value?.Item2!, facilityDto);
        }

        var response = await mediator.Send(
            searchByPlanQuery,
            cancellationToken
        );

        var siteServiceCacheInstanceName = serviceSettingOption.Value.MembershipFacilityService!.CacheInstanceName;
        cacheKey = $"{siteServiceCacheInstanceName}{cacheKey}.{LanguageHeaderUtil.DefaultLanguageCode}";
        await cacheService.SetIgnoreInstanceAsync(cacheKey, response, cancellationToken: cancellationToken);

        return BuildBookingXmlStructure(response.Item2, facilityDto);
    }

    private static BookingSearchByPlanQuery CreateBookingSearchByPlanQuery(
        long facilityId,
        long siteId,
        BookingGetAllQuery request,
        FacilityPersonAgeTypeData personAgeType
    )
    {
        var persons = request.Request.Person;
        var restNumber = request.Request.RestNumber;
        var romNumber = request.Request.RoomNumber;
        var (male, female) = SplitPersons(persons);

        var checkInDate = AppDate.GetDateTime(request.Request.CheckInDate);
        var checkOutDate = AppDate.GetId(
            checkInDate.AddDays(
                DefaultValues.BookingSearchCheckOutDayOffset
            )
        );
        var displayCheckOutDate = AppDate.GetId(
            checkInDate.AddDays(
                DefaultValues.BookingSearchDisplayCheckOutDayOffset
            )
        );

        var dates = Enumerable.Range(0, restNumber)
            .Select(x => AppDate.GetId(checkInDate.AddDays(x)))
            .ToList();

        var roomArray = Enumerable.Range(0, romNumber).ToList();

        var requestSearch = new BookingSearchPlanRequest
        {
            CheckInDate = request.Request.CheckInDate,
            CheckOutDate = checkOutDate,
            DisplayCheckInDate = request.Request.CheckInDate,
            DisplayCheckOutDate = displayCheckOutDate,
            RestNumber = request.Request.RestNumber,
            RoomNumber = request.Request.RoomNumber,
            GuestsPerRoom = dates.SelectMany(
                (
                        date,
                        restIndex
                    ) =>
                    roomArray.SelectMany(
                        roomGroup =>
                            personAgeType.PersonAgeType.Select(
                                x =>
                                    new PersonOfBookingSearchModel
                                    {
                                        AppDateId = date,
                                        PersonAgeTypeId = x.Id,
                                        RestIndex = restIndex,
                                        RoomGroupIndex = roomGroup,
                                        Persons = x.IsMain ? persons : 0,
                                        MalePersons = x.IsMain ? male : 0,
                                        FemalePersons = x.IsMain ? female : 0
                                    }
                            )
                    )
            ),
            OptionItems = []
        };

        var searchByPlanQuery = new BookingSearchByPlanQuery(
            requestSearch,
            facilityId,
            siteId,
            request.Pageable
        );

        return searchByPlanQuery;

        static (int male, int female) SplitPersons(
            int persons
        )
        {
            int male = 0, female = 0;
            if (persons <= 0)
            {
                return (male, female);
            }

            male = (persons / 2) + (persons % 2);
            female = persons / 2;
            return (male, female);
        }
    }

    private static SyndicationItem BuildBookingXmlStructure(
        IEnumerable<BookingSearchByPlanResponse> bookingSearchResponse,
        FacilityBaseInfoData facilityDto
    )
    {
        var bookingDataElement = new XElement(
            "bookingData",
            bookingSearchResponse.Select(
                booking =>
                    new XElement(
                        nameof(bookingSearchResponse),
                        new XElement(nameof(booking.Id).ToCamelCase(), booking.Id),
                        new XElement(nameof(booking.Name).ToCamelCase(), booking.Name),
                        new XElement(nameof(booking.Tag).ToCamelCase(), booking.Tag),
                        new XElement(nameof(booking.Summary).ToCamelCase(), booking.Summary),
                        new XElement(nameof(booking.IsOnLinePayment).ToCamelCase(), booking.IsOnLinePayment),
                        new XElement(nameof(booking.IsOnSidePayment).ToCamelCase(), booking.IsOnSidePayment),
                        new XElement(nameof(booking.PlanType).ToCamelCase(), booking.PlanType.ToString()),
                        new XElement(nameof(booking.DisplayOrder).ToCamelCase(), booking.DisplayOrder),
                        new XElement(nameof(booking.Description).ToCamelCase(), booking.Description),
                        new XElement(nameof(booking.UseDisplayDate).ToCamelCase(), booking.UseDisplayDate),
                        new XElement(nameof(booking.DisplayDateStart).ToCamelCase(), booking.DisplayDateStart),
                        new XElement(nameof(booking.DisplayDateEnd).ToCamelCase(), booking.DisplayDateEnd),
                        new XElement(nameof(booking.UseAcceptDate).ToCamelCase(), booking.UseAcceptDate),
                        new XElement(nameof(booking.AcceptDateStart).ToCamelCase(), booking.AcceptDateStart),
                        new XElement(nameof(booking.AcceptDateEnd).ToCamelCase(), booking.DisplayDateStart),
                        new XElement(nameof(booking.MinTotalPrice).ToCamelCase(), booking.MinTotalPrice),
                        new XElement(nameof(booking.BasePrice).ToCamelCase(), booking.BasePrice),
                        new XElement(
                            nameof(booking.CheckInEnd).ToCamelCase(),
                            booking.CheckInEnd?.ToString(@"hh\:mm\:ss")
                        ),
                        new XElement(
                            nameof(booking.Rooms).ToCamelCase(),
                            booking.Rooms.Select(
                                room =>
                                    new XElement(
                                        "room",
                                        new XElement(nameof(room.Id).ToCamelCase(), room.Id),
                                        new XElement(nameof(room.Name).ToCamelCase(), room.Name),
                                        new XElement(nameof(room.Tag).ToCamelCase(), room.Tag),
                                        new XElement(nameof(room.IsEnabledSmoking).ToCamelCase(), room.IsEnabledSmoking),
                                        new XElement(nameof(room.Overview).ToCamelCase(), room.Overview),
                                        new XElement(nameof(room.DisplayOrder).ToCamelCase(), room.DisplayOrder),
                                        new XElement(
                                            nameof(room.Files).ToCamelCase(),
                                            room.Files?.Select(
                                                file =>
                                                    new XElement(
                                                        "file",
                                                        new XElement(nameof(file.Code).ToCamelCase(), file.Code),
                                                        new XElement(nameof(file.ContentType).ToCamelCase(), file.ContentType),
                                                        new XElement(nameof(file.Index).ToCamelCase(), file.Index),
                                                        new XElement(nameof(file.IsEnabled).ToCamelCase(), file.IsEnabled)
                                                    )
                                            )
                                            ?? []
                                        ),
                                        new XElement(
                                            nameof(room.AppDatePrices).ToCamelCase(),
                                            room.AppDatePrices.Select(
                                                price =>
                                                    new XElement(
                                                        "price",
                                                        new XElement(nameof(price.AppDateId).ToCamelCase(), price.AppDateId),
                                                        new XElement(nameof(price.RemainNumber).ToCamelCase(), price.RemainNumber),
                                                        new XElement(
                                                            nameof(price.Status).ToCamelCase(),
                                                            new XElement(
                                                                nameof(price.Status.IsRoomAvailable).ToCamelCase(),
                                                                price.Status.IsRoomAvailable
                                                            ),
                                                            new XElement(
                                                                nameof(price.Status.IsRoomUnderRequested).ToCamelCase(),
                                                                price.Status.IsRoomUnderRequested
                                                            ),
                                                            new XElement(
                                                                nameof(price.Status.IsAcceptDate).ToCamelCase(),
                                                                price.Status.IsAcceptDate
                                                            ),
                                                            new XElement(
                                                                nameof(price.Status.IsDayBookable).ToCamelCase(),
                                                                price.Status.IsDayBookable
                                                            ),
                                                            new XElement(nameof(price.Status.IsNight).ToCamelCase(), price.Status.IsNight),
                                                            new XElement(
                                                                nameof(price.Status.IsAvailable).ToCamelCase(),
                                                                price.Status.IsAvailable
                                                            )
                                                        ),
                                                        new XElement(nameof(price.BasePrice).ToCamelCase(), price.BasePrice),
                                                        new XElement(nameof(price.Price).ToCamelCase(), price.Price),
                                                        new XElement(nameof(price.TotalSpaTax).ToCamelCase(), price.TotalSpaTax),
                                                        new XElement(nameof(price.TotalPrice).ToCamelCase(), price.TotalPrice)
                                                    )
                                            )
                                        )
                                    )
                            )
                        ),
                        new XElement(
                            nameof(booking.Meals).ToCamelCase(),
                            booking.Meals?.Select(
                                m =>
                                    new XElement(
                                        "meal",
                                        new XElement(nameof(m.Id).ToCamelCase(), m.Id),
                                        new XElement(nameof(m.MealTypeEatType).ToCamelCase(), m.MealTypeEatType.ToString()),
                                        new XElement(nameof(m.Name).ToCamelCase(), m.Name)
                                    )
                            )
                            ?? []
                        ),
                        new XElement(
                            nameof(booking.Files).ToCamelCase(),
                            booking.Files?.Select(
                                file =>
                                    new XElement(
                                        "file",
                                        new XElement(nameof(file.Code).ToCamelCase(), file.Code),
                                        new XElement(nameof(file.ContentType).ToCamelCase(), file.ContentType),
                                        new XElement(nameof(file.Index).ToCamelCase(), file.Index),
                                        new XElement(nameof(file.IsEnabled).ToCamelCase(), file.IsEnabled)
                                    )
                            )
                            ?? []
                        ),
                        new XElement(
                            nameof(booking.Categories).ToCamelCase(),
                            booking.Categories?.Select(
                                category =>
                                    new XElement(
                                        "category",
                                        new XElement(nameof(category.Id).ToCamelCase(), category.Id),
                                        new XElement(nameof(category.Name).ToCamelCase(), category.Name)
                                    )
                            )
                            ?? []
                        )
                    )
            )
        );

        var facilityData = new XElement(
            "facility",
            new XElement(nameof(facilityDto.Id).ToCamelCase(), facilityDto.Id),
            new XElement(nameof(facilityDto.Name).ToCamelCase(), facilityDto.Name!.GetValueByHeader()),
            new XElement(nameof(facilityDto.Heading1).ToCamelCase(), facilityDto.Heading1!.GetValueByHeader()),
            new XElement(nameof(facilityDto.Address1).ToCamelCase(), facilityDto.Address1!.GetValueByHeader()),
            new XElement(nameof(facilityDto.Address2).ToCamelCase(), facilityDto.Address2!.GetValueByHeader()),
            new XElement(nameof(facilityDto.Address3).ToCamelCase(), facilityDto.Address3!.GetValueByHeader()),
            new XElement(nameof(facilityDto.Code).ToCamelCase(), facilityDto.Code),
            new XElement(nameof(facilityDto.PhoneNumber).ToCamelCase(), facilityDto.PhoneNumber),
            new XElement(nameof(facilityDto.Url).ToCamelCase(), facilityDto.Url),
            bookingDataElement
        );
        var bookingXml = facilityData.ToString(SaveOptions.DisableFormatting);
        var item = new SyndicationItem
        {
            Id = facilityDto.Code,
            Title = new TextSyndicationContent(facilityDto.Name!.GetValueByHeader()),
            Summary = new TextSyndicationContent(bookingXml, TextSyndicationContentKind.Html),
            PublishDate = DateTimeOffset.UtcNow.AddHours(DefaultValues.TimeZoneOffset)
        };

        if (!string.IsNullOrEmpty(facilityDto.Url))
        {
            item.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(facilityDto.Url)));
        }

        item.ElementExtensions.Add(
            "date",
            "http://purl.org/dc/elements/1.1/",
            DateTimeOffset.UtcNow.AddHours(DefaultValues.TimeZoneOffset).ToString("r")
        );
        return item;
    }
}
