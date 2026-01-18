using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.File.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Queries.Image;

public class ImageGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityFileRepository facilityFileRepository
) : QueryPageBaseHandler<ImageGetAllQuery, ImageResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<ImageResponse>)> HandleAsync(
        ImageGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = facilityFileRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .OrderBy(x => x.Index)
            .ProjectTo<ImageResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var data = page.Content.ToList();
        foreach (var item in data)
        {
            item.FilePurposeTypes =
                [.. Enum.GetValues<FilePurposeTypes>().Where(x => x != FilePurposeTypes.None && item.FilePurposeType.HasFlag(x))];
        }

        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
