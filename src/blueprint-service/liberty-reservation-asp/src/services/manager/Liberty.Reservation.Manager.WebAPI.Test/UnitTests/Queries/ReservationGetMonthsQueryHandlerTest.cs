using Moq;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using AutoMapper;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using MockQueryable;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class ReservationGetMonthsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsCorrectMonths()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockReservationRepository = new Mock<IReservationRepository>();

        var facilityId = 123L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var reservations = new List<ReservationEntity>
        {
            new()
            {
                Id = 1,
                Facility = new Facility { Id = facilityId },
                CheckInDate = 20230101
            },
            new()
            {
                Id = 2,
                Facility = new Facility { Id = facilityId },
                CheckInDate = 20230301
            },
            new()
            {
                Id = 3,
                Facility = new Facility { Id = facilityId },
                CheckInDate = 20230501
            }
        };

        mockReservationRepository
            .Setup(r => r.GetQueryableWithAsNoTracking())
            .Returns(reservations.AsQueryable().BuildMock());

        var queryHandler = new ReservationGetMonthsQueryHandler(
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockReservationRepository.Object
        );

        var request = new ReservationGetMonthsQuery();

        // Act
        var (_, result) = await queryHandler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }
}
