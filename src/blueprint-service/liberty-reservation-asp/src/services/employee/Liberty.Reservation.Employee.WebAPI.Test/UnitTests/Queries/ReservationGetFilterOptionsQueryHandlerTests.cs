using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class ReservationGetFilterOptionsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAllFilterOptions()
    {
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();

        mockCacheService
            .Setup(
                c => c.GetAsync<ReservationFilterOptionsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((ReservationFilterOptionsResponse?)null);

        mockCacheService
            .Setup(
                c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<ReservationFilterOptionsResponse>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        var handler = new ReservationGetFilterOptionsQueryHandler(
            mockMapper.Object,
            mockCacheService.Object
        );

        var query = new ReservationGetFilterOptionsQuery();

        // Act
        var (headers, response) = await handler.Handle(
            query,
            CancellationToken.None
        );

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(headers);
        Assert.IsType<HeaderDictionary>(headers);

        Assert.NotNull(response.PaymentTypes);
        Assert.NotEmpty(response.PaymentTypes);
        var expectedPaymentTypes = Enum.GetValues<PaymentTypes>().Where(x => x != PaymentTypes.Unknown).ToList();
        Assert.Equal(expectedPaymentTypes.Count, response.PaymentTypes.ToList().Count);
        foreach (var paymentType in expectedPaymentTypes)
        {
            Assert.Contains(paymentType, response.PaymentTypes);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCachedResponse_WhenCacheExists()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();

        var cachedResponse = new ReservationFilterOptionsResponse(
            [ReservationStatus.Confirmed],
            [PaymentTypes.OnLinePayment]
        );

        mockCacheService
            .Setup(
                c => c.GetAsync<ReservationFilterOptionsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(cachedResponse);

        var handler = new ReservationGetFilterOptionsQueryHandler(
            mockMapper.Object,
            mockCacheService.Object
        );

        var query = new ReservationGetFilterOptionsQuery();

        // Act
        var (_, response) = await handler.Handle(
            query,
            CancellationToken.None
        );

        // Assert
        Assert.NotNull(response);

        mockCacheService.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<ReservationFilterOptionsResponse>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }
}
