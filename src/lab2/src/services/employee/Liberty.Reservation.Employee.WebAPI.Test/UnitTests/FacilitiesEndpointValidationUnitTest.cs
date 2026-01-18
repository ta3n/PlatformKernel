using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class FacilitiesEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task FacilityGetAllDestinationsQuery_ShouldReturnValidationError_WhenFacilityIdIsEmpty()
    {
        // Arrange
        var mockPageable = PageableBinderConfig.DefaultPageable;
        var query = new FacilityGetAllDestinationsQuery(mockPageable) { FacilityId = 0 };

        // Act
        var validation = await new FacilityGetAllDestinationsQueryValidator().ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("FacilityId", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityGetAllDestinationsQuery_ShouldReturnValidationError_WhenFacilityIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var mockPageable = PageableBinderConfig.DefaultPageable;
        var query = new FacilityGetAllDestinationsQuery(mockPageable) { FacilityId = -1 };

        // Act
        var validation = await new FacilityGetAllDestinationsQueryValidator().ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Equal("FacilityId", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateCommand_ShouldReturnValidationError_WhenIdIsNullOrEmpty()
    {
        // Arrange
        var command = new FacilityUpdateCommand { Payload = new FacilityUpdateRequest([1, 2, 3], true, "Facility 1", "Description 1") };

        // Act
        var validation = await new FacilityUpdateCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task FacilityUpdateCommand_ShouldReturnValidationError_WhenSystemEMailIsInvalid()
    {
        // Arrange
        var command = new FacilityUpdateCommand { Payload = new FacilityUpdateRequest([1, 2, 3], true, "invalid", "Memo") { Id = 1 } };

        // Act
        var validation = await new FacilityUpdateCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0006, validation.Errors.GetErrorCode());
        Assert.Equal("SystemEMail", validation.GetErrorField());
    }
}
