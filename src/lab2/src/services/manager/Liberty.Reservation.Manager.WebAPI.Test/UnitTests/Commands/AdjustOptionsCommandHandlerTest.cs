using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class AdjustOptionsCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task Handle_ShouldReturnBookingPriceResponse_WhenOptionsAreValid()
    {
        // Arrange
        var facilityId = 123L;
        var reservationId = 10L;
        var dummyPlan = new Plan();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

        var dummyReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            FacilityId = facilityId,
            SiteId = 200,
            RoomGroupId = 300,
            Plan = dummyPlan,
            CheckInDate = AppDate.GetId(DateTime.Now.AddMonths(3)),
            ReservationState = ReservationStatus.Reserved
        };

        var dummyBookingPriceResponse = new BookingPriceResponse();

        var checkInDate = AppDate.GetId(DateTime.UtcNow.AddMonths(3));

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
        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };

        var command = new AdjustOptionsCommand(reservationId) { Payload = payload };

        var mockPlan = new BookingPlanModel(
            1,
            true,
            false,
            null,
            null,
            null,
            2,
            true,
            new TimeSpan(15, 0, 0),
            new TimeSpan(21, 0, 0),
            null,
            true,
            false,
            0,
            null,
            false,
            null,
            null,
            true,
            20250612,
            20251229,
            null,
            null,
            new TimeSpan(10, 0, 0),
            false,
            20250624,
            20251229,
            new MultilingualText
            {
                { "jp", "Name Jp" },
                { "en", "Name En" }
            },
            new MultilingualText
            {
                { "jp", "Description Jp" },
                { "en", "Description En" }
            },
            new MultilingualText
            {
                { "jp", "Tag Jp" },
                { "en", "Tag En" }
            },
            new MultilingualText
            {
                { "jp", "Summary Jp" },
                { "en", "Summary En" }
            },
            PlanTypes.Combo,
            false,
            1,
            false,
            false,
            [],
            [],
            [],
            [],
            false,
            null,
            null,
            new BookingCancellationPolicyModel(),
            false,
            1,
            TimeSpan.Zero
        );

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockBookingDataAvailableService = new Mock<IBookingDataAvailableService>();
        var mockBookingPriceOfPlanService = new Mock<IBookingPriceService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockBookingDataAvailableService.Setup(
                s => s.GetReservationByFacilityIdAsync(
                    reservationId,
                    facilityId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyReservation);

        bookingCheckAvailableServiceMock.Setup(
                s => s.IsInvalidOptionItemsAsync(
                    payload,
                    dummyReservation.FacilityId,
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        mockBookingPriceOfPlanService.Setup(
                s => s.GetBookingPrice(
                    payload,
                    It.IsAny<BookingPlanModel>(),
                    dummyReservation.RoomGroupId
                )
            )
            .Returns(dummyBookingPriceResponse);

        mockMapper.SetupGet(m => m.ConfigurationProvider).Returns(new MapperConfiguration(_ => { }));

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockBookingDataAvailableService.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingSearchServiceMock.Object,
            mockBookingPriceOfPlanService.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_ShouldThrowReservationInvalidException_WhenOptionItemsAreInvalid()
    {
        // Arrange
        var facilityId = 123L;
        var reservationId = 10L;
        var dummyPlan = new Plan();

        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

        var dummyReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            FacilityId = facilityId,
            SiteId = 200,
            RoomGroupId = 300,
            Plan = dummyPlan,
            CheckInDate = AppDate.GetId(DateTime.Now.AddMonths(3)),
            ReservationState = ReservationStatus.Temporary
        };
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();

        var dummyBookingPriceResponse = new BookingPriceResponse();

        var checkInDate = AppDate.GetId(DateTime.UtcNow.AddMonths(3));

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
        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };

        var command = new AdjustOptionsCommand(reservationId) { Payload = payload };

        var mockPlan = new BookingPlanModel(
            1,
            true,
            false,
            null,
            null,
            null,
            2,
            true,
            new TimeSpan(15, 0, 0),
            new TimeSpan(21, 0, 0),
            null,
            true,
            false,
            0,
            null,
            false,
            null,
            null,
            true,
            20250612,
            20251229,
            null,
            null,
            new TimeSpan(10, 0, 0),
            false,
            20250624,
            20251229,
            new MultilingualText
            {
                { "jp", "Name Jp" },
                { "en", "Name En" }
            },
            new MultilingualText
            {
                { "jp", "Description Jp" },
                { "en", "Description En" }
            },
            new MultilingualText
            {
                { "jp", "Tag Jp" },
                { "en", "Tag En" }
            },
            new MultilingualText
            {
                { "jp", "Summary Jp" },
                { "en", "Summary En" }
            },
            PlanTypes.Combo,
            false,
            1,
            false,
            false,
            [],
            [],
            [],
            [],
            false,
            null,
            null,
            new BookingCancellationPolicyModel(),
            false,
            1,
            TimeSpan.Zero
        );

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockBookingDataAvailableService = new Mock<IBookingDataAvailableService>();
        var mockBookingPriceOfPlanService = new Mock<IBookingPriceService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockBookingDataAvailableService.Setup(
                s => s.GetReservationByFacilityIdAsync(
                    reservationId,
                    facilityId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyReservation);

        bookingCheckAvailableServiceMock.Setup(
                s => s.IsInvalidOptionItemsAsync(
                    payload,
                    dummyReservation.FacilityId,
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        mockBookingPriceOfPlanService.Setup(
                s => s.GetBookingPrice(
                    payload,
                    It.IsAny<BookingPlanModel>(),
                    dummyReservation.RoomGroupId
                )
            )
            .Returns(dummyBookingPriceResponse);

        mockMapper
            .SetupGet(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(_ => { }));

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockBookingDataAvailableService.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingSearchServiceMock.Object,
            mockBookingPriceOfPlanService.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<ReservationOptionException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }
}
