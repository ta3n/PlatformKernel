using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Queries.Image;

public record ImageGetQuery(
    string Code,
    string? SizeType
) : IQuerySingleBase<ImagePreviewResponse>;
