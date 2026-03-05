using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class OptionItemEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task OptionItemCreateCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new OptionItemCreateCommand
        {
            Payload = new OptionItemCreateRequest(
                string.Empty, // Name is empty
                "Description",
                10,
                100
            )
        };
        var validator = new OptionItemCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemCreateCommandValidator_ShouldThrowError_WhenNameIsTooLong()
    {
        // Arrange
        var command = new OptionItemCreateCommand
        {
            Payload = new OptionItemCreateRequest(
                new string('A', 256), // Name is too long (more than 255 characters)
                "Description",
                10,
                100
            )
        };
        var validator = new OptionItemCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemCreateCommandValidator_ShouldThrowError_WhenBaseNumberIsNegative()
    {
        // Arrange
        var command = new OptionItemCreateCommand
        {
            Payload = new OptionItemCreateRequest(
                "Valid Name",
                "Description",
                -1, // BaseNumber is negative
                100
            )
        };
        var validator = new OptionItemCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("BaseNumber", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemCreateCommandValidator_ShouldThrowError_WhenPriceIsNegative()
    {
        // Arrange
        var command = new OptionItemCreateCommand
        {
            Payload = new OptionItemCreateRequest(
                "Valid Name",
                "Description",
                10,
                -1 // Price is negative
            )
        };
        var validator = new OptionItemCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("Price", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemDeleteCommandValidator_ShouldThrowError_WhenIdIsZero()
    {
        // Arrange
        var command = new OptionItemDeleteCommand
        {
            Payload = new OptionItemDeleteRequest
            (
                0
            )
        };
        var validator = new OptionItemDeleteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemInventoryAdjustCommandValidator_ShouldThrowError_WhenPayloadIsEmpty()
    {
        // Arrange
        var command = new OptionItemInventoryAdjustCommand { Payload = [] };
        var validator = new OptionItemInventoryAdjustCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Payload", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemChangeRemainRequestValidator_ShouldThrowError_WhenAppDateIdIsZero()
    {
        // Arrange
        var command = new OptionItemChangeRemainRequest(
            0,
            1,
            10,
            false
        );
        var validator = new OptionItemChangeRemainRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemChangeRemainRequestValidator_ShouldThrowError_WhenOptionItemIdIsZero()
    {
        // Arrange

        var command = new OptionItemChangeRemainRequest(
            1,
            0,
            15,
            false
        );
        var validator = new OptionItemChangeRemainRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("OptionItemId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemInventoryGetAllQueryValidator_ShouldThrowError_WhenStartAppDateIsLessThanZero()
    {
        // Arrange
        var mockPageable = PageableBinderConfig.DefaultPageable;
        var command = new OptionItemInventoryGetAllQuery(
            mockPageable,
            -1, // Invalid StartAppDate
            10 // Valid EndAppDate
        );
        var validator = new OptionItemInventoryGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("StartAppDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemInventoryGetAllQueryValidator_ShouldThrowError_WhenEndAppDateIsLessThanStartAppDate()
    {
        // Arrange
        var mockPageable = PageableBinderConfig.DefaultPageable;
        var command = new OptionItemInventoryGetAllQuery(
            mockPageable,
            20250202,
            20250101
        );
        var validator = new OptionItemInventoryGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemUpdateCommandValidator_ShouldThrowError_WhenBaseNumberIsNegative()
    {
        // Arrange
        var command = new OptionItemUpdateCommand
        {
            Payload = new OptionItemUpdateRequest(
                "Product A",
                "Description of Product A",
                -1,
                100,
                [1],
                [2],
                [101],
                [new ImageOfOptionItemUpdateRequest(1, 0)]
            ) { Id = 1 }
        };
        var validator = new OptionItemUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("BaseNumber", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemUpdateCommandValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var command = new OptionItemUpdateCommand
        {
            Payload = new OptionItemUpdateRequest(
                "Product A",
                "Description of Product A",
                1,
                100,
                [1],
                [2],
                [101],
                [new ImageOfOptionItemUpdateRequest(1, 0)]
            ) { Id = -1 }
        };
        var validator = new OptionItemUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemUpdateCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new OptionItemUpdateCommand
        {
            Payload = new OptionItemUpdateRequest(
                string.Empty,
                "Description of Product A",
                10,
                100,
                [1],
                [2],
                [101],
                [new ImageOfOptionItemUpdateRequest(1, 0)]
            ) { Id = 1 }
        };
        var validator = new OptionItemUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task OptionItemUpdateCommandValidator_ShouldThrowError_WhenPriceIsNegative()
    {
        // Arrange
        var command = new OptionItemUpdateCommand
        {
            Payload = new OptionItemUpdateRequest(
                "Product A",
                "Description of Product A",
                10,
                -100,
                [1],
                [2],
                [101],
                [new ImageOfOptionItemUpdateRequest(1, 0)]
            ) { Id = 1 }
        };
        var validator = new OptionItemUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("Price", validation.Errors.GetErrorField());
    }
}
