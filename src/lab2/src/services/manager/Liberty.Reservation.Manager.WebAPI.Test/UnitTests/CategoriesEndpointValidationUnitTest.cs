using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class CategoriesEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task CategoryCreateRequestValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CategoryCreateRequest(
            string.Empty,
            "Valid description"
        );
        var validator = new CategoryCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryCreateRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var request = new CategoryCreateRequest(
            new string('A', 251), // Name exceeds max length (250)
            "Valid description"
        );
        var validator = new CategoryCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryCreateRequestValidator_ShouldThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var request = new CategoryCreateRequest(
            "Valid Name",
            new string('A', 501) // Description exceeds max length (500)
        );
        var validator = new CategoryCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Description", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryCreateRequestValidator_ShouldNotThrowError_WhenDataIsValid()
    {
        // Arrange
        var request = new CategoryCreateRequest(
            "Valid Name", // Valid Name
            "Valid description" // Valid Description
        );
        var validator = new CategoryCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.True(validation.IsValid); // No validation errors should be thrown
    }

    [Fact]
    public async Task CategoryEnabledRequestValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new CategoryEnabledRequest(true) { Id = 0 };
        var validator = new CategoryEnabledRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryUpdateRequestValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var request = new CategoryUpdateRequest(
            "Valid Name",
            "Valid Description"
        ) { Id = null };
        var validator = new CategoryUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryUpdateRequestValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new CategoryUpdateRequest(
            "Valid Name",
            "Valid Description"
        ) { Id = 0 };
        var validator = new CategoryUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryUpdateRequestValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CategoryUpdateRequest(
            string.Empty, // Name is empty
            "Valid Description"
        ) { Id = 1 };
        var validator = new CategoryUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryUpdateRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var request = new CategoryUpdateRequest(
            new string('A', 251), // Name exceeds 250 characters
            "Valid Description"
        ) { Id = 1 };
        var validator = new CategoryUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CategoryUpdateRequestValidator_ShouldThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var request = new CategoryUpdateRequest(
            "Valid Name",
            new string('B', 501) // Description exceeds 500 characters
        ) { Id = 1 };
        var validator = new CategoryUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Description", validation.GetErrorField());
    }

    [Fact]
    public async Task ItemUpdateArrangeOrderRequestValidator_ShouldThrowError_WhenIdsContainInvalidValue()
    {
        // Arrange
        var command = new ItemUpdateOrderRequest
        (
            []
        );
        var validator = new ItemUpdateArrangeOrderRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Ids", validation.GetErrorField());
    }

    [Fact]
    public async Task ItemUpdateArrangeOrderRequestValidator_ShouldThrowError_WhenIdsIsNegative()
    {
        // Arrange
        var command = new ItemUpdateOrderRequest
        (
            [-1]
        );
        var validator = new ItemUpdateArrangeOrderRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Ids", validation.GetErrorField());
    }
}
