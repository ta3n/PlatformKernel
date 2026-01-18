using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.Application.Auth;
using Moq;
using Liberty.Pagination;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using MockQueryable;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupGetAllPublishedInQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPagedSites_WhenQueryValid()
    {
        var facilityId = 100;
        var roomGroupId = 1;
        var pageable = PageableBinderConfig.DefaultPageable;
        var sites = new List<SiteOfRoomGroupDetailPublicationSettingResponse>
        {
            new(1, "Site A"),
            new(2, "Site B")
        };

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupRepository = new Mock<IRoomGroupRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var roomGroups = new List<RoomGroup>
            {
                new()
                {
                    Id = roomGroupId,
                    FacilityRoomGroups = new List<FacilityRoomGroup> { new() { FacilityId = facilityId } },
                    RoomGroupSites = new List<RoomGroupSite>
                    {
                        new()
                        {
                            Site = new Site
                            {
                                Id = 1,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site A" } }
                            }
                        },
                        new()
                        {
                            Site = new Site
                            {
                                Id = 2,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site B" } }
                            }
                        }
                    }
                }
            }.AsQueryable()
            .BuildMock();

        mockRoomGroupRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(roomGroups);

        var query = new RoomGroupGetAllPublishedInQuery(roomGroupId, pageable);
        var handler = new RoomGroupGetAllPublishedInQueryHandler(
            mockMapper.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupRepository.Object
        );

        // Act
        var (_, result) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sites.Count, result.Count());
    }
}
