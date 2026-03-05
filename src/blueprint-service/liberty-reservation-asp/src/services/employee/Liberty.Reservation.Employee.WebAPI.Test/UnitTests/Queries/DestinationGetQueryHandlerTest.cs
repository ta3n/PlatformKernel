using Moq;
using AutoMapper;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Cache.Services;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using MockQueryable;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class DestinationGetQueryHandlerTests
{
    private readonly IMapper _mapper;

    public DestinationGetQueryHandlerTests()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Site, DestinationDetailResponse>()
                    .ConstructUsing(
                        src => new DestinationDetailResponse(
                            src.Id,
                            src.Code,
                            src.Name!.GetValueByHeader(),
                            src.ShortName,
                            src.Url,
                            src.IsEnabled
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnDestinationDetailResponse_WhenSiteFound()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockSiteRepository = new Mock<ISiteRepository>();

        var query = new DestinationGetQuery(1);
        var sites = new List<Site>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Another Site" } }
            }
        };
        var destinationResponse = new DestinationDetailResponse(
            1,
            "TS",
            "Test Site",
            "Test",
            "https://example.com",
            true
        );

        mockSiteRepository
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(sites.AsQueryable().BuildMock());

        var handler = new DestinationGetQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSiteRepository.Object
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(destinationResponse.Id, result.Item2.Id);
    }
}
