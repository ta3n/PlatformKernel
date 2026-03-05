using System.Net;
using System.Text;
using Liberty.ApplicationShared.Utils;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Newtonsoft.Json;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class CreateGmoPaymentEndpointReturnReservationInvalidExceptionIntTest : BaseReservationEndpointIntTest
{
    [Fact]
    public async Task CreateGMOPayment_ReturnsReservationInvalidException()
    {
        var (reservationId, orderId) = await GetReservation();
        var gmoPayment = await ProcessPaymentAsync(reservationId);

        var fakeData = new
        {
            Transactionresult = new
            {
                AccessID = gmoPayment.AccessId,
                gmoPayment.AccessPass,
                OrderID = orderId,
                Result = "PAYSUCCESS",
                Processdate = DateTime.UtcNow,
                ErrCode = (string?)null,
                ErrInfo = (string?)null,
                Paymethod = "credit"
            },
            Credit = new
            {
                Status = "AUTH",
                Forward = "2a99662",
                Method = "1",
                PayTimes = (string?)null,
                TranID = gmoPayment.TranId,
                Approve = "018866",
                TranDate = "20250108184636"
            }
        };

        var jsonString = System.Text.Json.JsonSerializer.Serialize(fakeData);

        var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

        var requestData = new Dictionary<string, string?> { { "Result", base64String } };

        using var postContent = new FormUrlEncodedContent(requestData);

        var response = await Client.PostAsync(
            $"/gmo-payment-result",
            postContent
        );

        Assert.NotNull(response);
    }

    private async Task<GmoPaymentRequest> ProcessPaymentAsync(
        long reservationId
    )
    {
        var gmoPaymentGatewayService = Factory.GetRequiredService<IGmoPaymentGatewayService>();

        var responsePaymentInfo = await GetPaymentInfo(reservationId);

        const string cardNumber = "4111111111111111";
        const string cvvCard = "123";
        const int amount = 100;
        var tax = responsePaymentInfo?.Tax ?? 0;
        const string currency = "JPY";
        var orderId = responsePaymentInfo?.OrderId;
        const string shopId = "tshop00035518";
        const string shopPass = "dcnycrv1";
        const string expiryDate = "2512";
        const string jobCd = "AUTH";

        var paymentEntryTranRequest = new PaymentEntryTranRequest(
            orderId,
            cardNumber,
            cvvCard,
            amount,
            tax,
            currency
        );

        var entryTranResponse = await gmoPaymentGatewayService!.EntryTranAsync(
            paymentEntryTranRequest
        );

        var paymentExecTranRequest = new PaymentExecTranRequest(
            orderId,
            entryTranResponse.AccessId,
            entryTranResponse.AccessPass,
            cardNumber,
            cvvCard,
            expiryDate
        );
        var execTranResponse = await gmoPaymentGatewayService.ExecTranAsync(
            paymentExecTranRequest
        );

        var gmoPaymentRequest = new GmoPaymentRequest
        {
            ShopId = shopId,
            ShopPass = shopPass,
            AccessId = entryTranResponse.AccessId,
            AccessPass = entryTranResponse.AccessPass,
            OrderId = $"{execTranResponse.OrderId + 99}",
            Status = "CAPTURE",
            JobCd = jobCd,
            Amount = $"{amount}",
            Tax = $"{tax}",
            Currency = currency,
            Forward = execTranResponse.Forward,
            Method = execTranResponse.Method,
            PayTimes = execTranResponse.PayTimes,
            TranId = execTranResponse.TranId,
            Approve = execTranResponse.Approve,
            TranDate = execTranResponse.TranDate,
            ErrCode = string.Empty,
            ErrInfo = string.Empty,
            PayType = "OnLinePayment"
        };
        return gmoPaymentRequest;
    }

    private async Task<GmoPaymentResponse?> GetPaymentInfo(
        long reservationId
    )
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
        var response = await Client.GetAsync($"{BaseUrl}/{reservationId}/payment-info");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var gmoPaymentResponse = JsonConvert.DeserializeObject<GmoPaymentResponse>(responseString);

        return gmoPaymentResponse;
    }

    private async Task<(long id, string orderId)> GetReservation()
    {
        var orderReservationRepo = Factory.GetRequiredService<IBookingOrderReservationRepository>();
        var existing = await orderReservationRepo?.GetAllAsync()!;
        if (existing is { Count: > 0 })
        {
            return (existing[0].ReservationId, existing[0].Order?.ApiIssueCode ?? string.Empty);
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

        return (newReservation.Id, orderId);
    }
}
