using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityService(
    ILogger<FacilityService> logger,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityRepository facilityRepository
) : BaseService<Facility>(logger, cacheService, facilityRepository, new FacilityNotfoundException()), IFacilityService
{
    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<Facility> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return base.GetQueryable().Where(x => x.Id == facilityId);
    }

    public override Task<Facility> UpdateAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                    _,
                    updatedEntity
                ) => updatedEntity
            );

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateBasicSettingAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name ??= [];
                existingEntity.Name?.UpdateLocalized(updateEntity.Name);
                existingEntity.Address1 ??= [];
                existingEntity.Address1?.UpdateLocalized(updateEntity.Address1);
                existingEntity.Address2 ??= [];
                existingEntity.Address2?.UpdateLocalized(updateEntity.Address2);
                existingEntity.Address3 ??= [];
                existingEntity.Address3?.UpdateLocalized(updateEntity.Address3);
                existingEntity.Address4 ??= [];
                existingEntity.Address4?.UpdateLocalized(updateEntity.Address4);

                existingEntity.Description = updateEntity.Description;
                existingEntity.Fax = updateEntity.Fax;
                existingEntity.Url = updateEntity.Url;
                existingEntity.Kana = updateEntity.Kana;
                existingEntity.Phone = updateEntity.Phone;
                existingEntity.Postcode = updateEntity.Postcode;
                existingEntity.AreaId = updateEntity.AreaId;
                existingEntity.CategoryId = updateEntity.CategoryId;
                existingEntity.Meta!.RoomNumberWesternStyle = updateEntity.Meta!.RoomNumberWesternStyle;
                existingEntity.Meta.RoomNumberJapaneseStyle = updateEntity.Meta.RoomNumberJapaneseStyle;
                existingEntity.Meta.RoomNumberJapaneseWesternStyle = updateEntity.Meta.RoomNumberJapaneseWesternStyle;
                existingEntity.Meta.RoomNumberOtherStyle = updateEntity.Meta.RoomNumberOtherStyle;
                existingEntity.Meta.Logo = updateEntity.Meta.Logo;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdatePublicationInformationAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Heading1 ??= [];
                existingEntity.Heading1!.UpdateLocalized(updateEntity.Heading1);
                existingEntity.Meta!.PRPointComment = updateEntity.Meta!.PRPointComment;
                existingEntity.Meta!.EquipmentInfoComment = updateEntity.Meta!.EquipmentInfoComment;
                existingEntity.Meta!.RoomInfoComment = updateEntity.Meta!.RoomInfoComment;
                existingEntity.Meta!.AmenityInfoComment = updateEntity.Meta!.AmenityInfoComment;
                existingEntity.Meta!.LeisureInfoComment = updateEntity.Meta!.LeisureInfoComment;
                existingEntity.Meta!.FAQInfoComment = updateEntity.Meta!.FAQInfoComment;
                existingEntity.Meta!.OtherInfoComment = updateEntity.Meta!.OtherInfoComment;
                existingEntity.Meta!.MapUrl = updateEntity.Meta!.MapUrl;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateClassificationAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.FacilityAllergens = updateEntity.FacilityAllergens;
                existingEntity.FacilityCategories = updateEntity.FacilityCategories;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateAccessAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Latitude = updateEntity.Latitude;
                existingEntity.Longitude = updateEntity.Longitude;
                existingEntity.Meta!.ExistsParking = updateEntity.Meta!.ExistsParking;
                existingEntity.Meta!.CanTransfer = updateEntity.Meta!.CanTransfer;
                existingEntity.AccessInfoComment ??= [];
                existingEntity.AccessInfoComment!.UpdateLocalized(updateEntity.AccessInfoComment);
                existingEntity.ParkingInfoComment ??= [];
                existingEntity.ParkingInfoComment!.UpdateLocalized(updateEntity.ParkingInfoComment);
                existingEntity.TransferComment ??= [];
                existingEntity.TransferComment!.UpdateLocalized(updateEntity.TransferComment);
                existingEntity.NearStationInfoComment ??= [];
                existingEntity.NearStationInfoComment!.UpdateLocalized(updateEntity.NearStationInfoComment);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateBathAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Meta!.SpaType = updateEntity.Meta!.SpaType;
                existingEntity.Meta!.SpaName = updateEntity.Meta!.SpaName;
                existingEntity.Meta!.SpaInfoComment = updateEntity.Meta!.SpaInfoComment;
                existingEntity.Meta!.SpaDescription = updateEntity.Meta!.SpaDescription;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateAcceptAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Meta!.IsAcceptChildren = updateEntity.Meta!.IsAcceptChildren;
                existingEntity.Meta!.AcceptChildrenInfoComment = updateEntity.Meta!.AcceptChildrenInfoComment;
                existingEntity.Meta!.IsAcceptPet = updateEntity.Meta!.IsAcceptPet;
                existingEntity.Meta!.AcceptPetInfoComment = updateEntity.Meta!.AcceptPetInfoComment;
                existingEntity.Meta!.IsBarrierFree = updateEntity.Meta!.IsBarrierFree;
                existingEntity.BarrierFreeInfoComment ??= [];
                existingEntity.BarrierFreeInfoComment!.UpdateLocalized(updateEntity.BarrierFreeInfoComment);

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdatePaymentMethodAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.IsOnSidePayment = updateEntity.IsOnSidePayment;
                existingEntity.IsOnLinePayment = updateEntity.IsOnLinePayment;
                existingEntity.Meta!.OnSidePaymentComment = updateEntity.Meta!.OnSidePaymentComment;
                existingEntity.Meta!.OnLinePaymentComment = updateEntity.Meta!.OnLinePaymentComment;
                existingEntity.Meta!.PaymentComment = updateEntity.Meta!.PaymentComment;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateReservationSettingAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.UseDailyPerson = updateEntity.UseDailyPerson;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateReservationChangeAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.CanAddRoomOnModify = updateEntity.CanAddRoomOnModify;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public Task<Facility> UpdateSpaTaxChangeAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.SpaTaxComment ??= [];
                existingEntity.SpaTaxComment?.UpdateLocalized(updateEntity.SpaTaxComment);
                existingEntity.SpaTaxTable ??= [];
                existingEntity.SpaTaxTable?.UpdateLocalized(updateEntity.SpaTaxTable);
                existingEntity.Meta!.UseSpaTax = updateEntity.Meta!.UseSpaTax;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var longDate = ConvertUtil.ToLong(
            ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff")
        );

        await facilityRepository
            .GetQueryable()
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(b => b.UpdatedAt, longDate),
                cancellationToken
            );
    }

    public Task<Facility> UpdateMinimumPriceAsync(
        Facility entityToUpdate,
        bool autoSave = true,
        Func<Facility, Facility, Facility>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.IsEnabledMinimumPrice = updateEntity.IsEnabledMinimumPrice;
                existingEntity.MinimumPrice = updateEntity.MinimumPrice;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<long> GetFacilityIdByCodeAsync(
        string? code,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Code == code)
            .Where(x => x.IsEnabled)
            .Select(x => x.Id);

        var facilityId = await queryable.SingleOrDefaultAsync(
            cancellationToken
        );

        return facilityId;
    }

    public async Task<IEnumerable<FacilityBaseInfoData>> GetFacilityByCodesAsync(
        string[] code,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Where(x => code.Contains(x.Code))
            .Select(x => new FacilityBaseInfoData(x.Id, x.Name, x.Code, x.Phone, x.Address1, x.Address2, x.Address3, x.Url, x.Heading1));

        return await queryable.ToListAsync(cancellationToken) ?? throw new FacilityNotfoundException();
    }

    public async Task<FacilityBaseInfoData> GetFacilityByIdsAsync(
        long facilityIds,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = string.Format(CacheKeys.RssFacilityPrefixKey, facilityIds);

        var facilitiesBase = await CacheService!.GetAsync<FacilityBaseInfoData>(
            cacheKey,
            cancellationToken
        );
        if (facilitiesBase is not null)
        {
            return facilitiesBase;
        }

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Where(x => x.Id == facilityIds)
            .Select(
                x => new FacilityBaseInfoData(
                    x.Id,
                    x.Name,
                    x.Code,
                    x.Phone,
                    x.Address1,
                    x.Address2,
                    x.Address3,
                    x.Url,
                    x.Heading1
                )
            );

        var data = await queryable.SingleOrDefaultAsync(cancellationToken) ?? throw new FacilityNotfoundException();
        await CacheService!.SetAsync(
            cacheKey,
            data,
            cancellationToken
        );
        return data;
    }

    public async Task<Facility> FindByIdWithIncludeAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var result = await facilityRepository
                .GetQueryableWithAsNoTracking()
                .Include(x => x.FacilityAllergens)
                .Include(x => x.FacilityCategories)
                .SingleOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken
                )
            ?? throw new FacilityNotfoundException();

        return result;
    }

    public async Task<Facility> FindByIdWithIncludePersonAgeTypeAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await facilityRepository
                .GetQueryableWithAsNoTracking()
                .Include(x => x.FacilityPersonAgeTypes!)
                .ThenInclude(x => x.PersonAgeType)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken
                )
            ?? throw new FacilityNotfoundException();
    }

    public async Task<IEnumerable<Facility>> FindAllAvailableFacilitiesAsync(
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Select(
                x => new Facility
                {
                    Id = x.Id,
                    IsEnabled = x.IsEnabled,
                    FacilityPlans = x.FacilityPlans!.Select(
                            y => new FacilityPlan
                            {
                                Plan = new Plan
                                {
                                    Id = y.PlanId,
                                    IsEnabled = y.Plan!.IsEnabled,
                                    IsDeleted = y.Plan!.IsDeleted,
                                    IsCancelSameAccept = y.Plan!.IsCancelSameAccept,
                                    CancelDayLimit = y.Plan.CancelDayLimit,
                                    ReceptionDayLimit = y.Plan.ReceptionDayLimit
                                }
                            }
                        )
                        .ToList()
                }
            );

        var result = await queryable.ToListAsync(
            cancellationToken
        );

        return result;
    }

    public async Task<FacilityMinimumPriceData> GetMinimumPriceAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled && x.Id == facilityId)
            .Select(
                x => new FacilityMinimumPriceData(
                    x.IsEnabledMinimumPrice,
                    x.MinimumPrice
                )
            );

        var facilityMinimumPriceData = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new FacilityNotfoundException();

        return facilityMinimumPriceData;
    }

    public async Task<IPage<FacilityAvailableModel>> FindAllAvailableFacilitiesOfPrecomputeAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            // .Where(x => x.FacilitySites!.Any(t => t.Site!.IsEnabled))
            .Select(
                x => new FacilityAvailableModel(
                    x.Id,
                    x.Code,
                    x.Name,
                    x.FacilitySites!.Select(
                        t => new SiteOfFacilityAvailableModel(
                            t.Site!.Id,
                            t.Site!.Code,
                            t.Site!.Name,
                            t.Site!.ShortName
                        )
                    ),
                    x.FacilityPersonAgeTypes!
                        .Where(
                            t => t.PersonAgeType != null
                                && t.PersonAgeType.IsEnabled
                                && t.IsEnabled
                        )
                        .Select(
                            t => new PersonAgeTypeOfFacilityAvailableModel(
                                t.PersonAgeType!.Id,
                                t.PersonAgeType!.Code,
                                t.PersonAgeType!.Name,
                                t.PersonAgeType!.IsMain
                            )
                        )
                )
            );

        var availableFacilities = await queryable.UsePageableAsync(
            pageable,
            cancellationToken: cancellationToken
        );

        return availableFacilities;
    }

    public async Task<IEnumerable<FacilityPersonAgeTypeData>> GetAllAdultByFacilityIdsAsync(
        long[] facilityIds,
        CancellationToken cancellationToken = default
    )
    {
        var facilitiesPersonAgeTypeData = new List<FacilityPersonAgeTypeData>();
        foreach (var facilityId in facilityIds)
        {
            var cacheKey = string.Format(CacheKeys.RssPersonAgeTypesPrefixKey, facilityId);

            var cachedFacilityPersonAgeTypes = await CacheService!.GetAsync<FacilityPersonAgeTypeData>(
                cacheKey,
                cancellationToken
            );
            if (cachedFacilityPersonAgeTypes is not null)
            {
                facilitiesPersonAgeTypeData.Add(cachedFacilityPersonAgeTypes);
                continue;
            }

            var queryable = facilityRepository
                .GetQueryableWithAsNoTracking()
                .Include(x => x.FacilityPersonAgeTypes!)
                .ThenInclude(f => f.PersonAgeType)
                .Where(x => x.Id == facilityId)
                .Select(
                    x => new FacilityPersonAgeTypeData(
                        x.Id,
                        x.FacilityPersonAgeTypes!
                            .Where(y => y.PersonAgeType != null && y.PersonAgeType.IsEnabled)
                            .Select(
                                y =>
                                    new PersonAgeType
                                    {
                                        Id = y.PersonAgeTypeId,
                                        IsMain = y.PersonAgeType!.IsMain
                                    }
                            )
                            .OrderBy(i => i.Id)
                            .ToList()
                    )
                );

            var facilityFersonAgeType = await queryable.SingleOrDefaultAsync(cancellationToken)
                ?? throw new PersonAgeTypeNotfoundException();

            await CacheService.SetAsync(
                cacheKey,
                facilityFersonAgeType,
                cancellationToken
            );
            facilitiesPersonAgeTypeData.Add(facilityFersonAgeType);
        }

        return facilitiesPersonAgeTypeData;
    }

    public async Task<IEnumerable<FacilityBaseInfoData>> GetFacilityInfoBaseAsync(
        string[] facilityCodes,
        CancellationToken cancellationToken = default
    )
    {
        var facilityIds = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled)
            .Where(x => facilityCodes.Contains(x.Code))
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToArrayAsync(cancellationToken);

        var facilityBaseInfoData = new List<FacilityBaseInfoData>();
        foreach (var facilityId in facilityIds)
        {
            var facility = await GetFacilityByIdsAsync(
                facilityId,
                cancellationToken
            );
            facilityBaseInfoData.Add(facility);
        }

        return facilityBaseInfoData;
    }

    public async Task<bool> CheckPaymentOnSitePaymentAvailableAsync(
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId)
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnSidePayment);

        var result = await queryable.AnyAsync(cancellationToken);

        return result;
    }
}
