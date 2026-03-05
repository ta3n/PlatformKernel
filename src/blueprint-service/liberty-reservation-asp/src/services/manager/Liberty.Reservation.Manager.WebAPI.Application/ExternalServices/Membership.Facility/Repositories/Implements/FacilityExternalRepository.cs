using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;
using System.Data.Common;

namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;

public class FacilityExternalRepository(
    ILogger<FacilityExternalRepository> logger,
    MembershipFacilityExternalDbContext dataContext
) : GenericRepository<Models.Facility>(dataContext),
    IFacilityExternalRepository
{
    private const string ErrorMessageLog =
        "Database MembershipFacility connection error: {Message}";

    private const string ErrorMessageResponse =
        "Database MembershipFacility connection error. Please try again later.";

    public async Task<bool> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            cancellationToken = CancellationToken.None;
            return await GetQueryableWithAsNoTracking()
                .AnyAsync(
                    x => x.Code == facilityCode && x.State == Models.FacilityStates.Public,
                    cancellationToken
                );
        }
        catch (DbException ex)
        {
            logger.LogError(ex, ErrorMessageLog, ex.Message);
            throw new AppLibertyException(ErrorMessageResponse, ex);
        }
    }

    public async Task<FacilityInfoDto?> GetFacilityByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var queryable = GetQueryableWithAsNoTracking()
                .GroupJoin(
                    dataContext.KeyValues.Where(kv => kv.Key == "code"),
                    facility => facility.Code,
                    keyValue => keyValue.Record,
                    (
                        facility,
                        keyValues
                    ) => new
                    {
                        facility,
                        keyValue = keyValues.FirstOrDefault()
                    }
                )
                .Select(
                    x => new FacilityInfoDto
                    {
                        Id = x.facility.Id,
                        Code = x.facility.Code,
                        RecordCode = x.keyValue != null ? x.keyValue.Value : null,
                        Name = x.facility.Name,
                        Kana = x.facility.Kana,
                        Mobile = x.facility.Address!.Mobile,
                        Postcode = x.facility.Address!.Postcode,
                        CountryCode = x.facility.Address!.CountryCode,
                        Address1 = x.facility.Address!.Address1,
                        Address2 = x.facility.Address!.Address2,
                        Address3 = x.facility.Address!.Address3,
                        Address4 = x.facility.Address!.Address4,
                        Email = x.facility.Email,
                        Tel = x.facility.Address!.Tel
                    }
                );

            return await queryable.SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );
        }
        catch (DbException ex)
        {
            logger.LogError(ex, ErrorMessageLog, ex.Message);
            throw new AppLibertyException(ErrorMessageResponse, ex);
        }
    }
}
