using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Site.File.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.File.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Site.File.WebAPI.Application.UserCases.Queries.Media;

public record ImageGetQuery(
    ImagePreviewRequest Payload
) : IQuerySingleBase<ImagePreviewResponse>;
