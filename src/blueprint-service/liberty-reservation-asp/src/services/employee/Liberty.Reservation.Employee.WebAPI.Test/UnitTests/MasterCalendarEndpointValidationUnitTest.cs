using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MasterCalendar;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class MasterCalendarEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task MasterCalendarDeleteDataCommand_ShouldReturnValidationError_WhenAppDateIsNegative()
    {
        // Arrange
        var command = new MasterCalendarDeleteDataCommand { Payload = new MasterCalendarDeleteDataRequest(-01012025) };

        // Act
        var validation = await new MasterCalendarDeleteDataCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarDeleteDataCommand_ShouldReturnValidationError_WhenAppDateIsInvalid()
    {
        // Arrange
        var command = new MasterCalendarDeleteDataCommand { Payload = new MasterCalendarDeleteDataRequest(1) };

        // Act
        var validation = await new MasterCalendarDeleteDataCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarDeleteTypeCommand_ShouldReturnValidationError_WhenAppDateIsNegative()
    {
        // Arrange
        var command = new MasterCalendarDeleteTypeCommand { Payload = new MasterCalendarDeleteTypeRequest(-01012025) };

        // Act
        var validation = await new MasterCalendarDeleteTypeCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarDeleteTypeCommand_ShouldReturnValidationError_WhenAppDateIsInvalid()
    {
        // Arrange
        var command = new MasterCalendarDeleteTypeCommand { Payload = new MasterCalendarDeleteTypeRequest(1) };

        // Act
        var validation = await new MasterCalendarDeleteTypeCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarGetAllQuery_ShouldReturnValidationError_WhenStartDateIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new MasterCalendarGetAllQuery(pageable, 1, 20250101);

        // Act
        var validation = await new MasterCalendarGetAllQueryValidator().ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("StartDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarGetAllQuery_ShouldReturnValidationError_WhenEndDateIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new MasterCalendarGetAllQuery(pageable, 20250101, 2);

        // Act
        var validation = await new MasterCalendarGetAllQueryValidator().ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("EndDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarGetAllQuery_ShouldReturnValidationError_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new MasterCalendarGetAllQuery(pageable, 20250101, 20240101);

        // Act
        var validation = await new MasterCalendarGetAllQueryValidator().ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Equal("EndDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateDataCommand_ShouldReturnValidationError_WhenAppDateIsInvalid()
    {
        // Arrange
        var command = new MasterCalendarUpdateDataCommand { Payload = new MasterCalendarEditDataRequest(0101, string.Empty) };

        // Act
        var validation = await new MasterCalendarUpdateDataCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateDataCommand_ShouldReturnValidationError_WhenAppDateIsNegative()
    {
        // Arrange
        var command = new MasterCalendarUpdateDataCommand { Payload = new MasterCalendarEditDataRequest(-1, string.Empty) };

        // Act
        var validation = await new MasterCalendarUpdateDataCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateDataCommand_ShouldReturnValidationError_WhenNameIsTooLong()
    {
        // Arrange
        var command = new MasterCalendarUpdateDataCommand { Payload = new MasterCalendarEditDataRequest(20250101, new string('a', 251)) };

        // Act
        var validation = await new MasterCalendarUpdateDataCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateTypeCommand_ShouldReturnValidationError_WhenAppDateIsNegative()
    {
        // Arrange
        var command = new MasterCalendarUpdateTypeCommand { Payload = new MasterCalendarEditTypeRequest(-20250101, 1) };

        // Act
        var validation = await new MasterCalendarUpdateTypeCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateTypeCommand_ShouldReturnValidationError_WhenAppDateIsInvalidDate()
    {
        // Arrange
        var command = new MasterCalendarUpdateTypeCommand { Payload = new MasterCalendarEditTypeRequest(1, 1) };

        // Act
        var validation = await new MasterCalendarUpdateTypeCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("AppDate", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateTypeCommand_ShouldReturnValidationError_WhenAppDateTypeIdIsNegative()
    {
        // Arrange
        var command = new MasterCalendarUpdateTypeCommand { Payload = new MasterCalendarEditTypeRequest(20250101, -1) };

        // Act
        var validation = await new MasterCalendarUpdateTypeCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("AppDateTypeId", validation.GetErrorField());
    }

    [Fact]
    public async Task MasterCalendarUpdateTypeCommand_ShouldReturnValidationError_WhenAppDateTypeIdIsZeroOrLess()
    {
        // Arrange
        var command = new MasterCalendarUpdateTypeCommand { Payload = new MasterCalendarEditTypeRequest(20250101, 0) };

        // Act
        var validation = await new MasterCalendarUpdateTypeCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("AppDateTypeId", validation.GetErrorField());
    }
}
