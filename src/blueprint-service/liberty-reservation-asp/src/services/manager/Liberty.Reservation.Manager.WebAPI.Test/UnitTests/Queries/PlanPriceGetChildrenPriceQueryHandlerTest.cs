using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetChildrenPriceQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var mockPlanRepository = new Mock<IPlanRoomGroupSitePersonAgeTypeRepository>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                cfg.CreateMap<PlanRoomGroupSitePersonAgeType, RoomTypeChildrenPersonAgeTypeResponse>()
                    .ForMember(
                        dest => dest.PersonAgeTypeId,
                        opt => opt.MapFrom(
                            src => src.PersonAgeTypeId
                        )
                    )
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(
                            src => src.PersonAgeType!.Name!.GetValueByHeader()
                        )
                    )
                    .ForMember(
                        dest => dest.IsEnabled,
                        opt => opt.MapFrom(
                            src => src.IsEnabled
                        )
                    )
                    .ForMember(
                        dest => dest.IsRegardAdult,
                        opt => opt.MapFrom(
                            src => src.IsRegardAdult
                        )
                    )
                    .ForMember(
                        dest => dest.Value,
                        opt => opt.MapFrom(
                            src => src.Value
                        )
                    )
                    .ForMember(
                        dest => dest.PriceSettingType,
                        opt => opt.MapFrom(
                            src => src.PriceSettingType
                        )
                    )
                    .ForMember(
                        dest => dest.IsMain,
                        opt => opt.MapFrom(
                            src => src.PersonAgeType!.IsMain
                        )
                    )
                    .ForMember(
                        dest => dest.DisplayOrder,
                        opt => opt.MapFrom(
                            src => src.PersonAgeType!.DisplayOrder
                        )
                    );
            }
        );

        var plans = new List<PlanRoomGroupSitePersonAgeType>
        {
            new()
            {
                PlanId = 1,
                RoomGroupId = 1,
                SiteId = 1,
                PersonAgeType = new Reservation.Application.Contexts.DataContexts.Entities.Data.PersonAgeType
                {
                    Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "pat" } },
                    Id = 1,
                    IsEnabled = true,
                    IsVisible = true
                },
                PersonAgeTypeId = 1
            }
        };

        mockPlanRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plans.AsQueryable().BuildMock());

        var command = new PlanPriceGetChildrenPriceQuery(1, 1, 1);
        var handler = new PlanPriceGetChildrenPriceQueryHandler(
            config.CreateMapper(),
            mockPlanRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
