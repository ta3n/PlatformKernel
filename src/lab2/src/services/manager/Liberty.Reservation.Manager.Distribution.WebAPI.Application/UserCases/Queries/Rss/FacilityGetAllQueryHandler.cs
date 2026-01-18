using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using AutoMapper.QueryableExtensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;

public class FacilityGetAllQueryHandler(
    IMapper mapper,
    IFacilityRepository facilityRepository
) : QuerySingleBaseHandler<FacilityGetAllQuery, SyndicationFeed>(mapper)
{
    protected override async Task<(IHeaderDictionary, SyndicationFeed)> HandleAsync(
        FacilityGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var validator = new FacilityGetAllQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        var headersError = new HeaderDictionary { { "ErrorCode", StatusCodes.Status400BadRequest.ToString() } };
        if (!validationResult.IsValid)
        {
            var feedError = new SyndicationFeed();
            var errorData = new XElement(
                "error",
                new XElement("code", StatusCodes.Status400BadRequest.ToString()),
                new XElement(
                    "message",
                    string.Join(
                        Environment.NewLine,
                        validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    )
                )
            );

            feedError.ElementExtensions.Add(errorData);
            return (
                headersError,
                feedError
            );
        }

        var facilityIds = request.Request.FacilityIds ?? [];

        var facilities = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(f => facilityIds.Contains(f.Code) && f.IsEnabled)
            .ProjectTo<FacilityRoomGroupDto>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (facilities.Count == 0)
        {
            var feedError = new SyndicationFeed();
            var errorData = new XElement(
                "error",
                new XElement("code", StatusCodes.Status400BadRequest.ToString()),
                new XElement(
                    "message",
                    "No facilities found"
                )
            );

            feedError.ElementExtensions.Add(errorData);
            return (
                headersError,
                feedError
            );
        }

        var headers = new HeaderDictionary();
        var feed = SyndicationFeedOnlyFacility(facilities);
        return (headers, feed);
    }

    private static SyndicationFeed SyndicationFeedOnlyFacility(
        List<FacilityRoomGroupDto> facility
    )
    {
        var feed = new SyndicationFeed();
        var items = new List<SyndicationItem>();

        feed.AttributeExtensions[
            new XmlQualifiedName("dc", "http://www.w3.org/2000/xmlns/")
        ] = "http://purl.org/dc/elements/1.1/";
        foreach (var facilityRoomGroupDto in facility)
        {
            var facilityData = new XElement(
                "facility",
                new XElement(nameof(facilityRoomGroupDto.Id).ToCamelCase(), facilityRoomGroupDto.Id),
                new XElement(nameof(facilityRoomGroupDto.Heading).ToCamelCase(), facilityRoomGroupDto.Heading),
                new XElement(nameof(facilityRoomGroupDto.Name).ToCamelCase(), facilityRoomGroupDto.Name!),
                new XElement("address", $"{facilityRoomGroupDto.Address1} {facilityRoomGroupDto.Address2} {facilityRoomGroupDto.Address3}"),
                new XElement(nameof(facilityRoomGroupDto.Code).ToCamelCase(), facilityRoomGroupDto.Code),
                new XElement(nameof(facilityRoomGroupDto.Phone).ToCamelCase(), facilityRoomGroupDto.Phone),
                new XElement(nameof(facilityRoomGroupDto.Url).ToCamelCase(), facilityRoomGroupDto.Url),
                new XElement(
                    "rooms",
                    facilityRoomGroupDto.RoomGroup!
                        .Select(
                            x => new XElement(
                                "room",
                                new XElement(nameof(x.Code).ToCamelCase(), x.Code),
                                new XElement(nameof(x.Name).ToCamelCase(), x.Name)
                            )
                        )
                        .ToList()
                )
            );
            var facilityXml = facilityData.ToString(SaveOptions.DisableFormatting);
            var item = new SyndicationItem
            {
                Id = facilityRoomGroupDto.Code,
                Title = new TextSyndicationContent(facilityRoomGroupDto.Name!),
                Summary = new TextSyndicationContent(facilityXml, TextSyndicationContentKind.Html),
                PublishDate = DateTimeOffset.UtcNow.AddHours(DefaultValues.TimeZoneOffset)
            };
            if (!string.IsNullOrEmpty(facilityRoomGroupDto.Url))
            {
                item.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(facilityRoomGroupDto.Url!)));
            }

            item.ElementExtensions.Add(
                "date",
                "http://purl.org/dc/elements/1.1/",
                DateTimeOffset.UtcNow.AddHours(DefaultValues.TimeZoneOffset).ToString("r")
            );

            items.Add(item);
        }

        feed.Title = new TextSyndicationContent("施設一覧");
        feed.Language = "ja";
        feed.Items = items;
        return feed;
    }
}
