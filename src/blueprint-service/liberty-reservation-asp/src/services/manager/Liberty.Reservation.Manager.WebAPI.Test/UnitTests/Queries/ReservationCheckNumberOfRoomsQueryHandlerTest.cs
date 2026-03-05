using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;
using MockQueryable;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class ReservationCheckNumberOfRoomsQueryHandlerTest
{
    [Fact]
    public async Task ShouldReturnTrue_WhenRoomAvailable()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var roomGroupAppDateRepositoryMock = new Mock<IRoomGroupAppDateRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        securityContextAccessorMock.Setup(s => s.FacilityKey).Returns(1);
        var userCode = "test-user-code";
        var query = new ReservationCheckNumberOfRoomsQuery(1);

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            UserCode = userCode,
            PlanId = 1,
            RoomGroupId = 1,
            SiteId = 1,
            Facility = new() { Id = 1 },
            Site = new() { Id = 1 },
            RoomGroup = new() { Id = 1 },
            Plan = new() { Id = 1 },
            RestNumber = 3,
            CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1))
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new[] { existingReservation }.AsQueryable().BuildMock());

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = 1,
                            IsEnabled = true,
                            IsOnLinePayment = true,
                            UseDaySaleLimit = true,
                            RoomNumberDaySaleLimit = 5,
                            Cancellation = new Cancellation { IsEnabled = true }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupAppDateRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroupAppDate>
                    {
                        new()
                        {
                            AppDateId = existingReservation.CheckInDate,
                            RoomGroupId = existingReservation.RoomGroupId,
                            SellNumber = 2,
                            RoomGroup = new RoomGroup { IsEnabled = true },
                            IsNotSelled = false
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        var handler = new ReservationCheckNumberOfRoomsQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            roomGroupAppDateRepositoryMock.Object,
            planRepositoryMock.Object
        );

        // Act
        var (_, result) = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ShouldReturnReservationNotfoundException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var roomGroupAppDateRepositoryMock = new Mock<IRoomGroupAppDateRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();

        var userCode = "test-user-code";
        var query = new ReservationCheckNumberOfRoomsQuery(1);

        var existingReservation = new ReservationEntity
        {
            CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
            RoomGroupId = 1,
            RoomNumber = 5,
            PlanId = 1
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new[] { existingReservation }.AsQueryable().BuildMock());

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = 1,
                            IsEnabled = true,
                            IsOnLinePayment = true,
                            UseDaySaleLimit = true,
                            RoomNumberDaySaleLimit = 4
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupAppDateRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroupAppDate>
                    {
                        new()
                        {
                            AppDateId = existingReservation.CheckInDate,
                            RoomGroupId = existingReservation.RoomGroupId,
                            SellNumber = 5,
                            RoomGroup = new RoomGroup { IsEnabled = true },
                            IsNotSelled = false
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        var handler = new ReservationCheckNumberOfRoomsQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            roomGroupAppDateRepositoryMock.Object,
            planRepositoryMock.Object
        );

        // Act

        await Assert.ThrowsAsync<ReservationNotfoundException>(
            async () => await handler.Handle(query, cancellationToken)
        );
    }
}
