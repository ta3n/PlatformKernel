using Moq;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using AutoMapper;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.User.Application.Auth;
using MockQueryable;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Queries;

public class ReservationGetAllQueryByFilterHandlerTest : BaseUnitTest
{
    protected override void InitData()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<ReservationEntity, ReservationResponse>()
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
                            src => src.ReservationState
                        )
                    )
                    .ForMember(
                        dest => dest.CanOnLinePayment,
                        opt => opt.MapFrom(
                            src => src.Facility!.CanOnLinePayment
                                && src.Facility!.IsOnLinePayment
                                && src.Plan!.IsOnLinePayment
                                && src.PaymentType == PaymentTypes.OnLinePayment
                                && src.ReservationState == ReservationStatus.Temporary
                        )
                    )
                    .ForMember(
                        dest => dest.RoomGroupName,
                        opt => opt.MapFrom(
                            src => src.RoomGroup!.Name
                        )
                    )
                    .ForMember(
                        dest => dest.PlanName,
                        opt => opt.MapFrom(
                            src => src.Plan!.Name
                        )
                    )
                    .ForMember(
                        dest => dest.SiteName,
                        opt => opt.MapFrom(
                            src => src.Site!.Name
                        )
                    )
                    .ForMember(
                        dest => dest.CheckInDate,
                        opt => opt.MapFrom(
                            src => AppDate.GetDateTime(src.CheckInDate, null)
                        )
                    )
                    .ForMember(
                        dest => dest.ReservationDateTime,
                        opt => opt.MapFrom(
                            src => src.ReservationDateTime
                        )
                    )
                    .ForMember(
                        dest => dest.IsEnabledSmoking,
                        opt => opt.MapFrom(
                            src => src.RoomGroup!.IsEnabledSmoking
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
                        dest => dest.PaymentType,
                        opt => opt.MapFrom(
                            src => src.PaymentType.ToString()
                        )
                    )
                    .ForMember(
                        dest => dest.Memo,
                        opt => opt.MapFrom(
                            src => src.Memo
                        )
                    )
                    .ForMember(
                        dest => dest.Facilityname,
                        opt => opt.MapFrom(
                            src => src.BookingData!.Facility.Name
                        )
                    )
                    .ForMember(
                        dest => dest.TotalPrice,
                        opt => opt.MapFrom(
                            src => src.BookingData!.TotalPrice
                        )
                    )
                    .ForMember(
                        dest => dest.CheckInTime,
                        opt => opt.MapFrom(
                            src => src.CheckInTime
                        )
                    )
                    .ForMember(
                        dest => dest.ReceptionDayLimit,
                        opt => opt.MapFrom(
                            src => src.Plan!.ReceptionDayLimit
                        )
                    )
                    .ForMember(
                        dest => dest.ReceptionLimit,
                        opt => opt.MapFrom(
                            src => src.Plan!.ReceptionLimit
                        )
                    )
                    .ForMember(
                        dest => dest.IsCancelSameAccept,
                        opt => opt.MapFrom(
                            src => src.Plan!.IsCancelSameAccept
                        )
                    )
                    .ForMember(
                        dest => dest.CancelDayLimit,
                        opt => opt.MapFrom(
                            src => src.Plan!.CancelDayLimit
                        )
                    )
                    .ForMember(
                        dest => dest.CancelLimit,
                        opt => opt.MapFrom(
                            src => src.Plan!.CancelLimit
                        )
                    )
                    .ForMember(
                        dest => dest.CheckOutTime,
                        opt => opt.MapFrom(
                            src => src.Plan!.CheckOut
                        )
                    )
                    .ForMember(
                        dest => dest.RestNumber,
                        opt => opt.MapFrom(
                            src => src.RestNumber
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
            PlanMealTypes = [new() { MealType = new MealType { Name = "Breakfast" } }]
        };

        var mockRoomGroup = new RoomGroup
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test room" } },
            IsEnabledSmoking = true
        };

        var mockSite = new Reservation.Application.Contexts.DataContexts.Entities.Data.Site
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } }
        };

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
                ]
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
                    Rooms =
                    [
                        new()
                        {
                            RoomIndex = 1,
                            RoomPrice = 100,
                            PricePeoples =
                            [
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
                            ],
                            OptionItems =
                            [
                                new()
                                {
                                    Id = 1,
                                    Name = "Extra Bed",
                                    Price = 50,
                                    Number = 1
                                }
                            ],
                            CustomerInfo = new CustomerData
                            {
                                Name = "John Doe",
                                Kana = "ジョン・ドウ"
                            },
                            SpaTax = 10,
                            TotalOptionPrice = 50,
                            Persons = 2
                        }
                    ]
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
        var query = new ReservationGetAllQuery(PageableBinderConfig.DefaultPageable);

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

        var handler = new ReservationGetAllQueryHandler(MockMapper, securityContextAccessorMock.Object, reservationRepositoryMock.Object);

        // Act
        var (headers, _) = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(headers);
    }
}
