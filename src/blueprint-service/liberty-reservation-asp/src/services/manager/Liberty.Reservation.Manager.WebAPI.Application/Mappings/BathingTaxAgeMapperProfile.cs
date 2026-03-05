namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class BathingTaxAgeMapperProfile : Profile
{
    public BathingTaxAgeMapperProfile()
    {
        CreateMap<Facility, BathingTaxAgeResponse>()
            .ConstructUsing(
                src => new BathingTaxAgeResponse(
                    src.Meta!.UseSpaTax,
                    src.SpaTaxComment!.GetValueByHeader(),
                    src.SpaTaxTable!.GetValueByHeader()
                )
            );

        CreateMap<PersonAgeType, BathingTaxAgeDetailsResponse>()
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
                dest => dest.DisplayOrder,
                opt => opt.MapFrom(
                    src => src.DisplayOrder
                )
            )
            .ForMember(
                dest => dest.Meta,
                opt => opt.MapFrom(
                    src => new MetaOfBathingTaxAgeResponse(
                        src!.Meta!.GroupName,
                        src.Meta!.FoodBed.HasFlag(FoodBeds.Bed) ? FoodBeds.Bed : FoodBeds.None,
                        src.Meta!.FoodBed.HasFlag(FoodBeds.Food) ? FoodBeds.Food : FoodBeds.None,
                        src.Meta!.PersonAgeGroup
                    )
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
                        t => new SpaOfBathingTaxAgeResponse
                        {
                            PriceMin = t.SpaTaxData!.PriceMin,
                            PriceMax = t.SpaTaxData!.PriceMax,
                            Tax = t.SpaTaxData!.Tax
                        }
                    )
                )
            );
    }
}
