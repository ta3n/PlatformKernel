using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class PlansEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task PlanCreateUpdateCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new PlanCreateCommand
        {
            Payload = new PlanCreateRequest(
                string.Empty,
                "Description of the plan",
                false,
                PlanTypes.Combo
            )
        };
        var validator = new PlanCreateUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanCreateUpdateCommandValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new PlanCreateCommand
        {
            Payload = new PlanCreateRequest(
                new string('a', 1001),
                "Description of the plan",
                false,
                PlanTypes.Combo
            )
        };
        var validator = new PlanCreateUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanCreateUpdateCommandValidator_ShouldThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new PlanCreateCommand
        {
            Payload = new PlanCreateRequest(
                "Plan Name",
                new string('a', 1001),
                false,
                PlanTypes.Combo
            )
        };
        var validator = new PlanCreateUpdateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Summary", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceCreateSiteCommandValidator_ShouldThrowError_WhenPlanIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceCreateSiteCommand
        {
            Payload = new RoomGroupSiteCreateRequest(
                -1,
                1,
                1
            )
        };
        var validator = new PlanPriceCreateSiteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceCreateSiteCommandValidator_ShouldThrowError_WhenRomTypeIdIsNegative()
    {
        // Arrange
        var command = new PlanPriceCreateSiteCommand
        {
            Payload = new RoomGroupSiteCreateRequest(
                1,
                -1,
                1
            )
        };
        var validator = new PlanPriceCreateSiteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceCreateSiteCommandValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var command = new PlanPriceCreateSiteCommand
        {
            Payload = new RoomGroupSiteCreateRequest(
                1,
                1,
                -1
            )
        };
        var validator = new PlanPriceCreateSiteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenPlanIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateChildrenPriceCommand(
            0,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                        1,
                        true,
                        false,
                        PriceSettingTypes.Discount,
                        100f
                    )
                ]
            )
        };
        var validator = new PlanPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenRoomTypeIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateChildrenPriceCommand(
            1,
            0,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                        1,
                        true,
                        false,
                        PriceSettingTypes.Discount,
                        100f
                    )
                ]
            )
        };
        var validator = new PlanPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenSiteIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateChildrenPriceCommand(
            1,
            1,
            0
        )
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new(
                        1,
                        true,
                        false,
                        PriceSettingTypes.Discount,
                        100f
                    )
                ]
            )
        };
        var validator = new PlanPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenPersonAgeTypesIsEmpty()
    {
        // Arrange
        var command = new PlanPriceUpdateChildrenPriceCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                []
            )
        };
        var validator = new PlanPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PersonAgeTypes", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenPersonAgeTypeIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateChildrenPriceCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                        0,
                        true,
                        false,
                        PriceSettingTypes.Discount,
                        100f
                    )
                ]
            )
        };
        var validator = new PlanPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("PersonAgeTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenPlanIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateDiscountCommand(
            0,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateDiscountRequest
            (
                [
                    new RomTypeUpdateDiscountDataRequest(0, 1, 1, 10, PriceSettingTypes.Discount, null)
                ]
            )
        };

        var validator = new PlanPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenRoomTypeIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateDiscountCommand(
            1,
            0,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateDiscountRequest
            (
                [
                    new RomTypeUpdateDiscountDataRequest(
                        0,
                        1,
                        1,
                        10,
                        PriceSettingTypes.Discount,
                        null
                    )
                ]
            )
        };

        var validator = new PlanPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenSiteIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateDiscountCommand(
            1,
            1,
            0
        )
        {
            Payload = new RoomGroupPriceUpdateDiscountRequest
            (
                [
                    new RomTypeUpdateDiscountDataRequest(
                        0,
                        1,
                        1,
                        10,
                        PriceSettingTypes.Discount,
                        null
                    )
                ]
            )
        };

        var validator = new PlanPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenStartPrevDayIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateDiscountCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateDiscountRequest
            (
                [
                    new(
                        -1,
                        1,
                        1,
                        10,
                        PriceSettingTypes.Discount,
                        null
                    )
                ]
            )
        };

        var validator = new PlanPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("StartPrevDay", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenEndPrevDayIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateDiscountCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateDiscountRequest
            (
                [
                    new RomTypeUpdateDiscountDataRequest(
                        20250101,
                        1,
                        1,
                        10,
                        PriceSettingTypes.Discount,
                        null
                    )
                ]
            )
        };

        var validator = new PlanPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndPrevDay", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenPersonMinIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateDiscountCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateDiscountRequest
            (
                [
                    new RomTypeUpdateDiscountDataRequest(
                        0,
                        1,
                        0,
                        10,
                        PriceSettingTypes.Discount,
                        null
                    )
                ]
            )
        };

        var validator = new PlanPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("PersonMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenPlanIdIsInvalid()
    {
        // Arrange

        var payload = new RoomGroupPriceUpdatePriceCalendarRequest
        (
            [
                new RomTypeUpdatePriceDataCalendarRequest(
                    20230101,
                    1,
                    10,
                    1000,
                    true
                )
            ]
        );

        var command = new PlanPriceUpdatePriceCalendarCommand(
            0,
            1,
            1
        ) { Payload = payload };

        var validator = new PlanPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenRoomTypeIdIsInvalid()
    {
        // Arrange
        var payload = new RoomGroupPriceUpdatePriceCalendarRequest
        (
            [
                new RomTypeUpdatePriceDataCalendarRequest(
                    20230101,
                    1,
                    10,
                    1000,
                    true
                )
            ]
        );

        var command = new PlanPriceUpdatePriceCalendarCommand(
            1,
            0,
            1
        ) { Payload = payload };

        var validator = new PlanPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenDateCalendarIsInvalid()
    {
        // Arrange
        var payload = new RoomGroupPriceUpdatePriceCalendarRequest
        (
            [
                new RomTypeUpdatePriceDataCalendarRequest(
                    -1,
                    1,
                    10,
                    1000,
                    true
                )
            ]
        );

        var command = new PlanPriceUpdatePriceCalendarCommand(
            1,
            1,
            1
        ) { Payload = payload };

        var validator = new PlanPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("DateCalendar", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenPersonMinIsInvalid()
    {
        // Arrange
        var payload = new RoomGroupPriceUpdatePriceCalendarRequest
        (
            [
                new RomTypeUpdatePriceDataCalendarRequest(
                    20230101,
                    0, // Invalid PersonMin
                    10,
                    1000,
                    true
                )
            ]
        );

        var command = new PlanPriceUpdatePriceCalendarCommand(
            1,
            1,
            1
        ) { Payload = payload };

        var validator = new PlanPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("PersonMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenPersonMaxIsInvalid()
    {
        // Arrange
        var payload = new RoomGroupPriceUpdatePriceCalendarRequest
        (
            [
                new(
                    20230101,
                    1,
                    0,
                    1000,
                    true
                )
            ]
        );

        var command = new PlanPriceUpdatePriceCalendarCommand(
            1,
            1,
            1
        ) { Payload = payload };

        var validator = new PlanPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("PersonMax", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateSaleCommandValidator_ShouldThrowError_WhenPlanIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateSaleCommand(
            0,
            1,
            1
        )
        {
            Payload = new()
            {
                AutoExtendEveryMonthDay = 15,
                AutoExtendMonth = 1,
                UseAutoExtend = true
            }
        };

        var validator = new PlanPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateSaleCommandValidator_ShouldThrowError_WhenRoomTypeIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateSaleCommand(
            1,
            0,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateSaleRequest
            {
                AutoExtendEveryMonthDay = 15,
                AutoExtendMonth = 1,
                UseAutoExtend = true
            }
        };

        var validator = new PlanPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateSaleCommandValidator_ShouldThrowError_WhenSiteIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateSaleCommand(
            1,
            1,
            0
        )
        {
            Payload = new RoomGroupPriceUpdateSaleRequest
            {
                AutoExtendEveryMonthDay = 15,
                AutoExtendMonth = 1,
                UseAutoExtend = true
            }
        };

        var validator = new PlanPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenPlanIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateStandardPriceCommand(
            0,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateStandardPriceRequest(
                [
                    new(1, 1, 10, 1000)
                ]
            )
        };
        var validator = new PlanPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenRoomTypeIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateStandardPriceCommand(
            1,
            0,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateStandardPriceRequest(
                [
                    new(1, 1, 10, 1000)
                ]
            )
        };

        var validator = new PlanPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenSiteIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateStandardPriceCommand(
            1,
            1,
            0
        )
        {
            Payload = new RoomGroupPriceUpdateStandardPriceRequest(
                [
                    new(1, 1, 10, 1000)
                ]
            )
        };

        var validator = new PlanPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenDateTypeIdIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateStandardPriceCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateStandardPriceRequest(
                [
                    new(0, 1, 10, 1000)
                ]
            )
        };

        var validator = new PlanPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E1062, validation.Errors.GetErrorCode());
        Assert.Contains("DateTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenPersonMinIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateStandardPriceCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateStandardPriceRequest(
                [
                    new(1, 0, 10, 1000)
                ]
            )
        };

        var validator = new PlanPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("PersonMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenPersonMaxIsInvalid()
    {
        // Arrange
        var command = new PlanPriceUpdateStandardPriceCommand(
            1,
            1,
            1
        )
        {
            Payload = new RoomGroupPriceUpdateStandardPriceRequest(
                [
                    new(1, 1, 0, 1000)
                ]
            )
        };

        var validator = new PlanPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("PersonMax", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateBasicSettingCommandValidator_ShouldThrowError_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var command = new PlanUpdateBasicSettingCommand(1)
        {
            Payload = new PlanUpdateBasicSettingRequest(
                new string('a', 251),
                "Import Name",
                "Summary text",
                "Description text",
                [
                    new FileOfPlanUpdateBasicSettingRequest(1, 1)
                ]
            )
        };
        var validator = new PlanUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateBasicSettingCommandValidator_ShouldThrowError_WhenFileIndexIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateBasicSettingCommand(1)
        {
            Payload = new PlanUpdateBasicSettingRequest(
                "Valid Name",
                "Import Name",
                "Summary text",
                "Description text",
                [
                    new FileOfPlanUpdateBasicSettingRequest(1, -1)
                ]
            )
        };
        var validator = new PlanUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("Index", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateBasicSettingCommandValidator_ShouldThrowError_WhenFileIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateBasicSettingCommand(1)
        {
            Payload = new PlanUpdateBasicSettingRequest(
                "Valid Name",
                "Import Name",
                "Summary text",
                "Description text",
                [
                    new FileOfPlanUpdateBasicSettingRequest(-1, 0)
                ]
            )
        };
        var validator = new PlanUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateCancelCommandValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateCancelCommand(0) { Payload = new PlanUpdateCancelRequest(true, 5, TimeSpan.FromHours(2), 1) };
        var validator = new PlanUpdateCancelCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateCancelCommandValidator_ShouldThrowError_WhenCancellationIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateCancelCommand(1) { Payload = new PlanUpdateCancelRequest(true, 5, TimeSpan.FromHours(2), 0) };
        var validator = new PlanUpdateCancelCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("CancellationId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateCancelCommandValidator_ShouldThrowError_WhenCancelLimitIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateCancelCommand(1) { Payload = new PlanUpdateCancelRequest(true, 5, TimeSpan.FromDays(-1), 1) };
        var validator = new PlanUpdateCancelCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0012, validation.Errors.GetErrorCode());
        Assert.Contains("CancelLimit", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateDisplayCommandValidator_ShouldThrowError_WhenTagsContainEmptyValue()
    {
        // Arrange
        var command = new PlanUpdateDisplayCommand(1)
        {
            Payload = new PlanUpdateDisplayRequest(
                [""],
                [1, 2],
                [1, 3]
            )
        };
        var validator = new PlanUpdateDisplayCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Tags", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateDisplayCommandValidator_ShouldThrowError_WhenPlanCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new PlanUpdateDisplayCommand(1)
        {
            Payload = new PlanUpdateDisplayRequest(
                ["Tags"],
                [1, -2],
                [1, 3]
            )
        };
        var validator = new PlanUpdateDisplayCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("PlanCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateDisplayCommandValidator_ShouldThrowError_WhenMasterCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new PlanUpdateDisplayCommand(1)
        {
            Payload = new PlanUpdateDisplayRequest(
                ["Tags"],
                [1, 2],
                [1, -3]
            )
        };
        var validator = new PlanUpdateDisplayCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("MasterCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateImportantNoteCommandValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateImportantNoteCommand(0)
        {
            Payload = new PlanUpdateImportantNoteRequest
            {
                Payment = "Payment text",
                Meal = "Meal text",
                Other = "Other text"
            }
        };
        var validator = new PlanUpdateImportantNoteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateImportantNoteCommandValidator_ShouldThrowError_WhenPaymentExceedsMaxLength()
    {
        // Arrange
        var command = new PlanUpdateImportantNoteCommand(1)
        {
            Payload = new PlanUpdateImportantNoteRequest
            {
                Payment = new string('a', 1001),
                Meal = "Meal text",
                Other = "Other text"
            }
        };
        var validator = new PlanUpdateImportantNoteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Payment", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateImportantNoteCommandValidator_ShouldThrowError_WhenMealExceedsMaxLength()
    {
        // Arrange
        var command = new PlanUpdateImportantNoteCommand(1)
        {
            Payload = new PlanUpdateImportantNoteRequest
            {
                Payment = "Payment text",
                Meal = new string('b', 1001),
                Other = "Other text"
            }
        };
        var validator = new PlanUpdateImportantNoteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Meal", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateImportantNoteCommandValidator_ShouldThrowError_WhenOtherExceedsMaxLength()
    {
        // Arrange
        var command = new PlanUpdateImportantNoteCommand(1)
        {
            Payload = new PlanUpdateImportantNoteRequest
            {
                Payment = "Payment text",
                Meal = "Meal text",
                Other = new string('c', 1001)
            }
        };
        var validator = new PlanUpdateImportantNoteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Other", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateMealCommandValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateMealCommand(0)
        {
            Payload = new PlanUpdateMealRequest
            (
                [new PlanMealTypeRequest(1, MealTypeEatTypes.Room)]
            )
        };
        var validator = new PlanUpdateMealCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateOptionCommandValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdateOptionCommand(0)
        {
            Payload = new PlanUpdateOptionRequest(
                true,
                [1, 2]
            )
        };
        var validator = new PlanUpdateOptionCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateOptionCommandValidator_ShouldThrowError_WhenOptionIdsIsEmptyAndUseFixedOptionItemIsTrue()
    {
        // Arrange
        var command = new PlanUpdateOptionCommand(1)
        {
            Payload = new PlanUpdateOptionRequest(
                true,
                null
            )
        };
        var validator = new PlanUpdateOptionCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("OptionIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePaymentMethodCommandValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var command = new PlanUpdatePaymentMethodCommand(0) { Payload = new PlanUpdatePaymentMethodRequest(true, false) };
        var validator = new PlanUpdatePaymentMethodCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePaymentMethodCommandValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var command = new PlanUpdatePaymentMethodCommand(-1) { Payload = new PlanUpdatePaymentMethodRequest(true, false) };
        var validator = new PlanUpdatePaymentMethodCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePublishAcceptCommandValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            []
        );
        var command = new PlanUpdatePublishAcceptCommand(-1) { Payload = request };
        var validator = new PlanUpdatePublishAcceptCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePublishAcceptCommandValidator_ShouldThrowError_WhenSitesIsNull()
    {
        // Arrange
        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            null
        );
        var command = new PlanUpdatePublishAcceptCommand(1) { Payload = request };
        var validator = new PlanUpdatePublishAcceptCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Sites", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePublishAcceptCommandValidator_ShouldThrowError_WhenDisplayDateEndIsBeforeDisplayDateStart()
    {
        // Arrange
        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow.AddDays(1)),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            []
        );
        var command = new PlanUpdatePublishAcceptCommand(1) { Payload = request };
        var validator = new PlanUpdatePublishAcceptCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("DisplayDateEnd", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePublishAcceptCommandValidator_ShouldThrowError_WhenAcceptDateEndIsBeforeAcceptDateStart()
    {
        // Arrange
        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow.AddDays(1)),
            AppDate.GetId(DateTime.UtcNow),
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            []
        );
        var command = new PlanUpdatePublishAcceptCommand(1) { Payload = request };
        var validator = new PlanUpdatePublishAcceptCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("AcceptDateEnd", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePublishAcceptCommandValidator_ShouldThrowError_WhenDisplayDateEndIsInvalid()
    {
        // Arrange
        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            []
        );
        var command = new PlanUpdatePublishAcceptCommand(1) { Payload = request };
        var validator = new PlanUpdatePublishAcceptCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E1018, validation.Errors.GetErrorCode());
        Assert.Contains("Payload", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdatePublishAcceptCommandValidator_ShouldThrowError_WhenAcceptDateEndIsInvalid()
    {
        // Arrange
        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow),
            null,
            2,
            2,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            new TimeSpan(0, 10, 0),
            []
        );
        var command = new PlanUpdatePublishAcceptCommand(1) { Payload = request };
        var validator = new PlanUpdatePublishAcceptCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E1019, validation.Errors.GetErrorCode());
        Assert.Contains("Payload", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateQuestionCommandValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var command = new PlanUpdateQuestionCommand(-1) { Payload = new PlanUpdateQuestionRequest([1]) };
        var validator = new PlanUpdateQuestionCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateRoomTypeCommandValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var command = new PlanUpdateRoomTypeCommand(-1) { Payload = new PlanUpdateRoomTypeRequest([1]) };
        var validator = new PlanUpdateRoomTypeCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSaleCommandValidator_ShouldThrowError_WhenCheckInStartIsInvalid()
    {
        // Arrange
        var request = new PlanUpdateSaleRequest(
            null,
            new TimeSpan(0, 10, 0),
            new TimeSpan(0, 10, 0),
            0,
            1,
            1,
            PlanDaySaleLimitTypes.Pair,
            true,
            1,
            1,
            true,
            1,
            1,
            1,
            1
        );

        var command = new PlanUpdateSaleCommand(1) { Payload = request };

        var validator = new PlanUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("CheckInStart", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSaleCommandValidator_ShouldThrowError_WhenCheckOutIsInvalid()
    {
        // Arrange
        var request = new PlanUpdateSaleRequest(
            new TimeSpan(0, 10, 0),
            new TimeSpan(1, 10, 0),
            null,
            0,
            1,
            1,
            PlanDaySaleLimitTypes.Pair,
            true,
            1,
            1,
            true,
            1,
            1,
            1,
            1
        );

        var command = new PlanUpdateSaleCommand(1) { Payload = request };

        var validator = new PlanUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("CheckOut", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSaleCommandValidator_ShouldThrowError_WhenCheckInEndIsInvalid()
    {
        // Arrange
        var request = new PlanUpdateSaleRequest(
            new TimeSpan(0, 10, 0),
            new TimeSpan(0, 10, 0),
            null,
            0,
            1,
            1,
            PlanDaySaleLimitTypes.Pair,
            true,
            1,
            1,
            true,
            1,
            1,
            1,
            1
        );

        var command = new PlanUpdateSaleCommand(1) { Payload = request };

        var validator = new PlanUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        // Assert.Contains("CheckInEnd", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSpecialCommandValidator_ShouldThrowError_WhenIdIsNullOrLessThanOrEqualToZero()
    {
        // Arrange
        var request = new PlanUpdateSpecialRequest(
            SecretWord: "Secret",
            IsSecret: true
        );

        var command = new PlanUpdateSpecialCommand(0) // Invalid Id
        {
            Payload = request
        };

        var validator = new PlanUpdateSpecialCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSpecialCommandValidator_ShouldThrowError_WhenSecretWordIsEmptyAndIsSecretIsTrue()
    {
        // Arrange
        var request = new PlanUpdateSpecialRequest(
            SecretWord: string.Empty, // Invalid SecretWord
            IsSecret: true
        );

        var command = new PlanUpdateSpecialCommand(1) { Payload = request };

        var validator = new PlanUpdateSpecialCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SecretWord", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSpecialCommandValidator_ShouldThrowError_WhenSecretWordIsWhitespaceAndIsSecretIsTrue()
    {
        // Arrange
        var request = new PlanUpdateSpecialRequest(
            SecretWord: "   ", // Invalid SecretWord (whitespace)
            IsSecret: true
        );

        var command = new PlanUpdateSpecialCommand(1) { Payload = request };

        var validator = new PlanUpdateSpecialCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SecretWord", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSpecialCommandValidator_ShouldThrowError_WhenSecretWordExceedsMaxLength()
    {
        // Arrange
        var request = new PlanUpdateSpecialRequest(
            SecretWord: new string('a', 1001), // SecretWord exceeds max length
            IsSecret: true
        );

        var command = new PlanUpdateSpecialCommand(1) { Payload = request };

        var validator = new PlanUpdateSpecialCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("SecretWord", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task PlanUpdateSpecialCommandValidator_ShouldPass_WhenValidSecretWordAndIsSecretTrue()
    {
        // Arrange
        var request = new PlanUpdateSpecialRequest(
            SecretWord: "ValidSecretWord", // Valid SecretWord
            IsSecret: true
        );

        var command = new PlanUpdateSpecialCommand(1) { Payload = request };

        var validator = new PlanUpdateSpecialCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.True(validation.IsValid);
    }
}
