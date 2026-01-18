using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Site.WebAPI.Application.Mappings;

public class RoomGroupMapperProfile : Profile
{
    public RoomGroupMapperProfile()
    {
        CreateMap<RoomGroup, RoomGroupResponse>()
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.Overview, opt => opt.Ignore())
            .ForMember(dest => dest.Size, opt => opt.Ignore())
            .ForMember(dest => dest.RoomGroupSizeUnitType, opt => opt.Ignore())
            .ConstructUsing(
                src =>
                    new RoomGroupResponse(
                        src.Id,
                        src.Code,
                        src.Name!.GetValueByHeader(DefaultValues.LanguageCode),
                        src.Tag,
                        src.IsDescriptionVisible
                            ? MapMultilingualText(src.Description)
                            : null,
                        src.IsOverviewVisible
                            ? MapMultilingualText(src.Overview)
                            : null,
                        src.CapacityMax ?? 1,
                        src.CapacityMin ?? 1,
                        src.IsRoomSizeVisible
                            ? src.Size
                            : null,
                        src.IsEnabledSmoking,
                        src.IsRoomSizeVisible,
                        src.IsBedTypeVisible,
                        src.IsRoomSizeVisible
                            ? src.RoomGroupSizeUnitType
                            : null,
                        src.IsBedTypeVisible
                            ? src.RoomGroupBedTypes!
                                .Where(x => x.Number > 0)
                                .OrderBy(x => x.BedType!.Id)
                                .Select(x => $"{x.BedType!.Name!} {x.Number}")
                                .ToList()
                            : new List<string>(),
                        src.RoomGroupCategories!
                            .Where(
                                x => x.Category!.CategoryType == CategoryTypes.RoomGroupEquipment
                                    && x.Category!.IsEnabled
                                    && x.Category!.IsMaster
                            )
                            .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                            .ToList(),
                        src.RoomGroupCategories!
                            .Where(
                                x => x.Category!.CategoryType == CategoryTypes.RoomGroupEquipment
                                    && x.Category!.IsEnabled
                                    && !x.Category!.IsMaster
                                    && x.Category.FacilityCategories!.Any(
                                        y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                    )
                            )
                            .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                            .ToList(),
                        src.RoomGroupCategories!
                            .Where(
                                x => x.Category!.CategoryType == CategoryTypes.Amenity
                                    && x.Category!.IsEnabled
                                    && x.Category!.IsMaster
                            )
                            .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                            .ToList(),
                        src.RoomGroupCategories!
                            .Where(
                                x => x.Category!.CategoryType == CategoryTypes.Amenity
                                    && x.Category!.IsEnabled
                                    && !x.Category!.IsMaster
                                    && x.Category.FacilityCategories!.Any(
                                        y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                    )
                            )
                            .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                            .ToList(),
                        src.FileRoomGroups!
                            .Where(x => x.File!.IsEnabled)
                            .OrderBy(x => x.Index)
                            .Select(x => new FileOfBookingResponse(x.File!.Code, x.File.ContentType)),
                        src.UpdatedAt
                    )
            );
    }

    private static string? MapMultilingualText(
        MultilingualText? text
    )
    {
        return text?.GetValueByHeader(DefaultValues.LanguageCode);
    }
}
