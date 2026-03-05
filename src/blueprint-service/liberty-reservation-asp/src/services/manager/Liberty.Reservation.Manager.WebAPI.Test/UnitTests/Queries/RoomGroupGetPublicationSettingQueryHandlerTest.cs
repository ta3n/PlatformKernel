using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.Application.Auth;
using Moq;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupGetPublicationSettingQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanNotfoundException()
    {
        var facilityId = 100;
        var roomGroupId = 1;

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupRepository = new Mock<IRoomGroupRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var query = new RoomGroupGetPublicationSettingQuery(roomGroupId);
        var handler = new RoomGroupGetPublicationSettingQueryHandler(
            mockMapper.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupRepository.Object
        );

        await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(query, CancellationToken.None));
    }
}
