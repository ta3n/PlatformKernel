using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class GuestCancellationOfReservationIntTest : BaseReservationEndpointIntTest
{
    private static new string BaseUrl => "api/guest/booking";

    [Fact]
    public async Task Guest_CancellationOfReservation_ReturnOK()
    {
        var planRepo = Factory.GetRequiredService<IPlanRepository>();

        var plan = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            NumberOfStayLimitMin = 1,
            NumberOfStayLimitMax = int.MaxValue,
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            CancelLimit = new TimeSpan(12, 0, 0),
            Cancellation = new()
            {
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                CancellationCancellationDatas = new List<CancellationCancellationData>
                {
                    new()
                    {
                        CancellationData = new()
                        {
                            Code = EntityUtil.CreateCode(),
                            DayStart = 0,
                            DayEnd = 10,
                            Rate = 1
                        }
                    }
                }
            }
        };
        await planRepo!.AddAsync(plan, true);
        var roomGroup = await CreateRoomGroupAsync();
        var site = await CreateSiteAsync();

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);
        var reservationRepository = Factory.GetRequiredService<IReservationRepository>();
        _ = reservationRepository!.GetQueryableWithAsNoTracking()
            .ToList();
        var facilityRepo = Factory.GetRequiredService<IFacilityRepository>();
        var mockFacility = new Facility
        {
            Code = EntityUtil.CreateCode(),
            IsEnabled = true
        };
        var facility = await facilityRepo!.AddAsync(mockFacility, true);

        var reservationRepo = Factory.GetRequiredService<IReservationRepository>();
        var mockReservation = new ReservationEntity
        {
            Code = EntityUtil.CreateCode(),
            FacilityId = facility.Id,
            SiteId = site.Id,
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id,
            UserCode = MockUserCode,
            MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
            Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(10)),
            RestNumber = 1,
            RoomNumber = 1,
            ReservationState = ReservationStatus.Reserved,
            IsEnabled = true,
            BookingData = new BookingData
            {
                Plan = new PlanData
                {
                    CancellationDataPolicy = new BookingCancellationPolicyModel
                    {
                        Description = new MultilingualText
                        {
                            { "ja", "Cancel Name" },
                            { "en", "Cancel Name" }
                        },
                        Name = new MultilingualText
                        {
                            { "ja", "Cancel Name" },
                            { "en", "Cancel Name" }
                        },
                        TableSource = new MultilingualText
                        {
                            { "ja", "<html></html>" },
                            { "en", "<html></html>" }
                        },
                        CanOnLinePayment = true,
                        PaymentLimit = 1,
                        CancellationData =
                        [
                            new BookingCancellationDataPolicyModel
                            {
                                DayEnd = 10,
                                DayStart = 0,
                                Rate = 1
                            }
                        ]
                    }
                },
                Facility = new FacilityData(),
                RoomGroup = new RoomGroupData(),
                Site = new SiteData()
            }
        };
        var reservation = await reservationRepo!.AddAsync(mockReservation, true);

        var appDateRepo = Factory.GetRequiredService<IAppDateRepository>();
        var mockDate = new AppDate
        {
            DateTime = DateTime.UtcNow,
            Code = EntityUtil.CreateCode()
        };
        var appDate = await appDateRepo!.AddAsync(mockDate, true);

        var personAgeTypeRepo = Factory.GetRequiredService<IPersonAgeTypeRepository>();
        var mockPersonAgeType = new PersonAgeType
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Code = EntityUtil.CreateCode(),
            AgeMax = 10,
            AgeMin = 1
        };
        var personAgeType = await personAgeTypeRepo!.AddAsync(mockPersonAgeType, true);

        var reservationRoomAppDatePersonRepo =
            Factory.GetRequiredService<IReservationRoomGroupAppDatePersonAgeTypeRepository>();
        var mockReservationRoomAppDatePerson = new ReservationRoomGroupAppDatePersonAgeType
        {
            ReservationId = reservation.Id,
            RoomGroupId = roomGroup.Id,
            BookingDateId = appDate.Id,
            RestIndex = 0,
            RoomGroupIndex = 0,
            MaleNumber = 0,
            FemaleNumber = 1,
            GenderNoneNumber = 0,
            UnitPrice = 10,
            PersonAgeTypeId = personAgeType.Id
        };

        _ = await reservationRoomAppDatePersonRepo!.AddAsync(mockReservationRoomAppDatePerson, true);
        await CreateSystemConfigAsync();
        var guestCode = GetGuestCode(reservation);

        var bookingConfirmRequest = new BookingCancellationRequest { Id = reservation.Id };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{guestCode}/cancellation",
            TestUtil.ToJsonContent(bookingConfirmRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    private async Task CreateSystemConfigAsync()
    {
        var systemConfigRepo = Factory.GetRequiredService<ISystemConfigRepository>()
            ?? throw new ArgumentException(nameof(ISystemConfigRepository));
        var systemConfig = new SystemConfig
        {
            Code = "test",
            IsEnabled = true,
            CanOnlinePayment = true,
            TemplateFormatData = null
        };
        var systemConfigEntityAdd = await systemConfigRepo.AddAsync(systemConfig, true);
        systemConfigEntityAdd.TemplateFormatData = null;
        await systemConfigRepo.UpdateAsync(systemConfigEntityAdd, true);
    }
}
