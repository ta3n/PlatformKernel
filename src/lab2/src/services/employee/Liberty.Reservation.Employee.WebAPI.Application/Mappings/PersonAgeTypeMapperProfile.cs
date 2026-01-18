using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class PersonAgeTypeMapperProfile : Profile
{
    public PersonAgeTypeMapperProfile()
    {
        CreateMap<PersonAgeType, PersonAgeTypeResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.Name!.GetValueByHeader()
                )
            )
            .ForMember(
                dest => dest.AgeMax,
                opt => opt.MapFrom(
                    src => src.AgeMax
                )
            )
            .ForMember(
                dest => dest.AgeMin,
                opt => opt.MapFrom(
                    src => src.AgeMin
                )
            )
            .ForMember(
                dest => dest.IsEnabled,
                opt => opt.MapFrom(
                    src => src.IsEnabled
                )
            )
            .ForMember(
                dest => dest.IsVisible,
                opt => opt.MapFrom(
                    src => src.IsVisible
                )
            )
            .ForMember(
                dest => dest.Meta,
                opt => opt.MapFrom(
                    src => new PersonAgeTypeResponse.MetaOfPersonAgeTypeResponse
                    {
                        GroupName = src.Meta!.GroupName,
                        Bed = src.Meta!.FoodBed.HasFlag(FoodBeds.Bed) ? FoodBeds.Bed : FoodBeds.None,
                        Food = src.Meta!.FoodBed.HasFlag(FoodBeds.Food) ? FoodBeds.Food : FoodBeds.None,
                        PersonAgeGroup = (long)src.Meta!.PersonAgeGroup
                    }
                )
            )
            .ForMember(
                dest => dest.IsMain,
                opt => opt.MapFrom(
                    src => src.IsMain
                )
            )
            .ForMember(
                dest => dest.Spas,
                opt => opt.MapFrom(
                    src => src.PersonAgeTypeSpaTaxDatas!.Select(
                        t => new PersonAgeTypeResponse.SpaOfPersonAgeTypeResponse
                        {
                            Id = t.SpaTaxDataId,
                            PriceMin = t.SpaTaxData!.PriceMin,
                            PriceMax = t.SpaTaxData!.PriceMax,
                            Tax = t.SpaTaxData!.Tax
                        }
                    )
                )
            );

        CreateMap<PersonAgeTypeCreateRequest, PersonAgeType>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Meta,
                opt => opt.MapFrom(
                    src => new PersonAgeTypeMeta
                    {
                        GroupName = src.Meta!.GroupName,
                        FoodBed = (int)src.Meta!.Bed + src.Meta!.Food,
                        PersonAgeGroup = (PersonAgeGroups)src.Meta!.PersonAgeGroup
                    }
                )
            );

        CreateMap<PersonAgeTypeUpdateRequest, PersonAgeType>()
            .ForMember(
                dest => dest.IsMaster,
                opt => opt.MapFrom(
                    src => true
                )
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Meta,
                opt => opt.MapFrom(
                    src => new PersonAgeTypeMeta
                    {
                        GroupName = src.Meta!.GroupName,
                        FoodBed = (int)src.Meta!.Bed + src.Meta!.Food,
                        PersonAgeGroup = (PersonAgeGroups)src.Meta!.PersonAgeGroup
                    }
                )
            );
    }
}
