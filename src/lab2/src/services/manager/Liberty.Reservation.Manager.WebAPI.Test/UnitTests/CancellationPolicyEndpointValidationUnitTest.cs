using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class CancellationPolicyEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task CancellationPolicyCreateCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CancellationPolicyCreateCommand
        {
            Payload = new CancellationPolicyCreateRequest(
                string.Empty,
                "Valid description"
            )
        };
        var validator = new CancellationPolicyCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyCreateCommandValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new CancellationPolicyCreateCommand
        {
            Payload = new CancellationPolicyCreateRequest(
                new string('A', 501), // 501 characters
                "Valid description"
            )
        };
        var validator = new CancellationPolicyCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyCreateCommandValidator_ShouldThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new CancellationPolicyCreateCommand
        {
            Payload = new CancellationPolicyCreateRequest(
                "Valid Name",
                new string('A', 501) // 501 characters
            )
        };
        var validator = new CancellationPolicyCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Description", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyDeleteCommandValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var command = new CancellationPolicyDeleteCommand { Payload = new CancellationPolicyDeleteRequest(0) };
        var validator = new CancellationPolicyDeleteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyDeleteCommandValidator_ShouldNotThrowError_WhenIdIsGreaterThanZero()
    {
        // Arrange
        var command = new CancellationPolicyDeleteCommand { Payload = new CancellationPolicyDeleteRequest(1) };
        var validator = new CancellationPolicyDeleteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.True(validation.IsValid);
    }

    [Fact]
    public async Task CancellationPolicyUpdateCommandValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var command = new CancellationPolicyUpdateCommand
        {
            Payload = new CancellationPolicyUpdateRequest(
                "Valid Name",
                "Valid Description",
                "Rule Detail",
                []
            )
        };
        var validator = new CancellationPolicyUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyUpdateCommandValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var command = new CancellationPolicyUpdateCommand
        {
            Payload = new CancellationPolicyUpdateRequest(
                "Valid Name",
                "Valid Description",
                "Rule Detail",
                []
            )
        };
        var validator = new CancellationPolicyUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyUpdateCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CancellationPolicyUpdateCommand
        {
            Payload = new CancellationPolicyUpdateRequest(
                string.Empty,
                "Valid",
                "Rule Detail",
                []
            ) { Id = 1 }
        };
        var validator = new CancellationPolicyUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CancellationPolicyUpdateCommandValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new CancellationPolicyUpdateCommand
        {
            Payload = new CancellationPolicyUpdateRequest(
                new string('A', 251),
                "Valid Des",
                "Rule Detail",
                []
            ) { Id = 1 }
        };
        var validator = new CancellationPolicyUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task DataOfCancellationUpdateRequestValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var data = new DataOfCancellationUpdateRequest(
            0,
            20250101,
            20250202,
            3,
            "description",
            0
        );
        var validator = new DataOfCancellationUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(data);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task DataOfCancellationUpdateRequestValidator_ShouldThrowError_WhenDayStartIsLessThanOrEqualToZero()
    {
        // Arrange
        var data = new DataOfCancellationUpdateRequest(
            1,
            0,
            20250101,
            20250202,
            "description",
            0
        );
        var validator = new DataOfCancellationUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(data);

        // Assert
        Assert.Equal(ErrorCode.E0004, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task DataOfCancellationUpdateRequestValidator_ShouldThrowError_WhenDayEndIsNotGreaterThanDayStart()
    {
        // Arrange
        var data = new DataOfCancellationUpdateRequest(
            1,
            20250101,
            20250101,
            1,
            "description",
            0
        );
        var validator = new DataOfCancellationUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(data);

        // Assert
        Assert.Equal(ErrorCode.E0100, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task DataOfCancellationUpdateRequestValidator_ShouldThrowError_WhenRateIsLessThanZero()
    {
        // Arrange
        var data = new DataOfCancellationUpdateRequest(
            1,
            20250101,
            20250202,
            -1,
            "description",
            0
        );
        var validator = new DataOfCancellationUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(data);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("Rate", validation.GetErrorField());
    }

    [Fact]
    public async Task DataOfCancellationUpdateRequestValidator_ShouldThrowError_WhenRateIsGreaterThan100()
    {
        // Arrange
        var data = new DataOfCancellationUpdateRequest(
            1,
            20250101,
            20250202,
            1000,
            "description",
            0
        );
        var validator = new DataOfCancellationUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(data);

        // Assert
        Assert.Equal(ErrorCode.E0004, validation.Errors.GetErrorCode());
        Assert.Contains("Rate", validation.GetErrorField());
    }
}
