using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class MailTemplatesEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task MailTemplateGetQuery_ShouldReturnValidationError_WhenIoTypeIsEmpty()
    {
        // Arrange
        var query = new MailTemplateGetQuery(string.Empty);

        // Act
        var validation = await new MailTemplateGetQueryValidator().ValidateAsync(query);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplatePreviewRequest_ShouldReturnValidationError_WhenFormatOverMaximumLength()
    {
        // Arrange
        var request = new MailTemplatePreviewRequest(new string('a', 5001));

        // Act
        var validation = await new MailTemplatePreviewRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Format", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplatePreviewRequest_ShouldReturnValidationError_WhenIoTypeIsNull()
    {
        // Arrange
        var request = new MailTemplatePreviewRequest("Format");

        // Act
        var validation = await new MailTemplatePreviewRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplatePreviewRequest_ShouldReturnValidationError_WhenIoTypeOverMaximumLength()
    {
        // Arrange
        var request = new MailTemplatePreviewRequest("Format") { IoType = new string('a', 21) };

        // Act
        var validation = await new MailTemplatePreviewRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenSubjectIsEmpty()
    {
        // Arrange
        var request = new MailTemplateSendRequest(string.Empty, "Body", "email@test.com");

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Subject", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenSubjectIsTooLong()
    {
        // Arrange
        var request = new MailTemplateSendRequest(new string('a', 201), "Body", "email@test.com");

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Subject", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenBodyIsEmpty()
    {
        // Arrange
        var request = new MailTemplateSendRequest("Subject", string.Empty, "email@test.com");

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Body", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenBodyIsTooLong()
    {
        // Arrange
        var request = new MailTemplateSendRequest("Subject", new string('a', 5001), "email@test.com");

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Body", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenToEmailIsEmpty()
    {
        // Arrange
        var request = new MailTemplateSendRequest("Subject", "Body", string.Empty);

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("ToEmail", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenToEmailIsInvalid()
    {
        // Arrange
        var request = new MailTemplateSendRequest("Subject", "Body", "invalidEmail");

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0006, validation.Errors.GetErrorCode());
        Assert.Equal("ToEmail", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateSendRequest_ShouldReturnValidationError_WhenToEmailIsValid()
    {
        // Arrange
        var request = new MailTemplateSendRequest("Subject", "Body", "email@test.com");

        // Act
        var validation = await new MailTemplateSendRequestValidator().ValidateAsync(request);

        // Assert
        Assert.True(validation.IsValid);
    }

    [Fact]
    public async Task MailTemplateUpdateCommand_ShouldReturnValidationError_WhenIoTypeIsNull()
    {
        // Arrange
        var command = new MailTemplateUpdateCommand
        {
            Payload = new MailTemplateUpdateRequest("Valid format")
            {
                IoType = null,
                Format = string.Empty
            }
        };

        // Act
        var validation = await new MailTemplateUpdateCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateCommand_ShouldReturnValidationError_WhenIoTypeIsEmpty()
    {
        // Arrange
        var command = new MailTemplateUpdateCommand { Payload = new MailTemplateUpdateRequest("Valid format") { IoType = string.Empty } };

        // Act
        var validation = await new MailTemplateUpdateCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateCommand_ShouldReturnValidationError_WhenFormatIsEmpty()
    {
        // Arrange
        var command = new MailTemplateUpdateCommand { Payload = new MailTemplateUpdateRequest(string.Empty) { IoType = "IoType" } };

        // Act
        var validation = await new MailTemplateUpdateCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Format", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateCommand_ShouldReturnValidationError_WhenFormatIsTooLong()
    {
        // Arrange
        var command = new MailTemplateUpdateCommand
        {
            Payload = new MailTemplateUpdateRequest(new string('a', 5001)) { IoType = "ValidType" }
        };

        // Act
        var validation = await new MailTemplateUpdateCommandValidator().ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Format", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateRequest_ShouldReturnValidationError_WhenIoTypeIsNull()
    {
        // Arrange
        var request = new MailTemplateUpdateRequest("Format") { IoType = null };

        // Act
        var validation = await new MailTemplateUpdateRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateRequest_ShouldReturnValidationError_WhenIoTypeIsEmpty()
    {
        // Arrange
        var request = new MailTemplateUpdateRequest("Format") { IoType = string.Empty };

        // Act
        var validation = await new MailTemplateUpdateRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("IoType", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateRequest_ShouldReturnValidationError_WhenFormatIsEmpty()
    {
        // Arrange
        var request = new MailTemplateUpdateRequest(string.Empty) { IoType = "Type" };

        // Act
        var validation = await new MailTemplateUpdateRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Format", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateRequest_ShouldReturnValidationError_WhenFormatIsInvalidJson()
    {
        // Arrange
        var request = new MailTemplateUpdateRequest("invalid json") { IoType = "Type" };

        // Act
        var validation = await new MailTemplateUpdateRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0007, validation.Errors.GetErrorCode());
        Assert.Equal("Format", validation.GetErrorField());
    }

    [Fact]
    public async Task MailTemplateUpdateRequest_ShouldReturnValidationError_WhenFormatIsTooLong()
    {
        // Arrange
        var jsonString = new string('a', 4999);
        var request = new MailTemplateUpdateRequest($"{{ \"valid\": \"{jsonString}\" }}") { IoType = "ValidType" };

        // Act
        var validation = await new MailTemplateUpdateRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Equal("Format", validation.GetErrorField());
    }
}
