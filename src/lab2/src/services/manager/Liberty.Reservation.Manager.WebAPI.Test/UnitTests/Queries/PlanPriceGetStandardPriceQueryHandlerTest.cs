using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetStandardPriceQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var mockPlanRepository = new Mock<IPlanRoomGroupSiteAppDateTypePriceDataRepository>();
        var mockRoomRepository = new Mock<IRoomGroupRepository>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanRoomGroupSiteAppDateTypePriceData, RomTypeStandardPrinceDataResponse>()
                    .ForMember(
                        dest => dest.DateTypeId,
                        opt => opt.MapFrom(
                            src => src.AppDateTypeId
                        )
                    )
                    .ForMember(
                        dest => dest.PersonMin,
                        opt => opt.MapFrom(
                            src => src.PriceData!.PersonMin
                        )
                    )
                    .ForMember(
                        dest => dest.PersonMax,
                        opt => opt.MapFrom(
                            src => src.PriceData!.PersonMax
                        )
                    )
                    .ForMember(
                        dest => dest.Price,
                        opt => opt.MapFrom(
                            src => src.PriceData!.Price
                        )
                    );
            }
        );

        var plans = new List<PlanRoomGroupSiteAppDateTypePriceData>
        {
            new()
            {
                PlanId = 1,
                RoomGroupId = 1,
                SiteId = 1,
                AppDateType = new()
                {
                    IsEnabled = true,
                    IsDeleted = false
                },
                IsEnabled = true,
                PriceData = new()
                {
                    Id = 1,
                    PersonMin = 1,
                    PersonMax = 3,
                    Price = 100
                }
            }
        };

        mockPlanRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plans.AsQueryable().BuildMock());

        var rooms = new List<RoomGroup>
        {
            new()
            {
                Id = 1,
                CapacityMax = 3,
                CapacityMin = 1
            }
        };
        mockRoomRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(rooms.AsQueryable().BuildMock());

        var command = new PlanPriceGetStandardPriceQuery(1, 1, 1);
        var handler = new PlanPriceGetStandardPriceQueryHandler(
            config.CreateMapper(),
            mockPlanRepository.Object,
            mockRoomRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
