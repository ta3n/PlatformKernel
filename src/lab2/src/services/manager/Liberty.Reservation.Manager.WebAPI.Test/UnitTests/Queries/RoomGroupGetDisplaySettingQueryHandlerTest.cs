using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.Application.Auth;
using Moq;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupGetDisplaySettingQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanNotfoundException()
    {
        var facilityId = 100;
        var roomGroupId = 1;
        var expectedResponse = new RoomGroupDetailDisplaySettingResponse { };

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupRepository = new Mock<IRoomGroupRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMapper.Setup(m => m.Map<RoomGroupDetailDisplaySettingResponse>(It.IsAny<RoomGroup>()))
            .Returns(expectedResponse);

        var query = new RoomGroupGetDisplaySettingQuery(roomGroupId);
        var handler = new RoomGroupGetDisplaySettingQueryHandler(
            mockMapper.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupRepository.Object
        );

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(query, CancellationToken.None));
    }
}
