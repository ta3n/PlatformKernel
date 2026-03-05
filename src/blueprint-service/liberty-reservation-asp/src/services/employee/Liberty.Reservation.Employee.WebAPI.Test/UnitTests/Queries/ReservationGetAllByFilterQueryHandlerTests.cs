using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class ReservationGetAllByFilterQueryHandlerTests
{
    private readonly ReservationGetAllByFilterQueryHandler _handler;

    public ReservationGetAllByFilterQueryHandlerTests()
    {
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();

        _handler = new ReservationGetAllByFilterQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            reservationRepositoryMock.Object
        );
    }

    [Fact]
    public void GetCacheKey_ShouldGenerateCorrectKey()
    {
        // Arrange
        var request = new ReservationGetAllByFilterQuery(
            new(
                null, // FacilitySearch
                null, // SiteSearch
                null, // RoomSearch
                null, // PlanSearch
                null, // Code
                null, // Name
                null, // Kana
                null, // PostCode
                null, // Address1
                null, // PhoneNumber
                null, // EMail
                null, // FreeInput
                null, // StartCheckInDate
                null, // EndCheckInDate
                null, // StartReservationAcceptanceDate
                null, // EndReservationAcceptanceDate
                null, // StartCancellationDate
                null, // EndCancellationDate
                null, // StartNoShowDate
                null, // EndNoShowDate
                null, // IsNoShow
                false,
                null, // ReservationStatus
                null // PaymentTypes
            ),
            Pageable.Of(1, 1000)
        );

        // Act
        var cacheKey = _handler.GetType()
            .GetMethod("GetCacheKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_handler, [request]) as string;

        // Assert
        Assert.NotNull(cacheKey);
        Assert.Contains(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation),
            cacheKey
        );
        Assert.Contains(CacheKeys.AllFacilityBookingSearchPrefixKey, cacheKey);
    }
}
