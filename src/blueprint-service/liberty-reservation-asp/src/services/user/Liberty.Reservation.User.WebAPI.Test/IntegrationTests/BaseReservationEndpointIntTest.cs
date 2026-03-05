using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.User.Application.Contexts;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class BaseReservationEndpointIntTest : BaseIntegrationTest
{
    protected static string BaseUrl => "api/reservations";

    protected async Task<Plan> CreatePlanAsync()
    {
        var planRepo = Factory.GetRequiredService<IPlanRepository>();

        var existing = await planRepo?.GetAllAsync()!;
        if (existing is { Count: > 0 })
        {
            return existing[0];
        }

        var mockData = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            NumberOfStayLimitMin = 1,
            NumberOfStayLimitMax = int.MaxValue,
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            IsEnabled = true,
            Cancellation = new Cancellation { IsEnabled = true }
        };
        var newPlan = await planRepo.AddAsync(mockData, true);

        return newPlan;
    }

    protected async Task<RoomGroup> CreateRoomGroupAsync()
    {
        var roomGroupRepo = Factory.GetRequiredService<IRoomGroupRepository>();

        var existing = await roomGroupRepo?.GetAllAsync()!;
        if (existing is { Count: > 0 })
        {
            return existing[0];
        }

        var mockData = new RoomGroup
        {
            IsEnabled = true,
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } }
        };

        var newRoomGroup = await roomGroupRepo.AddAsync(mockData, true);

        return newRoomGroup;
    }

    protected async Task<Reservation.Application.Contexts.DataContexts.Entities.Data.Site> CreateSiteAsync()
    {
        var siteRepo = Factory.GetRequiredService<ISiteRepository>();

        var existing = await siteRepo?.GetAllAsync()!;
        if (existing is { Count: > 0 })
        {
            return existing[0];
        }

        var mockData = new Reservation.Application.Contexts.DataContexts.Entities.Data.Site
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = "Description",
            Url = "test.com",
            IsEnabled = true
        };

        var newSite = await siteRepo.AddAsync(mockData, true);

        return newSite;
    }

    protected string GetGuestCode(
        ReservationEntity reservation
    )
    {
        var bookingId = reservation.Id.ToString();
        var validMinutes = (int)(AppDate.GetDateTime(reservation.CheckInDate).AddDays(1) - DateTime.UtcNow).TotalMinutes;

        var bookingSecureUrlService = Factory.GetRequiredService<IBookingSecureUrlService>();
        var secureRequest = new BookingSecureUrlRequest(
            bookingId,
            validMinutes
        );

        var guestCode = bookingSecureUrlService!.EncryptDataWithHmacSha256(secureRequest);
        return guestCode;
    }

    protected async Task<RoomGroupAppDate> CreateRoomGroupAppDateAsync()
    {
        var roomGroup = await CreateRoomGroupAsync();
        var roomGroupAppDateRepo = Factory.GetRequiredService<IRoomGroupAppDateRepository>();
        var mockRoomGroupAppDate = new RoomGroupAppDate
        {
            RoomGroupId = roomGroup.Id,
            SellNumber = 10,
            AppDate = new AppDate
            {
                DateTime = DateTime.UtcNow,
                Id = AppDate.GetId(DateTime.UtcNow)
            },
            IsEnabled = true
        };

        var newRoomGroupAppDate = await roomGroupAppDateRepo!.AddAsync(mockRoomGroupAppDate, true);

        return newRoomGroupAppDate;
    }

    protected static HttpContent PayloadBookingPriceRequest()
    {
        var checkInDate = AppDate.GetId(DateTime.UtcNow);
        List<PersonOfBookingPriceRequest> guestsPerRoom =
        [
            new()
            {
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };
        var payload = TestUtil.ToJsonContent(bookingPrinceRequest);
        return payload;
    }

    protected async Task CreateDbFunction()
    {
        var dbContext = Factory.GetRequiredService<UserDataContext>();
        if (dbContext is null)
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            @"
                CREATE OR REPLACE FUNCTION public.reservation_diff_amount_gmo_online_payment(bigint)
                RETURNS boolean
                LANGUAGE sql
                AS $$ SELECT FALSE; $$;
            "
        );
    }
}
