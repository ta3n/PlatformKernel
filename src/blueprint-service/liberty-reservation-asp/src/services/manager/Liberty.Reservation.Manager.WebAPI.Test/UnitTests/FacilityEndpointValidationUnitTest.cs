using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class FacilityEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task FacilityUpdateAccessCommandValidator_ShouldThrowError_WhenAccessInfoCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateAccessCommand
        {
            Payload = new FacilityUpdateAccessRequest(
                10.0f,
                20.0f,
                new string('A', 1001),
                true,
                "Valid Comment",
                true,
                "Valid Comment",
                "Valid Comment"
            )
        };
        var validator = new FacilityUpdateAccessCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("AccessInfoComment", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateAccessCommandValidator_ShouldThrowError_WhenParkingInfoCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateAccessCommand
        {
            Payload = new FacilityUpdateAccessRequest(
                10.0f,
                20.0f,
                "Valid Comment",
                true,
                new string('A', 1001),
                true,
                "Valid Comment",
                "Valid Comment"
            )
        };
        var validator = new FacilityUpdateAccessCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("ParkingInfoComment", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateAccessCommandValidator_ShouldThrowError_WhenTransferCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateAccessCommand
        {
            Payload = new FacilityUpdateAccessRequest(
                10.0f,
                20.0f,
                "Valid Comment",
                true,
                "Valid Comment",
                true,
                new string('A', 1001),
                "Valid Comment"
            )
        };
        var validator = new FacilityUpdateAccessCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("TransferComment", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateAccessCommandValidator_ShouldThrowError_WhenNearStationInfoCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateAccessCommand
        {
            Payload = new FacilityUpdateAccessRequest(
                10.0f,
                20.0f,
                "Valid Comment",
                true,
                "Valid Comment",
                true,
                "Valid Comment",
                new string('A', 1001)
            )
        };
        var validator = new FacilityUpdateAccessCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("NearStationInfoComment", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShoulThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                new string('a', 10001),
                "123456789",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Description", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenFaxExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                new string('1', 251),
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Fax", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenUrlIsInvalid()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "invalid-url",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0005, validation.Errors.GetErrorCode());
        Assert.Contains("Url", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenAreaIdIsInvalid()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                0,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AreaId", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenRoomNumberWesternStyleExceedsLimit()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                1000,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0004, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumberWesternStyle", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                new string('a', 251),
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenKanaExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                new string('a', 251),
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenPostcodeExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                new string('1', 251),
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Postcode", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenAddress1ExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                new string('a', 251),
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Address1", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenRoomNumberJapaneseStyleIsNegative()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                -1,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumberJapaneseStyle", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenRoomNumberJapaneseWesternStyleExceedsLimit()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                1000,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0004, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumberJapaneseWesternStyle", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenPayloadIsValid()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "123456789",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.True(validation.IsValid);
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenRoomNumberOtherStyleIsNegative()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                -1,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumberOtherStyle", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenFacilityTypeIdIsNegative()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                5,
                5,
                1,
                -1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("FacilityTypeId", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenRoomNumberWesternStyleIsNegative()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                -1,
                5,
                5,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumberWesternStyle", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateBasicSettingCommandValidator_ShouldThrowError_WhenRoomNumberJapaneseWesternStyleIsNegative()
    {
        // Arrange
        var command = new FacilityUpdateBasicSettingCommand
        {
            Payload = new FacilityUpdateBasicSettingRequest(
                "Valid description",
                "Valid fax",
                "https://validurl.com",
                "Valid name",
                "Valid kana",
                "123-4567",
                "Address line 1",
                "Address line 2",
                "Address line 3",
                "Address line 4",
                "0123456789",
                5,
                5,
                -1,
                5,
                1,
                1,
                null
            )
        };
        var validator = new FacilityUpdateBasicSettingCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumberJapaneseWesternStyle", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenAllergensContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [-1],
                [],
                [],
                [],
                [],
                [],
                [],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Allergens", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenFeaturesContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [-1],
                [],
                [],
                [],
                [],
                [],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Features", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenEquipmentsContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [],
                [-1],
                [],
                [],
                [],
                [],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Equipments", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenServicesContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [],
                [],
                [-1],
                [],
                [],
                [],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Services", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenBathsContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [],
                [],
                [],
                [-1],
                [],
                [],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Baths", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenSceneriesContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [],
                [],
                [],
                [],
                [-1],
                [],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Sceneries", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenAmenitiesContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [],
                [],
                [],
                [],
                [],
                [-1],
                []
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Amenities", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateClassificationCommandValidator_ShouldThrowError_WhenMealsContainsInvalidValue()
    {
        // Arrange
        var command = new FacilityUpdateClassificationCommand
        {
            Payload = new FacilityUpdateClassificationRequest(
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                [-1]
            )
        };
        var validator = new FacilityUpdateClassificationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Meals", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdatePaymentMethodCommandValidator_ShouldThrowError_WhenOnLinePaymentCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdatePaymentMethodCommand
        {
            Payload = new FacilityUpdatePaymentMethodRequest(
                true,
                true,
                "Valid Comment",
                new string('B', 10001),
                "Valid Comment"
            )
        };
        var validator = new FacilityUpdatePaymentMethodCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.False(validation.IsValid);
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("OnLinePaymentComment", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdatePaymentMethodCommandValidator_ShouldThrowError_WhenOnSidePaymentCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdatePaymentMethodCommand
        {
            Payload = new FacilityUpdatePaymentMethodRequest(
                true,
                true,
                new string('A', 10001),
                "Valid Comment",
                "Valid Comment"
            )
        };
        var validator = new FacilityUpdatePaymentMethodCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("OnSidePaymentComment", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdatePaymentMethodCommandValidator_ShouldThrowError_WhenPaymentCommentExceedsMaxLength()
    {
        // Arrange
        var command = new FacilityUpdatePaymentMethodCommand
        {
            Payload = new FacilityUpdatePaymentMethodRequest(
                true,
                true,
                "Valid Comment",
                "Valid Comment",
                new string('C', 10001)
            )
        };
        var validator = new FacilityUpdatePaymentMethodCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("PaymentComment", validation.Errors.GetErrorField());
    }
}
