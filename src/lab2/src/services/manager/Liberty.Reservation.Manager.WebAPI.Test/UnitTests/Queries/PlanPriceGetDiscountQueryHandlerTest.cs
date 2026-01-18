using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetDiscountQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var mockPlanRepository = new Mock<IPlanRoomGroupSiteDiscountDataRepository>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanRoomGroupSiteDiscountData, RoomTypeDiscountDataResponse>()
                    .ForMember(
                        dest => dest.StartPrevDay,
                        opt => opt.MapFrom(
                            src => src.DiscountData!.StartPrevDay
                        )
                    )
                    .ForMember(
                        dest => dest.EndPrevDay,
                        opt => opt.MapFrom(
                            src => src.DiscountData!.EndPrevDay
                        )
                    )
                    .ForMember(
                        dest => dest.PersonMin,
                        opt => opt.MapFrom(
                            src => src.DiscountData!.PersonMin
                        )
                    )
                    .ForMember(
                        dest => dest.PersonMax,
                        opt => opt.MapFrom(
                            src => src.DiscountData!.PersonMax
                        )
                    )
                    .ForMember(
                        dest => dest.Value,
                        opt => opt.MapFrom(
                            src => src.DiscountData!.Value
                        )
                    )
                    .ForMember(
                        dest => dest.PriceSettingType,
                        opt => opt.MapFrom(
                            src => src.DiscountData!.PriceSettingType
                        )
                    );
            }
        );

        var plans = new List<PlanRoomGroupSiteDiscountData>
        {
            new()
            {
                PlanId = 1,
                RoomGroupId = 1,
                SiteId = 1,
                DiscountData = new()
                {
                    StartPrevDay = 1,
                    EndPrevDay = 2,
                    PersonMin = 1,
                    PersonMax = 2,
                    Value = 1,
                    PriceSettingType = Reservation.Application.Constants.PriceSettingTypes.Price
                }
            }
        };

        mockPlanRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plans.AsQueryable().BuildMock());

        var command = new PlanPriceGetDiscountQuery(1, 1, 1);
        var handler = new PlanPriceGetDiscountQueryHandler(
            config.CreateMapper(),
            mockPlanRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
