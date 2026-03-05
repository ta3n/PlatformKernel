using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Mappings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.UnitTests;

public class FacilityGetQueryHandlerTest
{
    private readonly FacilityGetAllQueryHandler _handler;

    public FacilityGetQueryHandlerTest()
    {
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var facilityRoomGroups = new List<FacilityRoomGroup>
        {
            new()
            {
                RoomGroup = new RoomGroup
                {
                    Id = 200,
                    BaseNumber = 5,
                    CapacityMax = 3,
                    Name = new()
                    {
                        { "ja", "部屋A" },
                        { "en", "Room A" }
                    }
                }
            }
        };

        var facilities = new List<Facility>
        {
            new()
            {
                Id = 1,
                Code = "123",
                FacilityRoomGroups = facilityRoomGroups
            },
            new()
            {
                Id = 2,
                Code = "1234",
                FacilityRoomGroups = facilityRoomGroups
            }
        };

        var mockQueryable = facilities.AsQueryable().BuildMock();

        var mapperMock = new Mock<IMapper>();
        var mapperConfig = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile<FacilityMappingProfile>();
            }
        );
        mapperMock
            .Setup(m => m.ConfigurationProvider)
            .Returns(mapperConfig);

        facilityRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(mockQueryable);
        _handler = new FacilityGetAllQueryHandler(
            mapperMock.Object,
            facilityRepositoryMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnData()
    {
        var request = new RssRequest { FacilityIds = ["123", "1234"] };
        var query = new FacilityGetAllQuery(request);
        var result = await _handler.Handle(query, CancellationToken.None);
        Assert.NotNull(result.Item2);
    }
}
