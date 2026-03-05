using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupDeleteCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        var roomGroupId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupDeleteCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockFacilityRoomGroupService = new Mock<IFacilityRoomGroupService>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockRoomGroupBedTypeService = new Mock<IRoomGroupBedTypeService>();
        var mockRoomGroupCategoryService = new Mock<IRoomGroupCategoryService>();
        var mockRoomGroupSiteService = new Mock<IRoomGroupSiteService>();
        var mockFileRoomGroupService = new Mock<IFileRoomGroupService>();

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        mockFacilityRoomGroupService.Setup(
                s => s.FindAllByRoomGroupIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockRoomGroupBedTypeService.Setup(
                s => s.FindAllByRoomGroupIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockRoomGroupCategoryService.Setup(
                s => s.FindAllByRoomGroupIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockRoomGroupSiteService.Setup(
                s => s.FindAllByRoomGroupIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockFileRoomGroupService.Setup(
                s => s.FindAllByRoomGroupIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        var roomGroup = new RoomGroup
        {
            Id = roomGroupId,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room group" } },
            GroupName = "Group name",
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            CapacityMax = 10,
            CapacityMin = 1,
            BaseNumber = 1,
            Code = "Code"
        };

        mockRoomGroupService.Setup(
                s => s.DeleteAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(roomGroup);
        mockFacilityRoomGroupService.Setup(
                s => s.DeleteRangeAsync(
                    It.IsAny<IEnumerable<FacilityRoomGroup>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockRoomGroupBedTypeService.Setup(
                s => s.DeleteRangeAsync(
                    It.IsAny<IEnumerable<RoomGroupBedType>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockRoomGroupCategoryService.Setup(
                s => s.DeleteRangeAsync(
                    It.IsAny<IEnumerable<RoomGroupCategory>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockRoomGroupSiteService.Setup(
                s => s.DeleteRangeAsync(
                    It.IsAny<IEnumerable<RoomGroupSite>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        mockFileRoomGroupService.Setup(
                s => s.DeleteRangeAsync(
                    It.IsAny<IEnumerable<FileRoomGroup>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        var deleteRequest = new RoomGroupDeleteRequest(
            1
        );

        var command = new RoomGroupDeleteCommand { Payload = deleteRequest };

        var handler = new RoomGroupDeleteCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockRoomGroupService.Object,
            mockFacilityRoomGroupService.Object,
            mockRoomGroupBedTypeService.Object,
            mockRoomGroupCategoryService.Object,
            mockRoomGroupSiteService.Object,
            mockFileRoomGroupService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(roomGroupId, result);
    }
}
