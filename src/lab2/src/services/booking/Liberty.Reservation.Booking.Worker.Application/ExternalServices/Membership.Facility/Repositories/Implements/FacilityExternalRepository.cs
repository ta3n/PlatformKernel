using Liberty.ApplicationShared.Domains.Repositories;
using System.Data.Common;
using Liberty.Reservation.Booking.Worker.Application.ExternalServices.Membership.Facility.Dtos;

namespace Liberty.Reservation.Booking.Worker.Application.ExternalServices.Membership.Facility.Repositories.Implements;

public class FacilityExternalRepository(
    ILogger<FacilityExternalRepository> logger,
    MembershipFacilityExternalDbContext dataContext
) : GenericRepository<Models.Facility>(dataContext), IFacilityExternalRepository
{
    private const string ErrorMessageLog =
        "Database MembershipFacility connection error: {Message}";

    private const string ErrorMessageResponse =
        "Database MembershipFacility connection error. Please try again later.";

    public async Task<bool> CheckFacilityAvailableAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryableWithAsNoTracking()
            .AnyAsync(
                x => x.Id == facilityId && x.State == Models.FacilityStates.Public,
                cancellationToken
            );
    }

    public async Task<FacilityInfoDto?> GetFacilityByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await GetQueryableWithAsNoTracking()
                .Select(
                    x =>
                        new FacilityInfoDto
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            Kana = x.Kana,
                            Mobile = x.Address!.Mobile,
                            Postcode = x.Address!.Postcode,
                            CountryCode = x.Address!.CountryCode,
                            Address1 = x.Address!.Address1,
                            Address2 = x.Address!.Address2,
                            Address3 = x.Address!.Address3,
                            Address4 = x.Address!.Address4,
                            Email = x.Email,
                            Tel = x.Address!.Tel
                        }
                )
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (DbException ex)
        {
            logger.LogError(ex, ErrorMessageLog, ex.Message);
            throw new AppLibertyException(ErrorMessageResponse, ex);
        }
    }
}
