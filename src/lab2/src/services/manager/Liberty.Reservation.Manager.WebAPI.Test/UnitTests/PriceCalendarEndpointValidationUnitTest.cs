using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceCalendar;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceCalendar;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PriceCalendarEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task PriceCalendarCreateCommandValidator_ShouldThrowError_WhenPriceTypeIdIsNullOrLessThanOrEqualToZero()
    {
        // Arrange
        var request = new PriceCalendarCreateRequest(
            [
                new FacilityCalendarCreateRequest(0, 1, false)
            ]
        );

        var command = new PriceCalendarCreateCommand { Payload = request };

        var validator = new PriceCalendarCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("Calendars[0].PriceTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceCalendarCreateCommandValidator_ShouldThrowError_WhenDateCalendarIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new PriceCalendarCreateRequest(
            [
                new FacilityCalendarCreateRequest(1, 0, false)
            ]
        );

        var command = new PriceCalendarCreateCommand { Payload = request };

        var validator = new PriceCalendarCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("Calendars[0].DateCalendar", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceCalendarCreateCommandValidator_ShouldThrowError_WhenDateCalendarIsInvalidDate()
    {
        // Arrange
        var request = new PriceCalendarCreateRequest(
            [
                new FacilityCalendarCreateRequest(1, 1, false)
            ]
        );

        var command = new PriceCalendarCreateCommand { Payload = request };

        var validator = new PriceCalendarCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("Calendars[0].DateCalendar", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceCalendarGetAllQueryValidator_ShouldThrowError_WhenStartDateIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new PriceCalendarGetAllQuery(-1, AppDate.GetId(DateTime.UtcNow), pageable);

        var validator = new PriceCalendarGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("StartDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceCalendarGetAllQueryValidator_ShouldThrowError_WhenEndDateIsLessThanStartDate()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new PriceCalendarGetAllQuery(
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow.AddDays(-1)),
            pageable
        );

        var validator = new PriceCalendarGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceCalendarGetAllQueryValidator_ShouldThrowError_WhenEndDateIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new PriceCalendarGetAllQuery(
            AppDate.GetId(DateTime.UtcNow),
            99999999999,
            pageable
        );

        var validator = new PriceCalendarGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("EndDate", validation.Errors.GetErrorField());
    }
}
