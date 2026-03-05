using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetSaleQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var mockPlanRepository = new Mock<IPlanRoomGroupSiteRepository>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanRoomGroupSite, PlanRoomDetailSaleResponse>()
                    .ForMember(
                        dest => dest.PlanId,
                        opt => opt.MapFrom(
                            src => src.PlanId
                        )
                    )
                    .ForMember(
                        dest => dest.RomTypeId,
                        opt => opt.MapFrom(
                            src => src.RoomGroupId
                        )
                    )
                    .ForMember(
                        dest => dest.SiteId,
                        opt => opt.MapFrom(
                            src => src.SiteId
                        )
                    )
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

        var plans = new List<PlanRoomGroupSite>
        {
            new()
            {
                PlanId = 1,
                RoomGroupId = 1,
                SiteId = 1,
                AutoExtendEveryMonthDay = 1,
                AutoExtendMonth = 1,
                UseAutoExtend = false
            }
        };

        mockPlanRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plans.AsQueryable().BuildMock());

        var command = new PlanPriceGetSaleQuery(1, 1, 1);
        var handler = new PlanPriceGetSaleQueryHandler(
            config.CreateMapper(),
            mockPlanRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
