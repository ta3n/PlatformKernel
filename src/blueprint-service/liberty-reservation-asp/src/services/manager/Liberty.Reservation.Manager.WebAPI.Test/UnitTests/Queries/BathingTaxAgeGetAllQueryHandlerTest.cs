using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class BathingTaxAgeGetAllQueryHandlerTest
{
    private readonly IMapper _mapper;

    public BathingTaxAgeGetAllQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Facility, BathingTaxAgeResponse>()
                    .ConstructUsing(
                        src => new BathingTaxAgeResponse(
                            src.Meta!.UseSpaTax,
                            src.SpaTaxComment!.GetValueByHeader(),
                            src.SpaTaxTable!.GetValueByHeader()
                        )
                    );

                cfg.CreateMap<PersonAgeType, BathingTaxAgeDetailsResponse>()
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
                                src.Meta!.FoodBed.HasFlag(Reservation.Application.Constants.FoodBeds.Bed)
                                    ? Reservation.Application.Constants.FoodBeds.Bed
                                    : Reservation.Application.Constants.FoodBeds.None,
                                src.Meta!.FoodBed.HasFlag(Reservation.Application.Constants.FoodBeds.Food)
                                    ? Reservation.Application.Constants.FoodBeds.Food
                                    : Reservation.Application.Constants.FoodBeds.None,
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
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBathingTaxAgeResponse_WithMappedPersonAgeTypes()
    {
        // Arrange
        var facilityKey = 123L;
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        var facilityEntity = new Facility
        {
            Id = facilityKey,
            DisplayOrder = 10,
            Meta = new() { UseSpaTax = true },
            SpaTaxComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "New Comment" } },
            SpaTaxTable = new MultilingualText { { TestUtil.DefaultLanguageCode, "New Table" } }
        };
        var mockFacilityRepository = new Mock<IFacilityRepository>();
        mockFacilityRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(new List<Facility> { facilityEntity }.AsQueryable().BuildMock());

        var personAgeTypes = new List<PersonAgeType>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 2" } }
            }
        };
        var mockPage = PageableBinderConfig.DefaultPageable;
        var mockPersonAgeTypeService = new Mock<IPersonAgeTypeService>();
        var mockPlanService = new Mock<IPlanService>();
        mockPersonAgeTypeService
            .Setup(
                service => service.GetAllPersonAgeTypes(
                    It.IsAny<IPageable>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Page<PersonAgeType>(personAgeTypes, mockPage, 2)
            );

        var handler = new BathingTaxAgeGetAllQueryHandler(
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityRepository.Object,
            mockPersonAgeTypeService.Object,
            mockPlanService.Object
        );

        var query = new BathingTaxAgeGetAllQuery(mockPage);

        // Act
        var (_, response) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }
}
