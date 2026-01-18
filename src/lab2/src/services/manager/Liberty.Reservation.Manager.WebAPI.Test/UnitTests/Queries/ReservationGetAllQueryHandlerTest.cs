using Moq;
using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using MockQueryable;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class ReservationGetAllQueryHandlerTest : BaseUnitTest
{
    protected override void InitData()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<ReservationEntity, BookingReservationResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.Code,
                        opt => opt.MapFrom(
                            src => src.Code
                        )
                    )
                    .ForMember(
                        dest => dest.State,
                        opt => opt.MapFrom(
                            src => src.ReservationState.ToString()
                        )
                    )
                    .ForMember(
                        dest => dest.IsReserved,
                        opt => opt.MapFrom(
                            src => src.IsReserved
                        )
                    )
                    .ForMember(
                        dest => dest.BookingDateTime,
                        opt => opt.MapFrom(
                            src => src.ReservationDateTime
                        )
                    )
                    .ForMember(
                        dest => dest.ConfirmDateTime,
                        opt => opt.MapFrom(
                            src => src.ConfirmedDateTime
                        )
                    )
                    .ForMember(
                        dest => dest.CancelledDateTime,
                        opt => opt.MapFrom(
                            src => src.CancelledDateTime
                        )
                    )
                    .ForMember(
                        dest => dest.CheckInDate,
                        opt => opt.MapFrom(
                            src => src.CheckInDate
                        )
                    )
                    .ForMember(
                        dest => dest.LengthOfStay,
                        opt => opt.MapFrom(
                            src => src.RestNumber
                        )
                    )
                    .ForMember(
                        dest => dest.NumberOfRooms,
                        opt => opt.MapFrom(
                            src => src.RoomNumber
                        )
                    )
                    .ForMember(
                        dest => dest.ReserverName,
                        opt => opt.MapFrom(
                            src => src.Reserver!.Name
                        )
                    )
                    .ForMember(
                        dest => dest.PaymentType,
                        opt => opt.MapFrom(
                            src => src.PaymentType.ToString()
                        )
                    )
                    .ForMember(
                        dest => dest.IsOnlinePayment,
                        opt => opt.MapFrom(
                            src => src.IsOnlinePayment
                        )
                    )
                    .ForMember(
                        dest => dest.IsNoShow,
                        opt => opt.MapFrom(
                            src => src.IsNoShow
                        )
                    )
                    .ForMember(
                        dest => dest.NoShowReason,
                        opt => opt.MapFrom(
                            src => src.NoShowReason
                        )
                    )
                    .ForMember(
                        dest => dest.NoShowDateTime,
                        opt => opt.MapFrom(
                            src => src.NoShowDateTime
                        )
                    )
                    .ForMember(
                        dest => dest.DayUse,
                        opt => opt.MapFrom(
                            src => src.BookingData!.Plan.DayUse
                        )
                    )
                    .ForMember(
                        dest => dest.UpdateCount,
                        opt => opt.MapFrom(
                            src => src.UpdateCount
                        )
                    )
                    .ForMember(
                        dest => dest.UpdatedAt,
                        opt => opt.MapFrom(
                            src =>
                                src.ModifiedDateTime.HasValue ? src.ModifiedDateTime.Value.ToUniversalTime() : (DateTime?)null
                        )
                    );
            }
        );

        var mapper = new Mapper(config);
        MockMapper = mapper;
    }

    [Fact]
    public async Task ShouldReturnPaginatedReservationResponse_WhenReservationsExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var cacheServiceMock = new Mock<ICacheService>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();

        var mockFacility = new Facility
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Facility" } },
            CanOnLinePayment = true,
            IsOnLinePayment = true
        };

        var mockPlan = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test Plan" } },
            IsOnLinePayment = true,
            PlanMealTypes = new List<PlanMealType> { new() { MealType = new MealType { Name = "Breakfast" } } }
        };

        var mockRoomGroup = new RoomGroup
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room Group 1" } },
            IsEnabledSmoking = true
        };

        var mockSite = new Site { Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } } };

        var mockBookingData = new BookingData
        {
            Facility = new FacilityData
            {
                Id = 1,
                Name = "Test Facility",
                Address1 = "123 Main St.",
                Address2 = "Suite 400",
                Address3 = "City Center",
                Address4 = "Cityville, State",
                CanOnLinePayment = true,
                IsOnSidePayment = false,
                IsOnLinePayment = true
            },
            Plan = new PlanData
            {
                Id = 1,
                Name = "Test Plan",
                IsOnSidePayment = true,
                IsOnLinePayment = false,
                Meta = new PlanMeta(),
                Files =
                [
                    new()
                    {
                        Code = "File1",
                        ContentType = "application/pdf"
                    }
                ],
                CancelDayLimit = null,
                IsCancelSameAccept = false,
                CancelLimit = null,
                Meals = [],
                ReceptionDayLimit = 1,
                CancellationDataPolicy = new BookingCancellationPolicyModel()
            },
            RoomGroup = new RoomGroupData
            {
                Id = 1,
                Name = "Deluxe Room",
                IsEnabledSmoking = false,
                CapacityMax = 4,
                CapacityMin = 1,
                Files =
                [
                    new()
                    {
                        Code = "RoomFile1",
                        ContentType = "image/jpeg"
                    }
                ]
            },
            Site = new SiteData
            {
                Id = 1,
                Name = "Test Site"
            },
            AppDates =
            [
                new()
                {
                    AppDateId = 1,
                    RestIndex = 1,
                    Price = 100,
                    SpaTax = 10,
                    OptionPrice = 20,
                    Rooms = new List<BookingRoomDataOfAppDate>
                    {
                        new()
                        {
                            RoomIndex = 1,
                            RoomPrice = 100,
                            PricePeoples = new List<PeoplePriceDataOfRoom>
                            {
                                new()
                                {
                                    RoomPrice = 100,
                                    SpaTax = 10,
                                    Persons = 2,
                                    MalePersons = 1,
                                    FemalePersons = 1,
                                    PersonAgeType = new PeopleDataOfRoom
                                    {
                                        Id = 1,
                                        Name = "Adult"
                                    }
                                }
                            },
                            OptionItems = new List<OptionItemDataOfRoom>
                            {
                                new()
                                {
                                    Id = 1,
                                    Name = "Extra Bed",
                                    Price = 50,
                                    Number = 1
                                }
                            },
                            CustomerInfo = new CustomerData
                            {
                                Name = "John Doe",
                                Kana = "ジョン・ドウ"
                            },
                            SpaTax = 10,
                            TotalOptionPrice = 50,
                            Persons = 2
                        }
                    }
                }
            ],
            PersonAgeTypes =
            [
                new()
                {
                    Id = 1,
                    Name = "Adult",
                    IsMain = true,
                    AgeMin = 18,
                    AgeMax = 65
                }
            ],
            SendMailState = new SendMailState
            {
                BookingReminderOfUpcomingCheckInDateSend = true,
                BookingReminderOfUpcomingCheckInDateSent = false,
                BookingCancellationFeeReminderSend = false,
                BookingCancellationFeeReminderSent = false
            },
            TotalRoomPrice = 100,
            TotalSpaTax = 10,
            TotalOptionPrice = 20,
            UsedPoint = 10,
            IsSiteLocation = true
        };
        var userCode = "test-user-code";
        var query = new ReservationGetAllQuery(20250101, 20250202, PageableBinderConfig.DefaultPageable);

        var reservationEntities = new[]
            {
                new ReservationEntity
                {
                    Id = 1,
                    Code = "RES123",
                    ReservationState = ReservationStatus.Confirmed,
                    Facility = mockFacility,
                    Plan = mockPlan,
                    RoomGroup = mockRoomGroup,
                    Site = mockSite,
                    CheckInDate = AppDate.GetId(DateTime.Now),
                    ReservationDateTime = DateTime.Now,
                    RoomNumber = 2,
                    RestNumber = 3,
                    Memo = "Test Memo",
                    PaymentType = PaymentTypes.OnLinePayment,
                    BookingData = mockBookingData
                }
            }.AsQueryable()
            .BuildMock();

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(reservationEntities);

        var handler = new ReservationGetAllQueryHandler(
            MockMapper,
            cacheServiceMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object
        );

        // Act
        var (headers, _) = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(headers);
    }
}
