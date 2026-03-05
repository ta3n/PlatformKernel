using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupInventory;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class RoomGroupsEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task RoomGroupCreateCommandValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new RoomGroupCreateCommand
        {
            Payload = new RoomGroupCreateRequest(
                new string('a', 256),
                "Valid Description",
                10,
                20,
                1
            )
        };
        var validator = new RoomGroupCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupCreateCommandValidator_ShouldThrowError_WhenBaseNumberIsNegative()
    {
        // Arrange
        var command = new RoomGroupCreateCommand
        {
            Payload = new RoomGroupCreateRequest(
                "Valid Name",
                "Valid Description",
                10,
                20,
                -1
            )
        };
        var validator = new RoomGroupCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("BaseNumber", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupCreateCommandValidator_ShouldThrowError_WhenCapacityMinIsZeroOrNegative()
    {
        // Arrange
        var command = new RoomGroupCreateCommand
        {
            Payload = new RoomGroupCreateRequest(
                "Valid Name",
                "Valid Description",
                0,
                20,
                1
            )
        };
        var validator = new RoomGroupCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Contains("CapacityMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupCreateCommandValidator_ShouldThrowError_WhenCapacityMaxIsLessThanCapacityMin()
    {
        // Arrange
        var command = new RoomGroupCreateCommand
        {
            Payload = new RoomGroupCreateRequest(
                "Valid Name",
                "Valid Description",
                10,
                5,
                1
            )
        };
        var validator = new RoomGroupCreateCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E1021, validation.Errors.GetErrorCode());
        Assert.Contains("CapacityMax", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupDeleteCommandValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var command = new RoomGroupDeleteCommand { Payload = new RoomGroupDeleteRequest(-1) };
        var validator = new RoomGroupDeleteCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupGetAllPublishedInQueryValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupGetAllPublishedInQuery(-1, PageableBinderConfig.DefaultPageable);

        var validator = new RoomGroupGetAllPublishedInQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupGetBasicConfigurationQueryValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupGetBasicConfigurationQuery(-1);
        var validator = new RoomGroupGetBasicConfigurationQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupGetDisplaySettingQueryValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupGetDisplaySettingQuery(0);
        var validator = new RoomGroupGetDisplaySettingQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupGetPublicationSettingQueryValidator_ShouldThrowError_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupGetDisplaySettingQuery(0);
        var validator = new RoomGroupGetPublicationSettingQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupInventoryAdjustCommandValidator_ShouldThrowError_WhenPayloadIsEmpty()
    {
        // Arrange
        var command = new RoomGroupInventoryAdjustCommand { Payload = new List<RoomGroupChangeRemainRequest>() };
        var validator = new RoomGroupInventoryAdjustCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Payload", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupChangeRemainRequestValidator_ShouldThrowError_WhenAppDateIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new RoomGroupChangeRemainRequest(-1, 1, 1, true);
        var validator = new RoomGroupChangeRemainRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupChangeRemainRequestValidator_ShouldThrowError_WhenAppDateIdIsInvalid()
    {
        // Arrange
        var request = new RoomGroupChangeRemainRequest(111, 1, 1, true);
        var validator = new RoomGroupChangeRemainRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupChangeRemainRequestValidator_ShouldThrowError_WhenRoomGroupIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var request = new RoomGroupChangeRemainRequest(AppDate.GetId(DateTime.Now), -1, 1, true);

        var validator = new RoomGroupChangeRemainRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupInventoryGetAllQueryValidator_ShouldThrowError_WhenEndAppDateIsNotGreaterThanOrEqualToStartAppDate()
    {
        // Arrange
        var query = new RoomGroupInventoryGetAllQuery(
            PageableBinderConfig.DefaultPageable,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(-1))
        );

        var validator = new RoomGroupInventoryGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupInventoryGetAllQueryValidator_ShouldThrowError_WhenStartAppDateIsInvalid()
    {
        // Arrange
        var query = new RoomGroupInventoryGetAllQuery(
            PageableBinderConfig.DefaultPageable,
            1,
            AppDate.GetId(DateTime.Now)
        );

        var validator = new RoomGroupInventoryGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("StartAppDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupInventoryGetAllQueryValidator_ShouldThrowError_WhenEndAppDateIsInvalid()
    {
        // Arrange
        var query = new RoomGroupInventoryGetAllQuery(
            PageableBinderConfig.DefaultPageable,
            AppDate.GetId(DateTime.Now),
            -1
        );

        var validator = new RoomGroupInventoryGetAllQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetChildrenPriceQueryValidator_ShouldThrowError_WhenRoomTypeIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetChildrenPriceQuery(-1, 1);

        var validator = new RoomGroupPriceGetChildrenPriceQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetChildrenPriceQueryValidator_ShouldThrowError_WhenSiteIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetChildrenPriceQuery(1, -1);
        var validator = new RoomGroupPriceGetChildrenPriceQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetDiscountQueryValidator_ShouldThrowError_WhenRoomTypeIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetDiscountQuery(-1, 1);
        var validator = new RoomGroupPriceGetDiscountQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetDiscountQueryValidator_ShouldThrowError_WhenSiteIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetDiscountQuery(1, -1);

        var validator = new RoomGroupPriceGetDiscountQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetPriceCalendarQueryValidator_ShouldThrowError_WhenRoomTypeIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetPriceCalendarQuery(
            -1,
            1,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(1))
        );
        var validator = new RoomGroupPriceGetPriceCalendarQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetPriceCalendarQueryValidator_ShouldThrowError_WhenSiteIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetPriceCalendarQuery(
            1,
            -1,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(1))
        );

        var validator = new RoomGroupPriceGetPriceCalendarQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetPriceCalendarQueryValidator_ShouldThrowError_WhenStartDateIsInvalid()
    {
        // Arrange
        var query = new RoomGroupPriceGetPriceCalendarQuery(
            1,
            1,
            11111,
            AppDate.GetId(DateTime.Now.AddDays(1))
        );

        var validator = new RoomGroupPriceGetPriceCalendarQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Contains("StartDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetPriceCalendarQueryValidator_ShouldThrowError_WhenEndDateIsLessThanOrEqualToStartDate()
    {
        // Arrange
        var query = new RoomGroupPriceGetPriceCalendarQuery(
            1,
            1,
            AppDate.GetId(DateTime.Now.AddDays(2)),
            AppDate.GetId(DateTime.Now.AddDays(1))
        );

        var validator = new RoomGroupPriceGetPriceCalendarQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetPriceCalendarQueryValidator_ShouldThrowError_WhenEndDateIsInvalid()
    {
        // Arrange
        var query = new RoomGroupPriceGetPriceCalendarQuery(
            1,
            1,
            AppDate.GetId(DateTime.Now.AddDays(1)),
            11111111111
        );

        var validator = new RoomGroupPriceGetPriceCalendarQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Contains("EndDate", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetSaleQueryValidator_ShouldThrowError_WhenRoomTypeIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetSaleQuery(-1, 1);
        var validator = new RoomGroupPriceGetSaleQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetSaleQueryValidator_ShouldThrowError_WhenSiteIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new RoomGroupPriceGetSaleQuery(1, -1);
        var validator = new RoomGroupPriceGetSaleQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetStandardPriceQueryValidator_ShouldThrowError_WhenRoomTypeIdIsNegative()
    {
        // Arrange
        var query = new RoomGroupPriceGetStandardPriceQuery(-1, 1);
        var validator = new RoomGroupPriceGetStandardPriceQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceGetStandardPriceQueryValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var query = new RoomGroupPriceGetStandardPriceQuery(1, -1);

        var validator = new RoomGroupPriceGetStandardPriceQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenRoomTypeIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateChildrenPriceCommand(-1, 1)
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new(1, true, true, PriceSettingTypes.Percent, 200)
                ]
            )
        };

        var validator = new RoomGroupPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateChildrenPriceCommand(1, -1)
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new(1, true, true, PriceSettingTypes.Percent, 200)
                ]
            )
        };

        var validator = new RoomGroupPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenPersonAgeTypeIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateChildrenPriceCommand(1, 1)
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new(-1, true, true, PriceSettingTypes.Percent, 200)
                ]
            )
        };

        var validator = new RoomGroupPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("PersonAgeTypes", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateChildrenPriceCommandValidator_ShouldThrowError_WhenValueIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateChildrenPriceCommand(1, 1)
        {
            Payload = new RoomGroupPriceUpdateChildrenPriceRequest
            (
                [
                    new(1, true, true, PriceSettingTypes.Percent, -200)
                ]
            )
        };

        var validator = new RoomGroupPriceUpdateChildrenPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("PersonAgeTypes", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenRoomTypeIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateDiscountCommand(-1, 1) { Payload = new RoomGroupPriceUpdateDiscountRequest([]) };

        var validator = new RoomGroupPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenRoomTypeIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateDiscountCommand(0, 1) { Payload = new RoomGroupPriceUpdateDiscountRequest([]) };

        var validator = new RoomGroupPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateDiscountCommand(1, -1) { Payload = new RoomGroupPriceUpdateDiscountRequest([]) };

        var validator = new RoomGroupPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateDiscountCommandValidator_ShouldThrowError_WhenSiteIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateDiscountCommand(1, 0) { Payload = new RoomGroupPriceUpdateDiscountRequest([]) };

        var validator = new RoomGroupPriceUpdateDiscountCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task RoomGroupPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenRoomTypeIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdatePriceCalendarCommand(-1, 1) { Payload = new RoomGroupPriceUpdatePriceCalendarRequest([]) };

        var validator = new RoomGroupPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenRoomTypeIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdatePriceCalendarCommand(0, 1) { Payload = new RoomGroupPriceUpdatePriceCalendarRequest([]) };

        var validator = new RoomGroupPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdatePriceCalendarCommand(1, -1) { Payload = new RoomGroupPriceUpdatePriceCalendarRequest([]) };

        var validator = new RoomGroupPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdatePriceCalendarCommandValidator_ShouldThrowError_WhenSiteIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdatePriceCalendarCommand(1, 0) { Payload = new RoomGroupPriceUpdatePriceCalendarRequest([]) };

        var validator = new RoomGroupPriceUpdatePriceCalendarCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateSaleCommandValidator_ShouldThrowError_WhenRoomTypeIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateSaleCommand(-1, 1) { Payload = new RoomGroupPriceUpdateSaleRequest() };

        var validator = new RoomGroupPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateSaleCommandValidator_ShouldThrowError_WhenRoomTypeIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateSaleCommand(0, 1) { Payload = new RoomGroupPriceUpdateSaleRequest() };

        var validator = new RoomGroupPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateSaleCommandValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateSaleCommand(1, -1) { Payload = new RoomGroupPriceUpdateSaleRequest() };

        var validator = new RoomGroupPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateSaleCommandValidator_ShouldThrowError_WhenSiteIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateSaleCommand(1, 0) { Payload = new RoomGroupPriceUpdateSaleRequest() };

        var validator = new RoomGroupPriceUpdateSaleCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenRoomTypeIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateStandardPriceCommand(-1, 1) { Payload = new RoomGroupPriceUpdateStandardPriceRequest([]) };

        var validator = new RoomGroupPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenRoomTypeIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateStandardPriceCommand(0, 1) { Payload = new RoomGroupPriceUpdateStandardPriceRequest([]) };

        var validator = new RoomGroupPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomTypeId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenSiteIdIsNegative()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateStandardPriceCommand(1, -1) { Payload = new RoomGroupPriceUpdateStandardPriceRequest([]) };

        var validator = new RoomGroupPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupPriceUpdateStandardPriceCommandValidator_ShouldThrowError_WhenSiteIdIsZero()
    {
        // Arrange
        var command = new RoomGroupPriceUpdateStandardPriceCommand(1, 0) { Payload = new RoomGroupPriceUpdateStandardPriceRequest([]) };

        var validator = new RoomGroupPriceUpdateStandardPriceCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [
                    new(1, 2)
                ],
                true,
                true,
                true,
                true,
                true,
                [
                    new(1, 0)
                ]
            )
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenIdIsZero()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [
                    new(1, 2)
                ],
                true,
                true,
                true,
                true,
                true,
                [
                    new(1, 0)
                ]
            ) { Id = 0 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                string.Empty, // Name is empty
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                new List<BedTypeOfRoomGroupUpdateBasicConfigurationRequest> { new(1, 2) },
                true,
                true,
                true,
                true,
                true,
                new List<FileOfRoomUpdateBasicSettingRequest> { new(1, 0) }
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenNameIsTooLong()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                new string('a', 256), // Name exceeds max length
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(1, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenGroupNameIsEmpty()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                string.Empty, // GroupName is empty
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(1, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("GroupName", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenBaseNumberIsNegative()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                -1, // BaseNumber is negative
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(1, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("BaseNumber", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenCapacityMinIsNegative()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                -1, // CapacityMin is negative
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(1, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("CapacityMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenCapacityMaxIsLessThanCapacityMin()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                0, // CapacityMax is less than CapacityMin
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(1, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E1021, validation.Errors.GetErrorCode());
        Assert.Contains("CapacityMax", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenSizeIsNegative()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                -1, // Size is negative
                RoomGroupSizeUnitTypes.M2,
                [new(1, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("Size", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenBedTypesIsEmpty()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [], // BedTypes is empty
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("BedTypes", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenBedTypeIdIsZero()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(0, 2)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateBasicConfigurationCommandValidator_ShouldThrowError_WhenBedTypeNumberIsNegative()
    {
        // Arrange
        var command = new RoomGroupUpdateBasicConfigurationCommand
        {
            Payload = new RoomGroupUpdateBasicConfigurationRequest(
                "Room Group",
                "Group Name",
                "Overview text",
                "Summary text",
                "Description text",
                1,
                2,
                1,
                100,
                RoomGroupSizeUnitTypes.M2,
                [new(1, -1)],
                true,
                true,
                true,
                true,
                true,
                [new(1, 0)]
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateBasicConfigurationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("Number", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateDisplaySettingCommandValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var command = new RoomGroupUpdateDisplaySettingCommand
        {
            Payload = new RoomGroupUpdateDisplaySettingRequest(
                [1, 2],
                [3],
                [4],
                [5],
                [6],
                []
            )
        };

        var validator = new RoomGroupUpdateDisplaySettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateDisplaySettingCommandValidator_ShouldThrowError_WhenRoomGroupMasterCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new RoomGroupUpdateDisplaySettingCommand
        {
            Payload = new RoomGroupUpdateDisplaySettingRequest(
                [0, -1], // Invalid values
                [3],
                [4],
                [5],
                [6],
                []
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateDisplaySettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupMasterCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateDisplaySettingCommandValidator_ShouldThrowError_WhenRoomGroupCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new RoomGroupUpdateDisplaySettingCommand
        {
            Payload = new RoomGroupUpdateDisplaySettingRequest(
                [1, 2],
                [0, -1], // Invalid values
                [4],
                [5],
                [6],
                []
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateDisplaySettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateDisplaySettingCommandValidator_ShouldThrowError_WhenRoomGroupFeatureCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new RoomGroupUpdateDisplaySettingCommand
        {
            Payload = new RoomGroupUpdateDisplaySettingRequest(
                [1, 2],
                [3],
                [0, -1], // Invalid values
                [5],
                [6],
                []
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateDisplaySettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupFeatureCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateDisplaySettingCommandValidator_ShouldThrowError_WhenRoomGroupEquipmentCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new RoomGroupUpdateDisplaySettingCommand
        {
            Payload = new RoomGroupUpdateDisplaySettingRequest(
                [1, 2],
                [3],
                [4],
                [0, -1], // Invalid values
                [6],
                []
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateDisplaySettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupEquipmentCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdateDisplaySettingCommandValidator_ShouldThrowError_WhenRoomAmenityCategoryIdsContainInvalidValue()
    {
        // Arrange
        var command = new RoomGroupUpdateDisplaySettingCommand
        {
            Payload = new RoomGroupUpdateDisplaySettingRequest(
                [1, 2],
                [3],
                [4],
                [5],
                [0, -1], // Invalid values
                []
            ) { Id = 1 }
        };

        var validator = new RoomGroupUpdateDisplaySettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomAmenityCategoryIds", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdatePublicationSettingCommandValidator_ShouldThrowError_WhenIdIsNullOrLessThanOrEqualToZero()
    {
        // Arrange
        var command = new RoomGroupUpdatePublicationSettingCommand
        {
            Payload = new RoomGroupUpdatePublicationSettingRequest(
                [1],
                true,
                AppDate.GetId(DateTime.Now),
                AppDate.GetId(DateTime.Now.AddDays(1)),
                true,
                AppDate.GetId(DateTime.Now.AddDays(10)),
                AppDate.GetId(DateTime.Now.AddDays(12)),
                10,
                10,
                PlanAcceptEndLimitTypes.AfterDays,
                10,
                TimeSpan.FromSeconds(30)
            )
        };
        var validator = new RoomGroupUpdatePublicationSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdatePublicationSettingCommandValidator_ShouldThrowError_WhenRoomGroupSiteIdsIsNull()
    {
        // Arrange
        var command = new RoomGroupUpdatePublicationSettingCommand
        {
            Payload = new RoomGroupUpdatePublicationSettingRequest(
                null,
                true,
                AppDate.GetId(DateTime.Now),
                AppDate.GetId(DateTime.Now.AddDays(1)),
                true,
                AppDate.GetId(DateTime.Now.AddDays(10)),
                AppDate.GetId(DateTime.Now.AddDays(12)),
                10,
                10,
                PlanAcceptEndLimitTypes.AfterDays,
                10,
                TimeSpan.FromSeconds(30)
            ) { Id = 1 }
        };
        var validator = new RoomGroupUpdatePublicationSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("SiteId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdatePublicationSettingCommandValidator_ShouldThrowError_WhenReceptionLimitIsInvalid()
    {
        // Arrange
        var command = new RoomGroupUpdatePublicationSettingCommand
        {
            Payload = new RoomGroupUpdatePublicationSettingRequest(
                [1],
                true,
                AppDate.GetId(DateTime.Now),
                AppDate.GetId(DateTime.Now.AddDays(1)),
                true,
                AppDate.GetId(DateTime.Now.AddDays(10)),
                AppDate.GetId(DateTime.Now.AddDays(12)),
                10,
                10,
                PlanAcceptEndLimitTypes.AfterDays,
                10,
                TimeSpan.FromMinutes(-10)
            ) { Id = 1 }
        };
        var validator = new RoomGroupUpdatePublicationSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0012, validation.Errors.GetErrorCode());
        Assert.Contains("ReceptionLimit", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdatePublicationSettingCommandValidator_ShouldThrowError_WhenDisplayDateEndIsLessThanDisplayDateStart()
    {
        // Arrange
        var command = new RoomGroupUpdatePublicationSettingCommand
        {
            Payload = new RoomGroupUpdatePublicationSettingRequest(
                [1],
                true,
                AppDate.GetId(DateTime.Now.AddDays(2)),
                AppDate.GetId(DateTime.Now.AddDays(1)),
                true,
                AppDate.GetId(DateTime.Now.AddDays(4)),
                AppDate.GetId(DateTime.Now.AddDays(10)),
                10,
                10,
                PlanAcceptEndLimitTypes.AfterDays,
                10,
                TimeSpan.FromSeconds(30)
            )
            {
                UseDisplayDate = true,
                Id = 1
            }
        };
        var validator = new RoomGroupUpdatePublicationSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("DisplayDateEnd", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task RoomGroupUpdatePublicationSettingCommandValidator_ShouldThrowError_WhenAcceptDateEndIsLessThanAcceptDateStart()
    {
        // Arrange
        var command = new RoomGroupUpdatePublicationSettingCommand
        {
            Payload = new RoomGroupUpdatePublicationSettingRequest(
                [1],
                true,
                AppDate.GetId(DateTime.Now),
                AppDate.GetId(DateTime.Now.AddDays(1)),
                true,
                AppDate.GetId(DateTime.Now.AddDays(10)),
                AppDate.GetId(DateTime.Now.AddDays(9)),
                10,
                10,
                PlanAcceptEndLimitTypes.AfterDays,
                10,
                TimeSpan.FromSeconds(30)
            )
            {
                UseAcceptDate = true,
                Id = 1
            }
        };
        var validator = new RoomGroupUpdatePublicationSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("AcceptDateEnd", validation.Errors.GetErrorField());
    }
}
