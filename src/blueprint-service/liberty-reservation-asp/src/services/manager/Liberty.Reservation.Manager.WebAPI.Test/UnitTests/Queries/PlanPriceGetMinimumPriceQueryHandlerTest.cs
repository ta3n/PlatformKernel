using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetMinimumPriceQueryHandlerTest
{
    private readonly IMapper _mapper;

    public PlanPriceGetMinimumPriceQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanRoomGroupSite, PlanRoomSiteMinimumPriceResponse>()
                    .ForMember(
                        dest => dest.PlanId,
                        opt => opt.MapFrom(
                            src => src.PlanId
                        )
                    )
                    .ForMember(
                        dest => dest.RoomId,
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
                        dest => dest.IsEnabledMinimumPrice,
                        opt => opt.MapFrom(
                            src => src.IsEnabledMinimumPrice
                        )
                    )
                    .ForMember(
                        dest => dest.MinimumPrice,
                        opt => opt.MapFrom(
                            src => src.MinimumPrice
                        )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPlanMinimumPriceResponse()
    {
        long planId = 1, roomGroupId = 1, siteId = 1;
        var mockPlanRoomGroupSiteRepository = new Mock<IPlanRoomGroupSiteRepository>();

        var planRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = roomGroupId,
            SiteId = siteId,
            IsEnabledMinimumPrice = true,
            MinimumPrice = 1000,
            IsEnabled = true
        };

        var mockPlanRoomGroupSiteBuild = new List<PlanRoomGroupSite> { planRoomGroupSite }.AsQueryable().BuildMock();
        mockPlanRoomGroupSiteRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockPlanRoomGroupSiteBuild);

        var request = new PlanPriceGetMinimumPriceQuery(planId, roomGroupId, siteId);

        var handler = new PlanPriceGetMinimumPriceQueryHandler(
            _mapper,
            mockPlanRoomGroupSiteRepository.Object
        );

        // Act
        var (_, response) = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }
}
