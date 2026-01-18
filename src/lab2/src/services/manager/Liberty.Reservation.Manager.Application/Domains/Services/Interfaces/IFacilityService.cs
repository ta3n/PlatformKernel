using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityService : IBaseService<Facility>
{
    Task<Facility> UpdateBasicSettingAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdatePublicationInformationAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateClassificationAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateAccessAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateBathAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateAcceptAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdatePaymentMethodAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateReservationSettingAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateReservationChangeAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateSpaTaxChangeAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<Facility> UpdateMinimumPriceAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    );

    Task<long> GetFacilityIdByCodeAsync(
        string? code,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<FacilityBaseInfoData>> GetFacilityByCodesAsync(
        string[] code,
        CancellationToken cancellationToken = default
    );

    Task<FacilityBaseInfoData> GetFacilityByIdsAsync(
        long facilityIds,
        CancellationToken cancellationToken = default
    );

    Task<Facility> FindByIdWithIncludeAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<Facility> FindByIdWithIncludePersonAgeTypeAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Facility>> FindAllAvailableFacilitiesAsync(
        CancellationToken cancellationToken = default
    );

    Task<FacilityMinimumPriceData> GetMinimumPriceAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<IPage<FacilityAvailableModel>> FindAllAvailableFacilitiesOfPrecomputeAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<FacilityPersonAgeTypeData>> GetAllAdultByFacilityIdsAsync(
        long[] facilityIds,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<FacilityBaseInfoData>> GetFacilityInfoBaseAsync(
        string[] facilityCodes,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckPaymentOnSitePaymentAvailableAsync(
        long facilityId,
        CancellationToken cancellationToken
    );
}
