using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanRoomGroupSiteUpdateMinimumPriceCommandHandlerTest
{
    private readonly IMapper _mapper;

    public PlanRoomGroupSiteUpdateMinimumPriceCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanRoomGroupSiteUpdateMinimumPriceRequest, PlanRoomGroupSite>()
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
    public async Task HandleAsync_ShouldReturnPlanId_WhenPlanRoomGroupSiteUpdateMinimumPrice()
    {
        var facilityId = 1;
        long planId = 1, roomGroupId = 1, siteId = 1;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockPlanRoomGroupSiteService = new Mock<IPlanRoomGroupSiteService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var mockFacilityMinimumPriceData = new FacilityMinimumPriceData(false, 1000);
        var updatePlanRoomGroupSite = new PlanRoomGroupSite
        {
            PlanId = planId,
            RoomGroupId = roomGroupId,
            SiteId = siteId,
            IsEnabledMinimumPrice = true,
            MinimumPrice = 1000,
            IsEnabled = true
        };

        mockFacilityService
            .Setup(
                s => s.GetMinimumPriceAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockFacilityMinimumPriceData);

        mockPlanRoomGroupSiteService
            .Setup(
                s => s.UpdateMinimumPriceAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<PlanRoomGroupSite>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatePlanRoomGroupSite);

        var handler = new PlanRoomGroupSiteUpdateMinimumPriceCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockPlanRoomGroupSiteService.Object
        );

        var payload = new PlanRoomGroupSiteUpdateMinimumPriceRequest(true, 1000);
        var command = new PlanRoomGroupSiteUpdateMinimumPriceCommand(planId, roomGroupId, siteId) { Payload = payload };

        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(updatePlanRoomGroupSite.PlanId, result);
    }
}
