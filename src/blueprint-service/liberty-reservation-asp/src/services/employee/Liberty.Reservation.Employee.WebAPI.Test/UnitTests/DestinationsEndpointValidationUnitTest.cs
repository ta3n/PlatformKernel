using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class DestinationsEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task CreateAppDateType_ShouldReturnValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var appDateTypeCreateRequest = new AppDateTypeCreateRequest(
            string.Empty, // Invalid Name
            "AppShortName",
            "Red",
            "Description"
        );

        // Act
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(
            appDateTypeCreateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CreateAppDateType_ShouldReturnValidationError_WhenShortNameExceedsMaxLength()
    {
        // Arrange
        var appDateTypeCreateRequest = new AppDateTypeCreateRequest(
            "AppName",
            new string('a', 251),
            "Red",
            "Description"
        );

        // Act
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(
            appDateTypeCreateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("ShortName", validation.GetErrorField());
    }

    [Fact]
    public async Task CreateAppDateType_ShouldReturnValidationError_WhenColorIsNullOrEmpty()
    {
        // Arrange
        var appDateTypeCreateRequest = new AppDateTypeCreateRequest(
            "AppName",
            "AppShortName",
            string.Empty,
            "Description"
        );

        // Act
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(
            appDateTypeCreateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Color", validation.GetErrorField());
    }

    [Fact]
    public async Task CreateAppDateType_ShouldReturnValidationError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var appDateTypeCreateRequest = new AppDateTypeCreateRequest(
            new string('a', 251),
            "AppShortName",
            "Red",
            "Description"
        );

        // Act
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(
            appDateTypeCreateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenIdIsInvalid()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            "AppName",
            "AppShortName",
            "Red",
            "Description"
        ) { Id = 0 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
            appDateTypeUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            string.Empty, // Invalid Name
            "AppShortName",
            "Red",
            "Description"
        ) { Id = 1 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
            appDateTypeUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            new string('a', 251), // Exceeds max length
            "AppShortName",
            "Red",
            "Description"
        ) { Id = 1 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
            appDateTypeUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenShortNameIsEmpty()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            "AppName",
            string.Empty, // Invalid ShortName
            "Red",
            "Description"
        ) { Id = 1 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
            appDateTypeUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("ShortName", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenShortNameExceedsMaxLength()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            "AppName",
            new string('a', 251), // Exceeds max length
            "Red",
            "Description"
        ) { Id = 1 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
            appDateTypeUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("ShortName", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenColorIsNullOrEmpty()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            "AppName",
            "AppShortName",
            string.Empty, // Invalid Color
            "Description"
        ) { Id = 1 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
            appDateTypeUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Color", validation.GetErrorField());
    }

    [Fact]
    public async Task EnableAppDateType_ShouldReturnValidationError_WhenIdIsInvalid()
    {
        // Arrange
        var appDateTypeEnabledRequest = new AppDateTypeEnabledRequest(true) { Id = 0 };

        // Act
        var validation = await new AppDateTypeEnabledRequestValidator().ValidateAsync(
            appDateTypeEnabledRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }
}
