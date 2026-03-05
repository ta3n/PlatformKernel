using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class AppDateTypesEndpointValidationUnitTest : BaseUnitTest
{
    private IAppDateTypeService mockAppDateTypeService { get; set; } = null!;

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
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(appDateTypeCreateRequest);

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
            new string('a', 251), // Exceeds max length
            "Red",
            "Description"
        );

        // Act
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(appDateTypeCreateRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("ShortName", validation.GetErrorField());
    }

    [Fact]
    public async Task CreateAppDateType_ShouldReturnValidationError_WhenColorIsEmpty()
    {
        // Arrange
        var appDateTypeCreateRequest = new AppDateTypeCreateRequest(
            "AppName",
            "AppShortName",
            string.Empty, // Invalid Color
            "Description"
        );

        // Act
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(appDateTypeCreateRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Color", validation.GetErrorField());
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
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(appDateTypeUpdateRequest);

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
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(appDateTypeUpdateRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAppDateType_ShouldReturnValidationError_WhenShortNameIsEmpty()
    {
        // Arrange
        var appDateTypeUpdateRequest = new AppDateTypeUpdateRequest(
            "AppName",
            string.Empty,
            "Red",
            "Description"
        ) { Id = 1 };

        // Act
        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(appDateTypeUpdateRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("ShortName", validation.GetErrorField());
    }

    [Fact]
    public async Task EnableAppDateType_ShouldReturnValidationError_WhenIdIsInvalid()
    {
        // Arrange
        var appDateTypeEnabledRequest = new AppDateTypeEnabledRequest(true) { Id = 0 };

        // Act
        var validation = await new AppDateTypeEnabledRequestValidator().ValidateAsync(appDateTypeEnabledRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }
}
