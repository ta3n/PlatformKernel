using AutoMapper;
using Liberty.Pagination;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;
using MockQueryable;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class ReservationGetAllOptionItemsQueryHandlerTest
{
    [Fact]
    public async Task ShouldThrowReservationNotFoundException_WhenReservationNotFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();

        var userCode = "test-user-code";
        var query = new ReservationGetAllOptionItemsQuery(
            1,
            1,
            101,
            PageableBinderConfig.DefaultPageable
        );

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<ReservationEntity>().AsQueryable().BuildMock());

        var handler = new ReservationGetAllOptionItemsQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            optionItemRepositoryMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<ReservationNotfoundException>(
            async () => await handler.Handle(query, cancellationToken)
        );
    }
}
