using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class FacilityMapperProfile : Profile
{
    public FacilityMapperProfile()
    {
        CreateMap<FacilityUpdateAcceptRequest, Facility>()
            .ForPath(
                dest => dest.Meta!.IsAcceptChildren,
                opt => opt.MapFrom(
                    src => src.IsAcceptChildren
                )
            )
            .ForPath(
                dest => dest.Meta!.AcceptChildrenInfoComment,
                opt => opt.MapFrom(
                    src => src.AcceptChildrenInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.IsAcceptPet,
                opt => opt.MapFrom(
                    src => src.IsAcceptPet
                )
            )
            .ForPath(
                dest => dest.Meta!.AcceptPetInfoComment,
                opt => opt.MapFrom(
                    src => src.AcceptPetInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.IsBarrierFree,
                opt => opt.MapFrom(
                    src => src.IsBarrierFree
                )
            )
            .ForPath(
                dest => dest.BarrierFreeInfoComment,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.BarrierFreeInfoComment.HtmlSanitize() ?? string.Empty }
                    }
                )
            );

        CreateMap<Facility, FacilityDetailAcceptResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.IsAcceptChildren,
                opt => opt.MapFrom(
                    src => src.Meta!.IsAcceptChildren
                )
            )
            .ForMember(
                dest => dest.AcceptChildrenInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.AcceptChildrenInfoComment
                )
            )
            .ForMember(
                dest => dest.IsAcceptPet,
                opt => opt.MapFrom(
                    src => src.Meta!.IsAcceptPet
                )
            )
            .ForMember(
                dest => dest.AcceptPetInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.AcceptPetInfoComment
                )
            )
            .ForMember(
                dest => dest.IsBarrierFree,
                opt => opt.MapFrom(
                    src => src.Meta!.IsBarrierFree
                )
            )
            .ForMember(
                dest => dest.BarrierFreeInfoComment,
                opt => opt.MapFrom(
                    src => src.BarrierFreeInfoComment!.GetValueByHeader()
                )
            );

        CreateMap<FacilityUpdateAccessRequest, Facility>()
            .ForMember(
                dest => dest.Latitude,
                opt => opt.MapFrom(
                    src => src.Latitude
                )
            )
            .ForMember(
                dest => dest.Longitude,
                opt => opt.MapFrom(
                    src => src.Longitude
                )
            )
            .ForPath(
                dest => dest!.AccessInfoComment,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.AccessInfoComment.HtmlSanitize() ?? string.Empty }
                    }
                )
            )
            .ForPath(
                dest => dest.Meta!.ExistsParking,
                opt => opt.MapFrom(
                    src => src.ExistsParking
                )
            )
            .ForPath(
                dest => dest!.ParkingInfoComment,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.ParkingInfoComment.HtmlSanitize() ?? string.Empty }
                    }
                )
            )
            .ForPath(
                dest => dest.Meta!.CanTransfer,
                opt => opt.MapFrom(
                    src => src.CanTransfer
                )
            )
            .ForPath(
                dest => dest!.TransferComment,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.TransferComment.HtmlSanitize() ?? string.Empty }
                    }
                )
            )
            .ForPath(
                dest => dest!.NearStationInfoComment,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.NearStationInfoComment.HtmlSanitize() ?? string.Empty }
                    }
                )
            );

        CreateMap<Facility, FacilityDetailAccessResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.Latitude,
                opt => opt.MapFrom(
                    src => src.Latitude
                )
            )
            .ForMember(
                dest => dest.Longitude,
                opt => opt.MapFrom(
                    src => src.Longitude
                )
            )
            .ForMember(
                dest => dest.AccessInfoComment,
                opt => opt.MapFrom(
                    src => src!.AccessInfoComment!.GetValueByHeader()
                )
            )
            .ForMember(
                dest => dest.ExistsParking,
                opt => opt.MapFrom(
                    src => src.Meta!.ExistsParking
                )
            )
            .ForMember(
                dest => dest.ParkingInfoComment,
                opt => opt.MapFrom(
                    src => src!.ParkingInfoComment!.GetValueByHeader()
                )
            )
            .ForMember(
                dest => dest.CanTransfer,
                opt => opt.MapFrom(
                    src => src.Meta!.CanTransfer
                )
            )
            .ForMember(
                dest => dest.TransferComment,
                opt => opt.MapFrom(
                    src => src!.TransferComment!.GetValueByHeader()
                )
            )
            .ForMember(
                dest => dest.NearStationInfoComment,
                opt => opt.MapFrom(
                    src => src!.NearStationInfoComment!.GetValueByHeader()
                )
            );

        CreateMap<FacilityUpdateBasicSettingRequest, Facility>()
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => src.Description
                )
            )
            .ForMember(
                dest => dest.Fax,
                opt => opt.MapFrom(
                    src => src.Fax
                )
            )
            .ForMember(
                dest => dest.Url,
                opt => opt.MapFrom(
                    src => src.Url
                )
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Kana,
                opt => opt.MapFrom(
                    src => src.Kana
                )
            )
            .ForMember(
                dest => dest.Phone,
                opt => opt.MapFrom(
                    src => src.Phone
                )
            )
            .ForMember(
                dest => dest.Postcode,
                opt => opt.MapFrom(
                    src => src.Postcode
                )
            )
            .ForMember(
                dest => dest.Address1,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address1 ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Address2,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address2 ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Address3,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address3 ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Address4,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address4 ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.AreaId,
                opt => opt.MapFrom(
                    src => src.AreaId
                )
            )
            .ForPath(
                dest => dest.CategoryId,
                opt => opt.MapFrom(
                    src => src.FacilityTypeId
                )
            )
            .ForPath(
                dest => dest.Meta!.RoomNumberWesternStyle,
                opt => opt.MapFrom(
                    src => src.RoomNumberWesternStyle
                )
            )
            .ForPath(
                dest => dest.Meta!.RoomNumberJapaneseStyle,
                opt => opt.MapFrom(
                    src => src.RoomNumberJapaneseStyle
                )
            )
            .ForPath(
                dest => dest.Meta!.RoomNumberJapaneseWesternStyle,
                opt => opt.MapFrom(
                    src => src.RoomNumberJapaneseWesternStyle
                )
            )
            .ForPath(
                dest => dest.Meta!.RoomNumberOtherStyle,
                opt => opt.MapFrom(
                    src => src.RoomNumberOtherStyle
                )
            );

        CreateMap<Facility, FacilityDetailBasicSettingResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => src.Description
                )
            )
            .ForMember(
                dest => dest.Phone,
                opt => opt.MapFrom(
                    src => src.Phone
                )
            )
            .ForMember(
                dest => dest.Fax,
                opt => opt.MapFrom(
                    src => src.Fax
                )
            )
            .ForMember(
                dest => dest.Url,
                opt => opt.MapFrom(
                    src => src.Url
                )
            )
            .ForMember(
                dest => dest.AreaId,
                opt => opt.MapFrom(
                    src => src.AreaId
                )
            )
            .ForMember(
                dest => dest.FacilityTypeId,
                opt => opt.MapFrom(
                    src => src.CategoryId
                )
            )
            .ForMember(
                dest => dest.RoomNumberWesternStyle,
                opt => opt.MapFrom(
                    src => src.Meta!.RoomNumberWesternStyle
                )
            )
            .ForMember(
                dest => dest.RoomNumberJapaneseStyle,
                opt => opt.MapFrom(
                    src => src.Meta!.RoomNumberJapaneseStyle
                )
            )
            .ForMember(
                dest => dest.RoomNumberJapaneseWesternStyle,
                opt => opt.MapFrom(
                    src => src.Meta!.RoomNumberJapaneseWesternStyle
                )
            )
            .ForMember(
                dest => dest.RoomNumberOtherStyle,
                opt => opt.MapFrom(
                    src => src.Meta!.RoomNumberOtherStyle
                )
            );

        CreateMap<FacilityUpdateBathRequest, Facility>()
            .ForPath(
                dest => dest.Meta!.SpaType,
                opt => opt.MapFrom(
                    src => src.SpaType.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.SpaName,
                opt => opt.MapFrom(
                    src => src.SpaName.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.SpaInfoComment,
                opt => opt.MapFrom(
                    src => src.SpaInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.SpaDescription,
                opt => opt.MapFrom(
                    src => src.SpaDescription.HtmlSanitize()
                )
            );

        CreateMap<Facility, FacilityDetailBathResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.SpaType,
                opt => opt.MapFrom(
                    src => src.Meta!.SpaType
                )
            )
            .ForMember(
                dest => dest.SpaName,
                opt => opt.MapFrom(
                    src => src.Meta!.SpaName
                )
            )
            .ForMember(
                dest => dest.SpaInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.SpaInfoComment
                )
            )
            .ForMember(
                dest => dest.SpaDescription,
                opt => opt.MapFrom(
                    src => src.Meta!.SpaDescription
                )
            );

        CreateMap<FacilityUpdatePaymentMethodRequest, Facility>()
            .ForMember(
                dest => dest.IsOnSidePayment,
                opt => opt.MapFrom(
                    src => src.IsOnSidePayment
                )
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                )
            )
            .ForPath(
                dest => dest.Meta!.OnSidePaymentComment,
                opt => opt.MapFrom(
                    src => src.OnSidePaymentComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.OnLinePaymentComment,
                opt => opt.MapFrom(
                    src => src.OnLinePaymentComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.PaymentComment,
                opt => opt.MapFrom(
                    src => src.PaymentComment.HtmlSanitize()
                )
            );

        CreateMap<FacilityUpdateMinimumPriceRequest, Facility>()
            .ForMember(
                dest => dest.IsEnabledMinimumPrice,
                opt => opt.MapFrom(
                    src => src.IsEnabledMinimumPrice
                )
            )
            .ForMember(
                dest => dest.MinimumPrice,
                opt => opt.MapFrom(
                    src => src.MinimumPrice
                )
            );

        CreateMap<Facility, FacilityDetailPaymentMethodResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.IsOnSidePayment,
                opt => opt.MapFrom(
                    src => src.IsOnSidePayment
                )
            )
            .ForMember(
                dest => dest.CanOnLinePayment,
                opt => opt.MapFrom(
                    src => src.CanOnLinePayment
                )
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                )
            )
            .ForMember(
                dest => dest.OnSidePaymentComment,
                opt => opt.MapFrom(
                    src => src.Meta!.OnSidePaymentComment
                )
            )
            .ForMember(
                dest => dest.OnLinePaymentComment,
                opt => opt.MapFrom(
                    src => src.Meta!.OnLinePaymentComment
                )
            )
            .ForMember(
                dest => dest.PaymentComment,
                opt => opt.MapFrom(
                    src => src.Meta!.PaymentComment
                )
            );

        CreateMap<FacilityUpdatePublicationInformationRequest, Facility>()
            .ForPath(
                dest => dest.Heading1,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Heading1.HtmlSanitize() ?? string.Empty }
                    }
                )
            )
            .ForPath(
                dest => dest.Meta!.PRPointComment,
                opt => opt.MapFrom(
                    src => src.PrPointComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.EquipmentInfoComment,
                opt => opt.MapFrom(
                    src => src.EquipmentInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.RoomInfoComment,
                opt => opt.MapFrom(
                    src => src.RoomInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.AmenityInfoComment,
                opt => opt.MapFrom(
                    src => src.AmenityInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.LeisureInfoComment,
                opt => opt.MapFrom(
                    src => src.LeisureInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.FAQInfoComment,
                opt => opt.MapFrom(
                    src => src.FaqInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.OtherInfoComment,
                opt => opt.MapFrom(
                    src => src.OtherInfoComment.HtmlSanitize()
                )
            )
            .ForPath(
                dest => dest.Meta!.MapUrl,
                opt => opt.MapFrom(
                    src => src.MapUrl
                )
            );

        CreateMap<Facility, FacilityDetailPublishResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.Heading1,
                opt => opt.MapFrom(
                    src => src.Heading1!.GetValueByHeader()
                )
            )
            .ForMember(
                dest => dest.PrPointComment,
                opt => opt.MapFrom(
                    src => src.Meta!.PRPointComment
                )
            )
            .ForMember(
                dest => dest.EquipmentInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.EquipmentInfoComment
                )
            )
            .ForMember(
                dest => dest.RoomInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.RoomInfoComment
                )
            )
            .ForMember(
                dest => dest.AmenityInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.AmenityInfoComment
                )
            )
            .ForMember(
                dest => dest.LeisureInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.LeisureInfoComment
                )
            )
            .ForMember(
                dest => dest.FaqInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.FAQInfoComment
                )
            )
            .ForMember(
                dest => dest.OtherInfoComment,
                opt => opt.MapFrom(
                    src => src.Meta!.OtherInfoComment
                )
            )
            .ForMember(
                dest => dest.MapUrl,
                opt => opt.MapFrom(
                    src => src.Meta!.MapUrl
                )
            );

        CreateMap<Facility, FacilityDetailClassificationResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.Allergens,
                opt => opt.MapFrom(
                    src => src.FacilityAllergens!.Select(x => x.AllergenId).ToList()
                )
            )
            .ForMember(
                dest => dest.Features,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.FacilityFeature)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Equipments,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!
                        .Where(x => x.Category!.CategoryType == CategoryTypes.FacilityEquipment)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Services,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.Leisure)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Baths,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.Spa)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Sceneries,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.View)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Amenities,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.Amenity)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Meals,
                opt => opt.MapFrom(
                    src => src.FacilityCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.MealType)
                        .Select(x => x.CategoryId)
                        .ToList()
                )
            );

        CreateMap<FacilityUpdateReservationChangeRequest, Facility>()
            .ForMember(
                dest => dest.CanAddRoomOnModify,
                opt => opt.MapFrom(
                    src => src.CanAddRoomOnModify
                )
            );

        CreateMap<Facility, FacilityDetailReservationChangeResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.CanAddRoomOnModify,
                opt => opt.MapFrom(
                    src => src.CanAddRoomOnModify
                )
            );

        CreateMap<FacilityUpdateReservationSettingRequest, Facility>()
            .ForMember(
                dest => dest.UseDailyPerson,
                opt => opt.MapFrom(
                    src => src.UseDailyPerson
                )
            );

        CreateMap<Facility, FacilityDetailReservationSettingResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.UseDailyPerson,
                opt => opt.MapFrom(
                    src => src.UseDailyPerson
                )
            );

        CreateMap<Facility, FacilityMinimumPriceResponse>()
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.IsEnabledMinimumPrice,
                opt => opt.MapFrom(
                    src => src.IsEnabledMinimumPrice
                )
            )
            .ForMember(
                dest => dest.MinimumPrice,
                opt => opt.MapFrom(
                    src => src.MinimumPrice
                )
            );
    }
}
