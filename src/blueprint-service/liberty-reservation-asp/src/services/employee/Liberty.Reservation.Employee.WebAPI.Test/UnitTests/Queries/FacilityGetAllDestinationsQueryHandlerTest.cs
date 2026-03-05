using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class FacilityGetAllDestinationsQueryHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityGetAllDestinationsQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilitySite, DestinationResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(src => src.Site!.Id)
                    )
                    .ForMember(
                        dest => dest.Code,
                        opt => opt.MapFrom(src => src.Site!.Code)
                    )
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(src => src.Site!.Name)
                    )
                    .ForMember(
                        dest => dest.ShortName,
                        opt => opt.MapFrom(src => src.Site!.ShortName)
                    )
                    .ForMember(
                        dest => dest.Url,
                        opt => opt.MapFrom(src => src.Site!.Url)
                    )
                    .ForMember(
                        dest => dest.DisplayOrder,
                        opt => opt.MapFrom(src => src.Site!.DisplayOrder)
                    )
                    .ForMember(
                        dest => dest.IsEnabled,
                        opt => opt.MapFrom(src => src.Site!.IsEnabled)
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnDestinationResponses_WhenDataExists()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockFacilitySiteRepository = new Mock<IFacilitySiteRepository>();
        var facilitySites = new List<FacilitySite>
        {
            new()
            {
                FacilityId = 100,
                IsDeleted = false,
                Site = new Site
                {
                    Id = 1,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site A" } },
                    DisplayOrder = 5,
                    IsEnabled = true
                }
            },
            new()
            {
                FacilityId = 100,
                IsDeleted = false,
                Site = new Site
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site B" } },
                    DisplayOrder = 10,
                    IsEnabled = false
                }
            },
            new()
            {
                FacilityId = 200,
                IsDeleted = false,
                Site = new Site
                {
                    Id = 3,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site C" } },
                    DisplayOrder = 15,
                    IsEnabled = true
                }
            }
        };

        mockFacilitySiteRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(facilitySites.AsQueryable().BuildMock());

        var query = new FacilityGetAllDestinationsQuery(
            PageableBinderConfig.DefaultPageable
        ) { FacilityId = 100 };

        var handler = new FacilityGetAllDestinationsQueryHandler(_mapper, mockCacheService.Object, mockFacilitySiteRepository.Object);

        // Act
        var (_, responses) = await handler.Handle(query, CancellationToken.None);
        var responseList = responses.ToList();

        // Assert
        Assert.Equal(2, responseList.Count);
        Assert.Equal(10, responseList[0].DisplayOrder);
        Assert.False(responseList[0].IsEnabled);
        Assert.Equal(5, responseList[1].DisplayOrder);
        Assert.True(responseList[1].IsEnabled);
    }
}
