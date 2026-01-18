using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class QuestionsEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task QuestionCreateRequestValidator_ShouldThrowError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var request = new QuestionCreateRequest(null, "Description");
        var validator = new QuestionCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionCreateRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var request = new QuestionCreateRequest(new string('a', 251), "Description");
        var validator = new QuestionCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionEnabledRequestValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var request = new QuestionEnabledRequest(true) { Id = null };

        var validator = new QuestionEnabledRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionEnabledRequestValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var request = new QuestionEnabledRequest(true) { Id = -1 };

        var validator = new QuestionEnabledRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionUpdateRequestValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var request = new QuestionUpdateRequest(
            "Valid Name",
            "Valid Description",
            "Valid FormData",
            QuestionTypes.Select,
            true
        ) { Id = null };

        var validator = new QuestionUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionUpdateRequestValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new QuestionUpdateRequest(
            "Valid Name",
            "Valid Description",
            "Valid FormData",
            QuestionTypes.Select,
            true
        ) { Id = 0 };

        var validator = new QuestionUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionUpdateRequestValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var request = new QuestionUpdateRequest(
            "",
            "Valid Description",
            "Valid FormData",
            QuestionTypes.Select,
            true
        ) { Id = 1 };

        var validator = new QuestionUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionUpdateRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var request = new QuestionUpdateRequest(
            new string('A', 251),
            "Valid Description",
            "Valid FormData",
            QuestionTypes.Select,
            true
        ) { Id = 1 };

        var validator = new QuestionUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionUpdateRequestValidator_ShouldThrowError_WhenFormDataIsEmpty()
    {
        // Arrange
        var request = new QuestionUpdateRequest(
            "Valid Name",
            "Valid Description",
            "",
            QuestionTypes.Select,
            true
        ) { Id = 1 };

        var validator = new QuestionUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("FormData", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task QuestionUpdateRequestValidator_ShouldThrowError_WhenFormDataExceedsMaxLength()
    {
        // Arrange
        var request = new QuestionUpdateRequest(
            "Valid Name",
            "Valid Description",
            new string('B', 10001),
            QuestionTypes.Select,
            true
        ) { Id = 1 };

        var validator = new QuestionUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FormData", validation.Errors.GetErrorField());
    }
}
