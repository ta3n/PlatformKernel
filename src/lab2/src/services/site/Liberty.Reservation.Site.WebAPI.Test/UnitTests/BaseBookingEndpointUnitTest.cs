using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests;

public abstract class BaseBookingEndpointUnitTest : BaseUnitTest
{
    protected static string? SecretTest => Environment.GetEnvironmentVariable("SECRET_TEST");

    protected IPlanRoomGroupService MockPlanService { get; set; } = null!;
    protected ISecurityContextAccessor MockSecurityContextAccessor { get; set; } = null!;

    protected IServiceProvider MockServiceProvider { get; set; } = null!;

    protected override void InitData()
    {
        var mockPlanService = new Mock<IPlanRoomGroupService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var mockMediator = new Mock<IMediator>();
        var planOfBookingResponse = new BookingSearchByPlanResponse(
            1,
            "Plan A",
            "TagA",
            true,
            false,
            "This is Plan A",
            false,
            null,
            null,
            false,
            null,
            null,
            PlanTypes.Combo,
            1,
            "des",
            false
        )
        {
            BasePrice = 100,
            Categories = [new(1, "Category 1")],
            Files = [new("FileCode1", "application/pdf", 0, true)],
            Meals = [new(1, MealTypeEatTypes.Box, "Meal 1")],
            Rooms =
            [
                new(
                    1,
                    "Room 1",
                    "Tag1",
                    false,
                    "des",
                    1
                )
                {
                    Files = [new("RoomFileCode1", "image/jpeg", 0, true)],
                    AppDatePrices =
                    [
                        new(1, 5)
                        {
                            BasePrice = 50,
                            Price = 60,
                            TotalSpaTax = 5
                        }
                    ]
                }
            ]
        };

        var bookingDetailsResponse = new BookingDetailsResponse
        {
            Id = 1,
            Code = "B001",
            Name = "Test Booking",
            Tag = "TestTag",
            Summary = "Test summary",
            Description = "Test description",
            Payment = "Online",
            Meal = "Meal Type 1",
            Other = "Other Info",
            CheckInStart = TimeSpan.FromHours(14),
            CheckInEnd = TimeSpan.FromHours(16),
            CheckOut = TimeSpan.FromHours(12),
            IsOnLinePayment = true,
            IsOnSidePayment = false,
            SiteId = 123,
            Categories =
            [
                "Category 1",
                "Category 2"
            ],
            LastUpdateString = "2025-01-08"
        };

        var priceCalendarResponse = new PriceCalendarOfRoomResponse
        {
            Price = 100,
            TotalPrice = 120,
            TotalSpaTax = 20,
            AppDatePrices =
            [
                new(
                    1,
                    20
                )
            ]
        };

        var appDatePriceResponse = new AppDatePriceOfBookingResponse
        {
            AppDateId = 1,
            Price = 100,
            TotalOptionPrice = 50,
            TotalSpaTax = 20,
            Rooms =
            [
                new(1, 1)
                {
                    Price = 100,
                    TotalSpaTax = 20,
                    TotalOptionPrice = 50,
                    Peoples =
                    [
                        new(
                            100,
                            20,
                            20,
                            200,
                            2,
                            1,
                            0,
                            3,
                            0,
                            1,
                            "Person A"
                        )
                    ],
                    Options =
                    [
                        new(
                            1,
                            "Option A",
                            50,
                            2
                        )
                    ]
                }
            ]
        };

        var bookingPriceResponse = new BookingPriceResponse
        {
            AppDatePrices = new List<AppDatePriceOfBookingResponse> { appDatePriceResponse }
        };

        var mockedAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                Price = 100,
                TotalSpaTax = 20,
                TotalOptionPrice = 10
            },
            new()
            {
                Price = 150,
                TotalSpaTax = 30m,
                TotalOptionPrice = 15
            }
        };

        var mockedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockedAppDatePrices };

        var mockResponse = new BookingPriceResponse
        {
            AppDatePrices = new List<AppDatePriceOfBookingResponse>
            {
                new()
                {
                    AppDateId = 123,
                    Price = 100m,
                    TotalOptionPrice = 50m,
                    TotalSpaTax = 20m,
                    Rooms =
                    [
                        new(123, 1)
                        {
                            Price = 100m,
                            TotalSpaTax = 20m,
                            TotalOptionPrice = 50m
                        }
                    ]
                }
            }
        };

        var mockOptionItemOfBookingResponse = new List<OptionItemOfBookingResponse>
        {
            new(
                1,
                "Option 1",
                "Description for option 1",
                100
            )
        };

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingGetAllOptionItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), mockOptionItemOfBookingResponse));

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingCheckRoomNumberCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        mockMediator.Setup(mediator => mediator.Send(It.IsAny<CheckChangedCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        mockMediator.Setup(mediator => mediator.Send(It.IsAny<AdjustOptionsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedBookingPriceResponse);

        mockMediator.Setup(mediator => mediator.Send(It.IsAny<ChangePersonsBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedBookingPriceResponse);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingCheckNightNumberCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookingPriceResponse);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingGetCalendarPricesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), priceCalendarResponse));

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingGetDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), bookingDetailsResponse));

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingSearchByPlanQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), new List<BookingSearchByPlanResponse> { planOfBookingResponse }));

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingCreateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Code");

        mockPlanService
            .Setup(
                service => service.GetPlanWithRoomOnlyTypeAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Plan
                {
                    Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
                    Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
                }
            );

        MockMediator = mockMediator.Object;
        MockPlanService = mockPlanService.Object;
        MockSecurityContextAccessor = mockSecurityContextAccessor.Object;
        MockServiceProvider = mockServiceProvider.Object;
    }
}
