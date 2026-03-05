using AutoMapper;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanPriceGetMinimumPriceSummaryQueryHandlerTest
{
    private readonly IMapper _mapper;

    public PlanPriceGetMinimumPriceSummaryQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            _ =>
            {
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPlanMinimumPriceSummaryResponse()
    {
        const long planId = 1;
        const long roomGroupId = 1;
        const long siteId = 1;

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<FacilityGetMinimumPriceQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    new HeaderDictionary(),
                    new FacilityMinimumPriceResponse(null, false, null)
                )
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<PlanPriceGetMinimumPriceQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    new HeaderDictionary(),
                    new PlanRoomSiteMinimumPriceResponse
                    {
                        IsEnabledMinimumPrice = false,
                        PlanId = planId,
                        RoomId = roomGroupId,
                        SiteId = siteId,
                        MinimumPrice = null
                    })
            );

        var request = new PlanPriceGetMinimumPriceSummaryQuery(
            planId,
            roomGroupId,
            siteId
        );

        var handler = new PlanPriceGetMinimumPriceSummaryQueryHandler(
            _mapper,
            mockMediator.Object
        );

        // Act
        var (_, response) = await handler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        Assert.NotNull(response);
    }
}
