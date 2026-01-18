using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Newtonsoft.Json;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class GetPaymentInfoReservationsIntTest : BaseReservationEndpointIntTest
{
    [Fact]
    public async Task GetPaymentInfoReservations_ReturnOK()
    {
        var systemConfigRepo = Factory.GetRequiredService<ISystemConfigRepository>()!;
        var systemConfig = new SystemConfig
        {
            Code = "Test",
            CanOnlinePayment = true,
            IsEnabled = true,
            TemplateFormatData = null
        };
        _ = await systemConfigRepo.AddAsync(systemConfig, true);
        var reservationId = await GetReservation();
        var response = await GetPaymentInfo(reservationId);
        Assert.NotNull(response);
    }

    private async Task<GmoPaymentResponse?> GetPaymentInfo(
        long reservationId
    )
    {
        var response = await Client.GetAsync($"{BaseUrl}/{reservationId}/payment-info");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var gmoPaymentResponse = JsonConvert.DeserializeObject<GmoPaymentResponse>(responseString);

        return gmoPaymentResponse;
    }

    private async Task<long> GetReservation()
    {
        var orderReservationRepo = Factory.GetRequiredService<IBookingOrderReservationRepository>();
        var existing = await orderReservationRepo?.GetAllAsync()!;
        if (existing is { Count: > 0 })
        {
            return existing[0].ReservationId;
        }

        var plan = await CreatePlanAsync();
        var roomGroup = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();

        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);

        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var mockFacility = new Facility
        {
            Id = 1,
            Code = EntityUtil.CreateCode(),
            CanOnLinePayment = true,
            IsOnLinePayment = true
        };
        var facility = await facilityRepo!.AddAsync(mockFacility, true);

        var reservationRepo = Factory.GetRequiredService<IReservationRepository>();
        var mockReservation =
            new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
            {
                Code = EntityUtil.CreateCode(),
                FacilityId = facility.Id,
                PlanId = plan.Id,
                SiteId = site.Id,
                RoomGroupId = roomGroup.Id,
                UserCode = MockUserCode,
                MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
                Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
                IsEnabled = true,
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(5)),
                PaymentType = Reservation.Application.Constants.PaymentTypes.OnLinePayment,
                ReservationDateTime = DateTime.UtcNow.AddDays(1),
                ReservationState = Reservation
                    .Application
                    .Constants
                    .ReservationStatus
                    .Temporary,
                BookingData = new Reservation.Application.Contexts.DataContexts.Entities.Metas.BookingData
                {
                    Facility = new Reservation.Application.Contexts.DataContexts.Entities.Metas.FacilityData(),
                    Plan = new Reservation.Application.Contexts.DataContexts.Entities.Metas.PlanData(),
                    RoomGroup = new Reservation.Application.Contexts.DataContexts.Entities.Metas.RoomGroupData(),
                    Site = new Reservation.Application.Contexts.DataContexts.Entities.Metas.SiteData(),
                    TotalRoomPrice = 100,
                    TotalSpaTax = 0,
                    TotalOptionPrice = 100,
                    UsedPoint = 0
                }
            };
        var newReservation = await reservationRepo!.AddAsync(mockReservation, true);

        var unixTimestamp = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
        var orderId = $"order{unixTimestamp}";
        var mockOrderReservation = new OrderReservation
        {
            ReservationId = newReservation.Id,
            Order = new Order
            {
                Code = EntityUtil.CreateCode(),
                ApiIssueCode = orderId,
                OrderDateTime = DateTime.UtcNow
            }
        };
        await orderReservationRepo.AddAsync(mockOrderReservation, true);

        return newReservation.Id;
    }
}
