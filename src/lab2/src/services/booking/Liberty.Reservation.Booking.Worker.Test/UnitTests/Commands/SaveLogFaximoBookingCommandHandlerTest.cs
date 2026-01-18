using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingAggregateFaxAuditLog;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Booking.Worker.Test.UnitTests.Commands;

public class SaveLogFaximoBookingCommandHandlerTest
{
    [Fact]
    public async Task HandlerAsync_ValidRequest_SaveLogFaximoBooking()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var faxAuditLogServiceMock = new Mock<IBookingAggregateFaxAuditLogService>();

        var faxAuditLogMock = new BookingAggregateFaxAuditLog();

        mapperMock
            .Setup(m => m.Map<BookingAggregateFaxAuditLog>(It.IsAny<SaveBookingAggregateFaxAuditLogRequest>()))
            .Returns(faxAuditLogMock);

        var cancellationToken = CancellationToken.None;

        var payload = new SaveBookingAggregateFaxAuditLogRequest(
            "faxNumber_123",
            "subject_123",
            "subject_123",
            true,
            "reservationCode_123",
            "body test"
        );

        var command = new SaveBookingAggregateFaxAuditLogCommand { Payload = payload };

        var commandHandler = new SaveBookingAggregateFaxAuditLogCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            faxAuditLogServiceMock.Object
        );

        var result = await commandHandler.Handle(command, cancellationToken);

        Assert.True(result);
    }
}
