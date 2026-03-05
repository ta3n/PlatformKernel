using System.Data.Common;
using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;
using Liberty.SysException.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implementations;

public class FacilityExternalRepository(
    ILogger<FacilityExternalRepository> logger,
    MembershipFacilityExternalDbContext dbContext
)
    : GenericRepository<Models.Facility>(dbContext),
        IFacilityExternalRepository
{
    private const string ErrorMessageLog =
        "Database MembershipFacility connection error: {Message}";

    private const string ErrorMessageResponse =
        "Database MembershipFacility connection error. Please try again later.";

    public async Task<IPage<FacilityInfoDto>?> GetAllFacilitiesByPageAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // var queryable = GetQueryableWithAsNoTracking()
            //     .GroupJoin(
            //         dbContext.KeyValues.Where(kv => kv.Key == "code"),
            //         facility => facility.Code,
            //         keyValue => keyValue.Record,
            //         (facility, keyValues) => new { facility, keyValues }
            //     )
            //     .SelectMany(
            //         x => x.keyValues.DefaultIfEmpty(),
            //         (x, keyValue) => new FacilityInfoDto
            //         {
            //             Id = x.facility.Id,
            //             Code = x.facility.Code,
            //             Name = x.facility.Name,
            //             State = x.facility.State,
            //             RecordCode = keyValue != null ? keyValue.Value : null
            //         }
            //     );

            var queryable = GetQueryableWithAsNoTracking()
                .GroupJoin(
                    dbContext.KeyValues.Where(kv => kv.Key == "code"),
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
                        Name = x.facility.Name,
                        State = x.facility.State,
                        RecordCode = x.keyValue != null ? x.keyValue.Value : null
                    }
                );

            return await queryable.UsePageableAsync(
                pageable,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(GetAllFacilitiesByPageAsync), ex.Message);
            return new Page<FacilityInfoDto>([], pageable, 0);
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
                    dbContext.KeyValues.Where(kv => kv.Key == "code"),
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
                    x =>
                        new FacilityInfoDto
                        {
                            Id = x.facility.Id,
                            Code = x.facility.Code,
                            Name = x.facility.Name,
                            Kana = x.facility.Kana,
                            Mobile = x.facility.Address!.Mobile,
                            Postcode = x.facility.Address!.Postcode,
                            CountryCode = x.facility.Address!.CountryCode,
                            Address1 = x.facility.Address!.Address1,
                            Address2 = x.facility.Address!.Address2,
                            Address3 = x.facility.Address!.Address3,
                            Address4 = x.facility.Address!.Address4,
                            Memo = x.facility.Memo,
                            Email = x.facility.Email,
                            Tel = x.facility.Address!.Tel,
                            State = x.facility.State,
                            RecordCode = x.keyValue != null ? x.keyValue.Value : null
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
