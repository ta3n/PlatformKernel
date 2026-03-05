using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Allergen;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class AllergenGetQueryAllHandlerTest
{
    private readonly IMapper _mapper;

    public AllergenGetQueryAllHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Allergen, AllergenResponse>();
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllergenResponses_WhenDataExists()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockAllergenRepository = new Mock<IAllergenRepository>();

        var allergens = new List<Allergen>
        {
            new()
            {
                Id = 1,
                Name = "Allergen A"
            },
            new()
            {
                Id = 2,
                Name = "Allergen B"
            }
        };

        mockAllergenRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(allergens.AsQueryable().BuildMock());

        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new AllergenGetAllQuery(pageable);

        var handler = new AllergenGetQueryAllHandler(_mapper, mockCacheService.Object, mockAllergenRepository.Object);

        // Act
        var (_, responses) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(responses);
    }
}
