using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class SalesEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task SaleGetAllReservationsQueryValidator_ShouldThrowError_WhenStartAppDateIdIsNotAValidDate()
    {
        // Arrange
        var command = new SaleGetAllReservationsQuery(
            0,
            AppDate.GetId(DateTime.Now.AddDays(1)),
            PageableBinderConfig.DefaultPageable
        );

        var validator = new SaleGetAllReservationsQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("StartAppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task SaleGetAllReservationsQueryValidator_ShouldThrowError_WhenEndAppDateIdIsInvalid()
    {
        // Arrange
        var command = new SaleGetAllReservationsQuery(
            AppDate.GetId(DateTime.Now),
            111,
            PageableBinderConfig.DefaultPageable
        );

        var validator = new SaleGetAllReservationsQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task SaleGetAllReservationsQueryValidator_ShouldThrowError_WhenEndAppDateIdIsLessThanStartAppDateId()
    {
        // Arrange
        var command = new SaleGetAllReservationsQuery(
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(-1)),
            PageableBinderConfig.DefaultPageable
        );

        var validator = new SaleGetAllReservationsQueryValidator();

        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task SalesGetOverviewQueryValidator_ShouldThrowError_WhenStartAppDateIdIsNotAValidDate()
    {
        // Arrange
        var command = new SaleGetReservationOverviewQuery(
            0, // Invalid date ID
            AppDate.GetId(DateTime.Now.AddDays(1))
        );
        var validator = new SalesGetOverviewQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("StartAppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task SalesGetOverviewQueryValidator_ShouldThrowError_WhenEndAppDateIdIsInvalid()
    {
        // Arrange
        var command = new SaleGetReservationOverviewQuery(
            AppDate.GetId(DateTime.Now),
            111 // Invalid EndAppDateId
        );
        var validator = new SalesGetOverviewQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDateId", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task SalesGetOverviewQueryValidator_ShouldThrowError_WhenEndAppDateIdIsLessThanStartAppDateId()
    {
        // Arrange
        var command = new SaleGetReservationOverviewQuery(
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(-1))
        );

        var validator = new SalesGetOverviewQueryValidator();
        // Act
        var validation = await validator.ValidateAsync(command);

        // Assert
        Assert.Equal(ErrorCode.E0009, validation.Errors.GetErrorCode());
        Assert.Contains("EndAppDateId", validation.Errors.GetErrorField());
    }
}
