using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using MockQueryable;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Queries;

public class ReservationCheckNumberOfNightsQueryHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenNumberOfNightsIsValid()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var planRoomGroupSiteAppDatePriceDataRepositoryMock = new Mock<IPlanRoomGroupSiteAppDatePriceDataRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<Plan> { }.AsQueryable().BuildMock());

        var userCode = "test-user-code";
        var query = new ReservationCheckNumberOfNightsQuery(1);

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            UserCode = userCode,
            PlanId = 1,
            RoomGroupId = 1,
            SiteId = 1,
            RestNumber = 3,
            CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1))
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey)
            .Returns(userCode);

        var priceDataList = new List<PlanRoomGroupSiteAppDatePriceData>
        {
            new()
            {
                DateCalendar = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                PriceData = new PriceData { Price = 100 }
            },
            new()
            {
                DateCalendar = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                PriceData = new PriceData { Price = 100 }
            },
            new()
            {
                DateCalendar = AppDate.GetId(DateTime.UtcNow.AddDays(3)),
                PriceData = new PriceData { Price = 100 }
            }
        };

        var reservations = new List<ReservationEntity> { existingReservation }
            .AsQueryable()
            .BuildMock();

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(reservations);

        planRoomGroupSiteAppDatePriceDataRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(priceDataList.AsQueryable().BuildMock());

        var handler = new ReservationCheckNumberOfNightsQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            planRoomGroupSiteAppDatePriceDataRepositoryMock.Object,
            planRepositoryMock.Object
        );

        // Act
        var (_, result) = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnReservationNotfoundException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var planRoomGroupSiteAppDatePriceDataRepositoryMock = new Mock<IPlanRoomGroupSiteAppDatePriceDataRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<Plan> { }.AsQueryable().BuildMock());

        var userCode = "test-user-code";
        var query = new ReservationCheckNumberOfNightsQuery(1);

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey)
            .Returns(userCode);

        var priceDataList = new List<PlanRoomGroupSiteAppDatePriceData>
        {
            new()
            {
                DateCalendar = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                PriceData = new PriceData { Price = 100 }
            },
            new()
            {
                DateCalendar = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
                PriceData = new PriceData { Price = 100 }
            },
            new()
            {
                DateCalendar = AppDate.GetId(DateTime.UtcNow.AddDays(3)),
                PriceData = new PriceData { Price = 100 }
            }
        };

        var reservations = new List<ReservationEntity> { }
            .AsQueryable()
            .BuildMock();

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(reservations);

        planRoomGroupSiteAppDatePriceDataRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(priceDataList.AsQueryable().BuildMock());

        var handler = new ReservationCheckNumberOfNightsQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            planRoomGroupSiteAppDatePriceDataRepositoryMock.Object,
            planRepositoryMock.Object
        );

        await Assert.ThrowsAsync<ReservationNotfoundException>(
            async () => await handler.Handle(query, cancellationToken)
        );
    }
}
