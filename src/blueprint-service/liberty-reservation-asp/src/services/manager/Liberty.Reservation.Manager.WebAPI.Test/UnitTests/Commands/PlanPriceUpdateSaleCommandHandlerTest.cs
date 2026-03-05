using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanPriceUpdateSaleCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanRoomGroupSiteService = new Mock<IPlanRoomGroupSiteService>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<RoomGroupPriceUpdateSaleRequest, PlanRoomGroupSite>()
                    .ForMember(
                        dest => dest.AutoExtendEveryMonthDay,
                        opt => opt.MapFrom(
                            src => src.AutoExtendEveryMonthDay
                        )
                    )
                    .ForMember(
                        dest => dest.AutoExtendMonth,
                        opt => opt.MapFrom(
                            src => src.AutoExtendMonth
                        )
                    )
                    .ForMember(
                        dest => dest.UseAutoExtend,
                        opt => opt.MapFrom(
                            src => src.UseAutoExtend
                        )
                    );
            }
        );

        var planRoomGroupSites = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = 1,
            SiteId = 1
        };
        mockPlanRoomGroupSiteService.Setup(
                x => x.FindByPlanIdAndRomTypeIdAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(planRoomGroupSites);

        mockPlanRoomGroupSiteService
            .Setup(x => x.UpdateAsync(It.IsAny<PlanRoomGroupSite>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planRoomGroupSites);

        var request = new RoomGroupPriceUpdateSaleRequest
        {
            AutoExtendEveryMonthDay = 1,
            AutoExtendMonth = 1,
            UseAutoExtend = true
        };

        var command = new PlanPriceUpdateSaleCommand(1, 1, 1) { Payload = request };
        var handler = new PlanPriceUpdateSaleCommandHandler(
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockPlanRoomGroupSiteService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(planId, result);
    }
}
