using Moq;
using AutoMapper;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Cache.Services;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using MockQueryable;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class DestinationGetQueryHandlerExceptionTest
{
    private readonly IMapper _mapper;

    public DestinationGetQueryHandlerExceptionTest()
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
                            src.PrefixName,
                            src.Url,
                            src.IsEnabled
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowSiteNotfoundException_WhenSiteNotFound()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockSiteRepository = new Mock<ISiteRepository>();

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

        var query = new DestinationGetQuery(3);
        mockSiteRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(sites.AsQueryable().AsEnumerable().BuildMock());

        var handler = new DestinationGetQueryHandler(_mapper, mockCacheService.Object, mockSiteRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<SiteNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
