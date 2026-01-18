using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PriceTypesEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task PriceTypeCreateCommandValidator_ShouldThrowError_WhenShortNameIsEmpty()
    {
        // Arrange
        var request = new PriceTypeCreateRequest(string.Empty, "Name", "#FFFFFF");
        var command = new PriceTypeCreateCommand { Payload = request };
        var validator = new PriceTypeCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("ShortName", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeCreateCommandValidator_ShouldThrowError_WhenShortNameIsTooLong()
    {
        // Arrange
        var request = new PriceTypeCreateRequest(new string('A', 251), "Name", "#FFFFFF");
        var command = new PriceTypeCreateCommand { Payload = request };
        var validator = new PriceTypeCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("ShortName", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeCreateCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var request = new PriceTypeCreateRequest("ShortName", string.Empty, "#FFFFFF");
        var command = new PriceTypeCreateCommand { Payload = request };
        var validator = new PriceTypeCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeCreateCommandValidator_ShouldThrowError_WhenColorIsEmpty()
    {
        // Arrange
        var request = new PriceTypeCreateRequest("Name", "ShortName", string.Empty);
        var command = new PriceTypeCreateCommand { Payload = request };
        var validator = new PriceTypeCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Color", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeCreateCommandValidator_ShouldThrowError_WhenColorIsInvalid()
    {
        // Arrange
        var request = new PriceTypeCreateRequest("Name", "ShortName", "InvalidColor");
        var command = new PriceTypeCreateCommand { Payload = request };
        var validator = new PriceTypeCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0013, validation.Errors.GetErrorCode());
        Assert.Contains("Color", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeGetQueryValidator_ShouldThrowError_WhenIdIsZero()
    {
        // Arrange
        var query = new PriceTypeGetQuery(0);
        var validator = new PriceTypeGetQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest("Name", "ShortName", "Red") { Id = null };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenIdIsZeroOrNegative()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest("Name", "ShortName", "Red") { Id = -1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest("ShortName", string.Empty, "Red") { Id = 1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest("ShortName", new string('a', 251), "Red") { Id = 1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenShortNameIsNullOrEmpty()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest(string.Empty, "Name", "Red") { Id = 1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("ShortName", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenShortNameExceedsMaxLength()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest(new string('b', 51), "Name", "Red") { Id = 1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("ShortName", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenColorIsNullOrEmpty()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest("Name", "ShortName", string.Empty) { Id = 1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Color", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PriceTypeUpdateCommandValidator_ShouldThrowError_WhenColorIsInvalid()
    {
        // Arrange
        var request = new PriceTypeUpdateRequest("Name", "ShortName", "InvalidColor") { Id = 1 };
        var command = new PriceTypeUpdateCommand { Payload = request };
        var validator = new PriceTypeUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0013, validation.Errors.GetErrorCode());
        Assert.Contains("Color", validation.Errors.GetErrorField());
    }
}
