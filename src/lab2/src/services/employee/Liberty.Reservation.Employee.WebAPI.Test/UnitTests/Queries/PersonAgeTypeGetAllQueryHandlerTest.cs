using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.PersonAgeType;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class PersonAgeTypeGetAllQueryHandlerTest
{
    private readonly IMapper _mapper;

    public PersonAgeTypeGetAllQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PersonAgeType, PersonAgeTypeResponse>()
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
                                GroupName = src!.Meta!.GroupName,
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
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedResponse_WhenDataExists()
    {
        var mockCacheService = new Mock<ICacheService>();
        var mockPersonAgeTypeRepository = new Mock<IPersonAgeTypeRepository>();
        var cancellationToken = CancellationToken.None;

        var mockData = new List<PersonAgeType>
        {
            new()
            {
                Id = 1,
                AgeMin = 0,
                AgeMax = 10,
                DisplayOrder = 1,
                IsMaster = true,
                IsMain = true,
                IsEnabled = true,
                Name = new MultilingualText
                {
                    { "en-US", "Child" },
                    { "fr-FR", "Enfant" }
                },
                Meta = new PersonAgeTypeMeta
                {
                    GroupName = "Group1",
                    FoodBed = FoodBeds.Food | FoodBeds.Bed,
                    PersonAgeGroup = PersonAgeGroups.Child
                },
                PersonAgeTypeSpaTaxDatas =
                [
                    new()
                    {
                        SpaTaxData = new SpaTaxData
                        {
                            PriceMin = 50,
                            PriceMax = 100,
                            Tax = 10,
                            IsEnabled = true
                        }
                    }
                ]
            }
        };

        mockPersonAgeTypeRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockData.AsQueryable().BuildMock());

        var query = new PersonAgeTypeGetAllQuery(
            PageableBinderConfig.DefaultPageable
        );

        var queryHandler = new PersonAgeTypeGetAllQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockPersonAgeTypeRepository.Object
        );

        var (_, result) = await queryHandler.Handle(query, cancellationToken);

        Assert.NotEmpty(result);
    }
}
