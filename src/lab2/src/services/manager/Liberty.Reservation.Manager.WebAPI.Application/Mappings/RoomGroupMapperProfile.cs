using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class RoomGroupMapperProfile : Profile
{
    public RoomGroupMapperProfile()
    {
        CreateMap<RoomGroupCreateRequest, RoomGroup>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name } }
                )
            )
            .ForMember(
                dest => dest.Overview,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Overview ?? string.Empty } }
                )
            );

        CreateMap<RoomGroupUpdateBasicConfigurationRequest, RoomGroup>()
            .ForMember(
                des => des.CapacityMin,
                opt => opt.MapFrom(
                    src => src.CapacityMin ?? 0
                )
            )
            .ForMember(
                des => des.CapacityMax,
                opt => opt.MapFrom(
                    src => src.CapacityMax ?? 0
                )
            )
            .ForMember(
                des => des.BaseNumber,
                opt => opt.MapFrom(
                    src => src.BaseNumber ?? 0
                )
            )
            .ForMember(
                des => des.Size,
                opt => opt.MapFrom(
                    src => src.Size ?? 0
                )
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name } }
                )
            )
            .ForMember(
                dest => dest.Overview,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Overview ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Description ?? string.Empty } }
                )
            );

        CreateMap<RoomGroup, RoomGroupResponse>()
            .ConstructUsing(
                src => new RoomGroupResponse(
                    src.Id,
                    src.Code ?? string.Empty,
                    src.Name!.GetValueByHeader(),
                    src.GroupName,
                    src.CapacityMin ?? 0,
                    src.CapacityMax ?? 0,
                    src.BaseNumber,
                    src.Size ?? 0,
                    src.RoomGroupSizeUnitType,
                    src.DisplayOrder,
                    src.IsEnabledSmoking,
                    src.IsEnabled,
                    src.FileRoomGroups!.OrderBy(x => x.Index)
                        .Select(
                            x => new ImageOfRoomGroupDetailMediaSettingResponse(
                                x.FileId,
                                x.File!.Code ?? string.Empty,
                                x.Index,
                                x.File!.IsEnabled,
                                x.File!.Description
                            )
                        )
                        .ToArray(),
                    src.Tag
                )
            );

        CreateMap<RoomGroup, RoomGroupDetailBasicConfigurationResponse>()
            .ConstructUsing(
                src => new RoomGroupDetailBasicConfigurationResponse(
                    src.Id,
                    src.Name!.GetValueByHeader(),
                    src.GroupName ?? string.Empty,
                    src.Description != null ? src.Description.GetValueByHeader() : null,
                    src.CapacityMin ?? 0,
                    src.CapacityMax ?? 0,
                    src.BaseNumber,
                    src.Size,
                    src.RoomGroupSizeUnitType,
                    src.RoomGroupBedTypes!.Select(
                            x => new BedTypeOfRoomGroupUpdateBasicConfigurationResponse(
                                x.BedTypeId,
                                x.BedType!.Code ?? string.Empty,
                                x.BedType!.Name ?? string.Empty,
                                x.Number ?? 0
                            )
                        )
                        .ToArray(),
                    src.IsEnabledSmoking,
                    src.IsDescriptionVisible,
                    src.IsOverviewVisible,
                    src.IsRoomSizeVisible,
                    src.IsBedTypeVisible,
                    src.FileRoomGroups!.OrderBy(x => x.Index)
                        .Select(
                            x => new FileOfRoomBasicConfigurationResponse(
                                x.FileId,
                                x.File!.Code ?? string.Empty,
                                x.Index,
                                x.File!.IsEnabled,
                                x.File!.Description
                            )
                        )
                        .ToArray()
                )
            );

        CreateMap<RoomGroup, RoomGroupDetailDisplaySettingResponse>()
            .ForMember(
                des => des.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                des => des.RoomGroupMasterCategories,
                opt => opt.MapFrom(
                    src => src.RoomGroupCategories!
                        .Where(
                            t =>
                                t.Category!.CategoryType == CategoryTypes.RoomGroup
                                && t.Category!.IsMaster
                        )
                        .Select(
                            x => new CategoryOfRoomGroupDetailDisplaySettingResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.CategoryType
                            )
                        )
                        .ToArray()
                )
            )
            .ForMember(
                des => des.RoomGroupCategories,
                opt => opt.MapFrom(
                    src => src.RoomGroupCategories!
                        .Where(
                            t =>
                                t.Category!.CategoryType == CategoryTypes.RoomGroup
                                && !t.Category!.IsMaster
                                && t.Category.FacilityCategories!.Any(
                                    y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                )
                        )
                        .Select(
                            x => new CategoryOfRoomGroupDetailDisplaySettingResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.CategoryType
                            )
                        )
                        .ToArray()
                )
            )
            .ForMember(
                des => des.RoomGroupFeatureCategories,
                opt => opt.MapFrom(
                    src => src.RoomGroupCategories!
                        .Where(
                            t =>
                                t.Category!.CategoryType == CategoryTypes.RoomGroupFeature
                        )
                        .Select(
                            x => new CategoryOfRoomGroupDetailDisplaySettingResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.CategoryType
                            )
                        )
                        .ToArray()
                )
            )
            .ForMember(
                des => des.RoomGroupEquipmentCategories,
                opt => opt.MapFrom(
                    src => src.RoomGroupCategories!
                        .Where(
                            t =>
                                t.Category!.CategoryType == CategoryTypes.RoomGroupEquipment
                                && t.Category!.IsMaster
                        )
                        .Select(
                            x => new CategoryOfRoomGroupDetailDisplaySettingResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.CategoryType
                            )
                        )
                        .ToArray()
                )
            )
            .ForMember(
                des => des.RoomGroupAmenityCategories,
                opt => opt.MapFrom(
                    src => src.RoomGroupCategories!
                        .Where(
                            t =>
                                t.Category!.CategoryType == CategoryTypes.Amenity
                                && t.Category!.IsMaster
                        )
                        .Select(
                            x => new CategoryOfRoomGroupDetailDisplaySettingResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.CategoryType
                            )
                        )
                        .ToArray()
                )
            );

        CreateMap<RoomGroup, RoomGroupDetailPublicationSettingResponse>()
            .ForMember(
                des => des.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                des => des.Sites,
                opt => opt.MapFrom(
                    src => src.RoomGroupSites!.Select(
                            x => new SiteOfRoomGroupDetailPublicationSettingResponse(
                                x.SiteId,
                                x.Site!.Name!.GetValueByHeader()
                            )
                        )
                        .ToArray()
                )
            );

        CreateMap<RoomGroupUpdatePublicationSettingRequest, Plan>()
            .ForMember(
                dest => dest.UseDisplayDate,
                opt => opt.MapFrom(
                    src => src.UseDisplayDate
                )
            )
            .ForMember(
                dest => dest.DisplayDateStart,
                opt => opt.MapFrom(
                    src => src.DisplayDateStart
                )
            )
            .ForMember(
                dest => dest.DisplayDateEnd,
                opt => opt.MapFrom(
                    src => src.DisplayDateEnd
                )
            )
            .ForMember(
                dest => dest.UseAcceptDate,
                opt => opt.MapFrom(
                    src => src.UseAcceptDate
                )
            )
            .ForMember(
                dest => dest.AcceptDateStart,
                opt => opt.MapFrom(
                    src => src.AcceptDateStart
                )
            )
            .ForMember(
                dest => dest.AcceptDateEnd,
                opt => opt.MapFrom(
                    src => src.AcceptDateEnd
                )
            )
            .ForMember(
                dest => dest.AcceptDays,
                opt => opt.MapFrom(
                    src => src.AcceptDays
                )
            )
            .ForMember(
                dest => dest.AcceptMonths,
                opt => opt.MapFrom(
                    src => src.AcceptMonths
                )
            )
            .ForMember(
                dest => dest.AcceptEndLimitType,
                opt => opt.MapFrom(
                    src => src.AcceptEndLimitType
                )
            )
            .ForMember(
                dest => dest.ReceptionDayLimit,
                opt => opt.MapFrom(
                    src => src.ReceptionDayLimit
                )
            )
            .ForMember(
                dest => dest.ReceptionLimit,
                opt => opt.MapFrom(
                    src => src.ReceptionLimit
                )
            );
    }
}
