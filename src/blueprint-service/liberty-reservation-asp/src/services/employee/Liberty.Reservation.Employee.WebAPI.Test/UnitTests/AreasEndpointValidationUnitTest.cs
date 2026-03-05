using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Area;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class AreasEndpointValidationUnitTest : BaseUnitTest
{
    private IAreaService MockAreaService { get; set; } = null!;

    [Fact]
    public async Task CreateArea_ShouldReturnValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var areaCreateRequest = new AreaCreateRequest(string.Empty);

        // Act
        var validation = await new AreaCreateRequestValidator().ValidateAsync(
            areaCreateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task CreateArea_ShouldReturnValidationError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var areaCreateRequest = new AreaCreateRequest(new string('a', 251));

        // Act
        var validation = await new AreaCreateRequestValidator().ValidateAsync(
            areaCreateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateArea_ShouldReturnValidationError_WhenIdIsLessThanOrEqualZero()
    {
        // Arrange
        var areaUpdateRequest = new AreaUpdateRequest("Valid Area Name") { Id = -1 };

        // Act
        var validation = await new AreaUpdateRequestValidator().ValidateAsync(
            areaUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateArea_ShouldReturnValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var areaUpdateRequest = new AreaUpdateRequest(string.Empty) { Id = 1 };

        // Act
        var validation = await new AreaUpdateRequestValidator().ValidateAsync(
            areaUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateArea_ShouldReturnValidationError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var areaUpdateRequest = new AreaUpdateRequest(new string('a', 251)) { Id = 1 };

        // Act
        var validation = await new AreaUpdateRequestValidator().ValidateAsync(
            areaUpdateRequest
        );

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task EnableArea_ShouldThrowError_WhenIdIsLessThanOrEqualZero()
    {
        // Arrange
        var areaEnabledRequest = new AreaEnabledRequest(true);
        var mockMapper = MockServices.MockMapper();
        _ = new AreasEndpoint(mockMapper, MockMediator, MockAreaService);
        _ = CancellationToken.None;

        // Act
        var validation = await new AreaEnableCommandValidator().ValidateAsync(
            new AreaEnableCommand { Payload = areaEnabledRequest }
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }
}
