using System.ServiceModel.Syndication;
using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;

[IgnoreValidation]
public record FacilityGetAllQuery(
    RssRequest Request
) : IQuerySingleBase<SyndicationFeed>;
