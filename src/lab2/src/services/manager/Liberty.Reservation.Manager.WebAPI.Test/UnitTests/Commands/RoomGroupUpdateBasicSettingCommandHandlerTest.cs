using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Liberty.Cache.Services;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupUpdateBasicSettingCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        const int roomGroupId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupUpdateBasicConfigurationCommandHandler>>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockBedTypeService = new Mock<IBedTypeService>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockFileRoomGroupService = new Mock<IFileRoomGroupService>();
        var mockPlanService = new Mock<IPlanService>();
        var mockCacheService = new Mock<ICacheService>();

        mockServiceProvider
            .Setup(s => s.GetService(typeof(ISecurityContextAccessor)))
            .Returns(mockSecurityContextAccessor.Object);

        mockServiceProvider
            .Setup(s => s.GetService(typeof(ICacheService)))
            .Returns(mockCacheService.Object);

        mockServiceProvider
            .Setup(s => s.GetService(typeof(IPlanService)))
            .Returns(mockPlanService.Object);

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        mockBedTypeService.Setup(x => x.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);

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

        mockRoomGroupService
            .Setup(
                x =>
                    x.UpdateBasicConfigurationOfRoomGroupAsync(
                        It.IsAny<RoomGroup>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(roomGroup);

        var planForRoomOnly = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan For Room Only" } },
            Code = "Code",
            Summary = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "summary" } }
        };

        mockPlanService.Setup(
                x => x.GetPlanWithRoomOnlyTypeAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(planForRoomOnly);

        var createRequest = new RoomGroupUpdateBasicConfigurationRequest(
            "Name",
            "Group name",
            "Overview",
            "Summary",
            "Description",
            1,
            10,
            1,
            1,
            Reservation.Application.Constants.RoomGroupSizeUnitTypes.M2,
            [],
            true,
            true,
            true,
            true,
            true,
            []
        );

        var command = new RoomGroupUpdateBasicConfigurationCommand { Payload = createRequest };
        mockSecurityContextAccessor.Setup(s => s.AcceptLanguage).Returns("ja-JP");
        mockSecurityContextAccessor.Setup(s => s.GetLanguageCode()).Returns("ja");

        var handler = new RoomGroupUpdateBasicConfigurationCommandHandler(
            mockLogger.Object,
            mockServiceProvider.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockRoomGroupService.Object,
            mockBedTypeService.Object,
            mockFileRoomGroupService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(roomGroupId, result);
    }
}
