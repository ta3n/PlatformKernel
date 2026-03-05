using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetPriceCalendarQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var mockPlanAppDateRepository = new Mock<IPlanRoomGroupSiteAppDateRepository>();
        var mockPlanAppDatePriceRepository = new Mock<IPlanRoomGroupSiteAppDatePriceDataRepository>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanRoomGroupSiteAppDatePriceData, RoomTypePriceDataCalendarResponse>()
                    .ForMember(
                        dest => dest.DateCalendar,
                        opt => opt.MapFrom(
                            src => src.DateCalendar
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
                cfg.CreateMap<PlanRoomGroupSiteAppDate, RoomTypePriceCalendarDataResponse>()
                    .ForMember(
                        dest => dest.DateCalendar,
                        opt => opt.MapFrom(
                            src => src.DateCalendar
                        )
                    )
                    .ForMember(
                        dest => dest.UseAutoDiscount,
                        opt => opt.MapFrom(
                            src => src.UseAutoDiscount
                        )
                    )
                    .ForMember(
                        dest => dest.PointRate,
                        opt => opt.MapFrom(
                            src => src.PointRate
                        )
                    );
            }
        );

        var plansAppDatePrice = new List<PlanRoomGroupSiteAppDatePriceData>
        {
            new()
            {
                PlanId = 1,
                RoomGroupId = 1,
                SiteId = 1,
                PriceData = new()
                {
                    Id = 1,
                    PersonMin = 1,
                    PersonMax = 3,
                    Price = 100
                },
                DateCalendar = 20250111
            }
        };
        var plansAppDate = new List<PlanRoomGroupSiteAppDate>
        {
            new()
            {
                PlanId = 1,
                RoomGroupId = 1,
                SiteId = 1,
                DateCalendar = 20250111
            }
        };

        mockPlanAppDateRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plansAppDate.AsQueryable().BuildMock());
        mockPlanAppDatePriceRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plansAppDatePrice.AsQueryable().BuildMock());

        var command = new PlanPriceGetPriceCalendarQuery(1, 1, 1, 20250101, 20250202);
        var handler = new PlanPriceGetPriceCalendarQueryHandler(
            config.CreateMapper(),
            mockPlanAppDateRepository.Object,
            mockPlanAppDatePriceRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
