using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class AlertMessageEndpointValidationUnitTest
{
    [Fact]
    public async Task UpdateAlertMessage_ShouldReturnValidationError_WhenIdIsNotGreaterThanZero()
    {
        var request = new AlertMessageUpdateRequest(
            null,
            "",
            null,
            null,
            true
        ) { Id = -1 };

        var command = new AlertMessageUpdateCommand { Payload = request };

        var validation = await new AlertMessageUpdateCommandValidator().ValidateAsync(command);

        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAlertMessage_ShouldReturnValidationError_WhenTitleExceedsMaxLength()
    {
        var request = new AlertMessageUpdateRequest(
            new string('a', 256),
            "",
            null,
            null,
            true
        ) { Id = 1 };

        var command = new AlertMessageUpdateCommand { Payload = request };

        var validation = await new AlertMessageUpdateCommandValidator().ValidateAsync(command);

        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Title", validation.GetErrorField());
    }

    [Fact]
    public async Task UpdateAlertMessage_ShouldReturnValidationSuccess()
    {
        var request = new AlertMessageUpdateRequest(
            "title test",
            "content test",
            null,
            null,
            true
        ) { Id = 1 };

        var command = new AlertMessageUpdateCommand { Payload = request };

        var validation = await new AlertMessageUpdateCommandValidator().ValidateAsync(command);

        Assert.True(validation.IsValid);
    }
}
