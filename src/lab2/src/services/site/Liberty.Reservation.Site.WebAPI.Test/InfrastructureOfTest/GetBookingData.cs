using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

public class GetBookingData(
    AppWebApplicationFactory<TestStartup> factory,
    HttpClient httpClient
)
{
    private async Task<IEnumerable<PersonAgeType>> GetAllPersonAgeTypesAsync()
    {
        var personAgeTypeRepo = factory.GetRequiredService<IFacilityPersonAgeTypeRepository>()
            ?? throw new ArgumentException(nameof(IFacilityPersonAgeTypeRepository));
        var existingPersonAgeTypes = await personAgeTypeRepo
            .GetQueryableWithAsNoTracking()
            .Select(x => x.PersonAgeType)
            .ToListAsync();
        return existingPersonAgeTypes!;
    }

    public async Task<(IEnumerable<ReservationEntity> Reservations, List<PersonAgeType> PersonAgeTypes)> GetReservationsAsync()
    {
        var personAgeTypes = await GetAllPersonAgeTypesAsync();
        var reservationRepo = factory.GetRequiredService<IReservationRepository>()
            ?? throw new ArgumentException(nameof(IReservationRepository));
        var existingReservations = await reservationRepo.GetAllAsync();
        return (existingReservations, personAgeTypes.ToList());
    }

    public async Task<Plan> GetPlan(
        bool? isOnLinePayment = true,
        PlanTypes planType = PlanTypes.Combo
    )
    {
        var planRepo = factory.GetRequiredService<IPlanRepository>()
            ?? throw new ArgumentException(nameof(IPlanRepository));
        var plans = await planRepo.GetAllAsync();
        foreach (var plan in plans)
        {
            plan.IsEnabled = true;
            plan.IsOnLinePayment = isOnLinePayment ?? true;
            plan.IsOnSidePayment = true;
            plan.NumberOfStayLimitMax = 10;
            plan.CheckInEnd = new TimeSpan(23, 59, 0);
            plan.ReceptionDayLimit = -1;
            plan.UseReceptionStartDay = false;
            //plan.
        }

        _ = await planRepo.UpdateRangeAsync(plans, true);
        var existingPlan = await planRepo
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Cancellation)
            .Where(x => x.PlanType == planType)
            .FirstOrDefaultAsync();

        return existingPlan!;
    }

    public async Task<RoomGroup> GetRoomGroup()
    {
        var roomGroupRepo = factory.GetRequiredService<IRoomGroupRepository>()
            ?? throw new ArgumentException(nameof(IRoomGroupRepository));
        var roomGroups = await roomGroupRepo.GetAllAsync();
        var roomGroupUpdates = new List<RoomGroup>();
        foreach (var roomGroup in roomGroups)
        {
            roomGroup.IsEnabled = true;
            roomGroupUpdates.Add(roomGroup);
        }

        _ = await roomGroupRepo.UpdateRangeAsync(roomGroupUpdates, true);
        var existingRoomGroup = await roomGroupRepo
            .GetQueryableWithAsNoTracking()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync();
        return existingRoomGroup!;
    }

    public async Task<(IEnumerable<BookingSearchByPlanResponse>?, Plan)> GetAllBooking(
        bool? isOnLinePayment = true
    )
    {
        var plan = await GetPlan(isOnLinePayment);
        var checkInDate = plan.DisplayDateStart;
        var roomNumber = 1;
        List<PersonOfBookingSearchModel> rooms =
        [
            new()
            {
                AppDateId = plan.DisplayDateStart ?? AppDate.GetId(DateTime.UtcNow),
                RestIndex = 1,
                RoomGroupIndex = 1,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];

        var displayCheckInDate = checkInDate ?? AppDate.GetId(DateTime.UtcNow);
        var displayCheckOutDate = checkInDate.HasValue
            ? AppDate.GetId(AppDate.GetDateTime(checkInDate)?.AddDays(1)) ?? 0
            : AppDate.GetId(DateTime.UtcNow.AddDays(1));

        var bookingRequest = new BookingSearchPlanRequest
        {
            CheckInDate = displayCheckInDate,
            CheckOutDate = displayCheckOutDate,
            DisplayCheckInDate = displayCheckInDate,
            DisplayCheckOutDate = displayCheckOutDate,
            Secret = null,
            MaxPrice = null,
            MinPrice = null,
            OptionItems = null,
            RestNumber = 1,
            RoomNumber = roomNumber,
            GuestsPerRoom = rooms
        };

        var payload = TestUtil.ToJsonContent(bookingRequest);
        var response = await httpClient.PostAsync(
            "api/booking/search",
            payload
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        var bookingRes = JsonConvert.DeserializeObject<IEnumerable<BookingSearchByPlanResponse>>(responseString);
        return (bookingRes, plan);
    }
}
