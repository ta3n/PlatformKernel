using SharedKernel.Cache.Inventory;

namespace SharedKernel.Cache.Test.Integration;

public sealed class RedisInventoryServiceIntegrationTests(
    RedisContainerFixture fixture
) : RedisIntegrationTestBase(fixture)
{
    private const int InventoryDatabase = 3;

    [Fact]
    public async Task SeedAvailability_AndGetAvailability_WorkForMultipleDates()
    {
        var dates = RedisInventoryHelper.ExpandStayDates(
            new DateOnly(2026, 3, 24),
            new DateOnly(2026, 3, 27)
        );

        await RedisInventoryService.SeedAvailabilityAsync(
            "room-1",
            new Dictionary<DateOnly, long>
            {
                [dates[0]] = 2,
                [dates[1]] = 1,
                [dates[2]] = 3
            },
            InventoryDatabase
        );

        var availability = (await RedisInventoryService.GetAvailabilityAsync(
            "room-1",
            [dates[2], dates[0], dates[1], dates[1]],
            InventoryDatabase
        )).ToArray();

        Assert.Equal(3, availability.Length);
        Assert.Equal(2, availability[0].AvailableQuantity);
        Assert.Equal(1, availability[1].AvailableQuantity);
        Assert.Equal(3, availability[2].AvailableQuantity);
    }

    [Fact]
    public async Task ReserveAndConfirm_WorkAcrossDateRange()
    {
        var dates = RedisInventoryHelper.ExpandStayDates(
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 4)
        );

        await RedisInventoryService.SeedAvailabilityAsync(
            "room-2",
            dates.ToDictionary(date => date, _ => 1L),
            InventoryDatabase
        );

        var reservation = await RedisInventoryService.ReserveAsync(
            new RedisInventoryReservationRequest
            {
                ReservationId = "reservation-1",
                InventoryId = "room-2",
                Dates = dates,
                Quantity = 1,
                HoldTtl = TimeSpan.FromSeconds(30),
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );
        var snapshot = await RedisInventoryService.GetReservationAsync("reservation-1", InventoryDatabase);
        var availability = await RedisInventoryService.GetAvailabilityAsync("room-2", dates, InventoryDatabase);
        var confirmed = await RedisInventoryService.ConfirmAsync(
            new RedisInventoryConfirmRequest
            {
                ReservationId = "reservation-1",
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );
        var confirmedAgain = await RedisInventoryService.ConfirmAsync(
            new RedisInventoryConfirmRequest
            {
                ReservationId = "reservation-1",
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );

        Assert.True(reservation.Success);
        Assert.NotNull(snapshot);
        Assert.Equal("reserved", snapshot!.Status);
        Assert.All(availability, item => Assert.Equal(0, item.AvailableQuantity));
        Assert.Equal("CONFIRMED", confirmed.Code);
        Assert.Equal("EXISTS", confirmedAgain.Code);
    }

    [Fact]
    public async Task ReserveFailsAtomically_WhenOneDateHasNoStock()
    {
        var dates = RedisInventoryHelper.ExpandStayDates(
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 4)
        );

        await RedisInventoryService.SeedAvailabilityAsync(
            "room-3",
            new Dictionary<DateOnly, long>
            {
                [dates[0]] = 1,
                [dates[1]] = 0,
                [dates[2]] = 1
            },
            InventoryDatabase
        );

        var result = await RedisInventoryService.ReserveAsync(
            new RedisInventoryReservationRequest
            {
                ReservationId = "reservation-atomic",
                InventoryId = "room-3",
                Dates = dates,
                Quantity = 1,
                HoldTtl = TimeSpan.FromSeconds(30),
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );
        var availability = await RedisInventoryService.GetAvailabilityAsync("room-3", dates, InventoryDatabase);

        Assert.Equal("INSUFFICIENT", result.Code);
        Assert.Equal("20260502", result.FailedDate);
        Assert.Equal([1L, 0L, 1L], availability.Select(static item => item.AvailableQuantity).ToArray());
    }

    [Fact]
    public async Task ReleaseRestoresStock_AndIsIdempotent()
    {
        var dates = RedisInventoryHelper.ExpandStayDates(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 3)
        );

        await RedisInventoryService.SeedAvailabilityAsync(
            "room-4",
            dates.ToDictionary(date => date, _ => 1L),
            InventoryDatabase
        );
        await RedisInventoryService.ReserveAsync(
            new RedisInventoryReservationRequest
            {
                ReservationId = "reservation-release",
                InventoryId = "room-4",
                Dates = dates,
                Quantity = 1,
                HoldTtl = TimeSpan.FromSeconds(30),
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );

        var released = await RedisInventoryService.ReleaseAsync(
            new RedisInventoryReleaseRequest
            {
                ReservationId = "reservation-release",
                Reason = "cancelled",
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );
        var releasedAgain = await RedisInventoryService.ReleaseAsync(
            new RedisInventoryReleaseRequest
            {
                ReservationId = "reservation-release",
                Reason = "cancelled",
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );
        var availability = await RedisInventoryService.GetAvailabilityAsync("room-4", dates, InventoryDatabase);

        Assert.Equal("RELEASED", released.Code);
        Assert.Equal("EXISTS", releasedAgain.Code);
        Assert.All(availability, item => Assert.Equal(1, item.AvailableQuantity));
    }

    [Fact]
    public async Task ReleaseExpiredReservations_RestoresStock()
    {
        var dates = RedisInventoryHelper.ExpandStayDates(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 3)
        );

        await RedisInventoryService.SeedAvailabilityAsync(
            "room-5",
            dates.ToDictionary(date => date, _ => 1L),
            InventoryDatabase
        );
        await RedisInventoryService.ReserveAsync(
            new RedisInventoryReservationRequest
            {
                ReservationId = "reservation-expired",
                InventoryId = "room-5",
                Dates = dates,
                Quantity = 1,
                HoldTtl = TimeSpan.FromSeconds(1),
                ReservationDataTtl = TimeSpan.FromMinutes(5),
                Database = InventoryDatabase
            }
        );

        await Task.Delay(TimeSpan.FromSeconds(2));

        var expiredIds = await RedisInventoryService.GetExpiredReservationIdsAsync(
            DateTimeOffset.UtcNow,
            database: InventoryDatabase
        );
        var results = await RedisInventoryService.ReleaseExpiredReservationsAsync(
            DateTimeOffset.UtcNow,
            database: InventoryDatabase
        );
        var snapshot = await RedisInventoryService.GetReservationAsync("reservation-expired", InventoryDatabase);
        var availability = await RedisInventoryService.GetAvailabilityAsync("room-5", dates, InventoryDatabase);

        Assert.Contains("reservation-expired", expiredIds);
        Assert.Contains(results, result => result.Code == "RELEASED");
        Assert.NotNull(snapshot);
        Assert.Equal("released", snapshot!.Status);
        Assert.All(availability, item => Assert.Equal(1, item.AvailableQuantity));
    }

    [Fact]
    public async Task Reserve_AllowsSingleWinnerUnderConcurrency()
    {
        var stayDates = RedisInventoryHelper.ExpandStayDates(
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 2)
        );

        await RedisInventoryService.SeedAvailabilityAsync(
            "room-6",
            stayDates.ToDictionary(date => date, _ => 1L),
            InventoryDatabase
        );

        var tasks = Enumerable.Range(1, 20)
            .Select(
                index => RedisInventoryService.ReserveAsync(
                    new RedisInventoryReservationRequest
                    {
                        ReservationId = $"reservation-concurrency-{index}",
                        InventoryId = "room-6",
                        Dates = stayDates,
                        Quantity = 1,
                        HoldTtl = TimeSpan.FromSeconds(30),
                        ReservationDataTtl = TimeSpan.FromMinutes(5),
                        Database = InventoryDatabase
                    }
                )
            );

        var results = await Task.WhenAll(tasks);
        var availability = await RedisInventoryService.GetAvailabilityAsync("room-6", stayDates, InventoryDatabase);

        Assert.Equal(1, results.Count(static result => result.Code == "RESERVED"));
        Assert.Equal(19, results.Count(static result => result.Code == "INSUFFICIENT"));
        Assert.All(availability, item => Assert.Equal(0, item.AvailableQuantity));
    }

    [Fact]
    public async Task InvalidReservationRequest_IsRejected()
    {
        var result = await RedisInventoryService.ReserveAsync(
            new RedisInventoryReservationRequest
            {
                ReservationId = "invalid",
                InventoryId = string.Empty,
                Dates = [],
                Quantity = 0,
                Database = InventoryDatabase
            }
        );

        Assert.Equal("INVALID_REQUEST", result.Code);
        Assert.False(result.Success);
    }
}
