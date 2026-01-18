using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class ReservationDetailEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/reservations";
    private MockBookingData MockBookingData { get; }

    public ReservationDetailEndpointIntTest()
    {
        MockBookingData = new MockBookingData(Factory, Client, FacilityInfo);
    }

    [Fact]
    public async Task GetReservationDetails_ReturnOK_WithReservationResponse()
    {
        var plan = await MockBookingData.CreatePlanAsync();
        var roomGroup = await MockBookingData.CreateRoomGroupAsync();
        var site = await MockBookingData.CreateSiteAsync();
        var cancellationData = await CreateCancellationDataAsync();
        await CreateDataOfCancellationDataAsync(plan.Cancellation, cancellationData);

        var planRoomGroupRepo = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var mockPlanRoom = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = roomGroup.Id
        };
        _ = await planRoomGroupRepo!.AddAsync(mockPlanRoom, true);

        var reservationRepo = Factory.GetRequiredService<IReservationRepository>();
        var mockReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            Code = EntityUtil.CreateCode(),
            Facility = FacilityInfo,
            Plan = plan,
            RoomGroup = roomGroup,
            Site = site,
            MainUser = new CustomerInfo { Code = EntityUtil.CreateCode() },
            Reserver = new CustomerInfo { Code = EntityUtil.CreateCode() },
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = 20240909,
            RestNumber = 1,
            RoomNumber = 1,
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

        var response = await Client.GetAsync($"{BaseUrl}/{reservation.Id}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    private async Task<CancellationData> CreateCancellationDataAsync()
    {
        var destinationRepo = Factory.GetRequiredService<ICancellationDataRepository>()
            ?? throw new ArgumentException(nameof(ICancellationDataRepository));

        var destinationsToCreate =
            new CancellationData
            {
                Code = "1",
                DayEnd = 20251203,
                DayStart = 20250103,
                Description = null,
                Rate = 6,
                IsEnabled = true
            };

        var createdDestinations = await destinationRepo.AddAsync(
            destinationsToCreate,
            true
        );

        return createdDestinations;
    }

    private async Task CreateDataOfCancellationDataAsync(
        Cancellation? cancellation,
        CancellationData cancellationData
    )
    {
        var destinationService = Factory.GetRequiredService<IDataOfCancellationRepository>()
            ?? throw new ArgumentException(nameof(IDataOfCancellationRepository));
        var destinationsToCreate = new List<CancellationCancellationData>
        {
            new()
            {
                CancellationData = cancellationData,
                Cancellation = cancellation,
                CancellationId = cancellation?.Id ?? 1,
                CancellationDataId = cancellationData.Id,
                IsEnabled = true
            }
        };
        await destinationService.AddRangeAsync(destinationsToCreate, true);
    }
}
