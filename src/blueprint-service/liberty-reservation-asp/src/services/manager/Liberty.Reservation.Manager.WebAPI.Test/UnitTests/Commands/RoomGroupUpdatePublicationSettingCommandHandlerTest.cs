using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.UnitOfWork.Abstractions;
using Moq;
using Microsoft.Extensions.Logging;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Application.Constants;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class RoomGroupUpdatePublicationSettingCommandHandlerTest
{
    private readonly IMapper _mapper;

    public RoomGroupUpdatePublicationSettingCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<RoomGroupUpdatePublicationSettingRequest, Plan>()
                    .ForMember(
                        dest => dest.UseDisplayDate,
                        opt => opt.MapFrom(
                            src => src.UseDisplayDate
                        )
                    )
                    .ForMember(
                        dest => dest.DisplayDateStart,
                        opt => opt.MapFrom(
                            src => src.DisplayDateStart
                        )
                    )
                    .ForMember(
                        dest => dest.DisplayDateEnd,
                        opt => opt.MapFrom(
                            src => src.DisplayDateEnd
                        )
                    )
                    .ForMember(
                        dest => dest.UseAcceptDate,
                        opt => opt.MapFrom(
                            src => src.UseAcceptDate
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptDateStart,
                        opt => opt.MapFrom(
                            src => src.AcceptDateStart
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptDateEnd,
                        opt => opt.MapFrom(
                            src => src.AcceptDateEnd
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptDays,
                        opt => opt.MapFrom(
                            src => src.AcceptDays
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptMonths,
                        opt => opt.MapFrom(
                            src => src.AcceptMonths
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptEndLimitType,
                        opt => opt.MapFrom(
                            src => src.AcceptEndLimitType
                        )
                    )
                    .ForMember(
                        dest => dest.ReceptionDayLimit,
                        opt => opt.MapFrom(
                            src => src.ReceptionDayLimit
                        )
                    )
                    .ForMember(
                        dest => dest.ReceptionLimit,
                        opt => opt.MapFrom(
                            src => src.ReceptionLimit
                        )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupId_WhenCommandValid()
    {
        var roomGroupId = 1;
        var planId = 10;
        var siteIds = new long[] { 100, 200 };

        // Mocks
        var mockLogger = new Mock<ILogger<RoomGroupUpdateDisplaySettingCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockSiteService = new Mock<ISiteService>();
        var mockRoomGroupService = new Mock<IRoomGroupService>();
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanSiteService = new Mock<IPlanSiteService>();
        mockMapper.SetupGet(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
                    _ =>
                    {
                    }
                )
            );
        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(1);
        mockSiteService.Setup(x => x.CountByIdsAsync(siteIds, It.IsAny<CancellationToken>())).ReturnsAsync(siteIds.Length);
        mockRoomGroupService.Setup(x => x.FindByIdAsync(roomGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new RoomGroup
                {
                    Id = roomGroupId,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room group" } }
                }
            );
        mockPlanService.Setup(x => x.GetPlanWithRoomOnlyTypeAsync(roomGroupId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Plan { Id = planId });
        mockRoomGroupService.Setup(x => x.ChangeSitesOfRoomGroupAsync(roomGroupId, siteIds, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<RoomGroupSite>().AsEnumerable(), new List<RoomGroupSite>().AsEnumerable()));
        mockPlanSiteService.Setup(x => x.ChangeSitesOfPlanAsync(planId, siteIds.ToList(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PlanSite>().AsEnumerable(), new List<PlanSite>().AsEnumerable()));

        mockPlanService.Setup(
                x => x.UpdateAsync(It.IsAny<Plan>(), false, It.IsAny<Func<Plan, Plan, Plan>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Plan { Id = planId });

        var command = new RoomGroupUpdatePublicationSettingCommand
        {
            Payload = new RoomGroupUpdatePublicationSettingRequest(
                [100, 200],
                true,
                1714060800,
                1716749200,
                true,
                1714060800,
                1716749200,
                30,
                1,
                PlanAcceptEndLimitTypes.AfterMonths,
                5,
                TimeSpan.FromHours(3)
            ) { Id = 1 }
        };

        var handler = new RoomGroupUpdatePublicationSettingCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockSiteService.Object,
            mockRoomGroupService.Object,
            mockPlanService.Object,
            mockPlanSiteService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(roomGroupId, result);
    }
}
