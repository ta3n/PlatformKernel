using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Queries;

public class BookingSearchQueryHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanOfBookingResponse_WhenValidRequest()
    {
        var mediatorMock = new Mock<IMediator>();
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var cacheServiceMock = new Mock<ICacheService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingPlanPriceServiceMock = new Mock<IBookingCalendarPriceService>();

        var mapperMock = new Mock<IMapper>();
        var facilityCode = "FAC123";
        var siteId = 1;
        var facilityId = 1;

        var plans = new Page<BookingPlanModel>(
            new List<BookingPlanModel>(),
            new Mock<IPageable>().Object,
            1
        );

        var payload = new BookingSearchPlanRequest
        {
            GuestsPerRoom = null,
            RestNumber = 2,
            RoomNumber = 1,
            CheckInDate = 20250101,
            CheckOutDate = 20250105,
            DisplayCheckInDate = 20250101,
            DisplayCheckOutDate = 20250105,
            MinPrice = 100,
            MaxPrice = 500
        };

        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new BookingSearchByPlanQuery(payload, pageable);

        facilityExternalRepositoryMock
            .Setup(x => x.CheckFacilityAvailableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        securityContextAccessorMock.Setup(x => x.GetFacilityCodeSelected()).Returns(facilityCode);
        securityContextAccessorMock.Setup(x => x.GetSiteIdSelected()).Returns(siteId);
        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);

        bookingSearchServiceMock
            .Setup(
                x => x.GetAllBookingDataPlansAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<IPageable>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(plans);

        bookingPlanPriceServiceMock
            .Setup(
                x => x.GetAllRoomDatePricesInPlans(
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<List<BookingPlanModel>>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>()
                )
            )
            .Returns(
                [
                    new(
                        1,
                        "Plan 1",
                        "Tag",
                        true,
                        false,
                        "Summary",
                        false,
                        null,
                        null,
                        false,
                        null,
                        null,
                        PlanTypes.Combo,
                        1,
                        "des",
                        false
                    )
                ]
            );

        cacheServiceMock
            .Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        var handler = new BookingSearchByPlanQueryHandler(
            mapperMock.Object,
            mediatorMock.Object,
            securityContextAccessorMock.Object,
            cacheServiceMock.Object
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<IEnumerable<BookingSearchByPlanResponse>>(result.Item2, false);
    }
}
