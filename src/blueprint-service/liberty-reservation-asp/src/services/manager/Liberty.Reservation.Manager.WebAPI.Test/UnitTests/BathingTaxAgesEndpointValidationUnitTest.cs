using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class BathingTaxAgesEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest(string.Empty, 99, 10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest(new string('A', 256), 99, 10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenAgeMaxIsLessThanZero()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest("Valid Name", -1, 10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Contains("AgeMax", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenAgeMinIsLessThanZero()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest("Valid Name", 1, -10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Contains("AgeMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenSpasCountExceedsFour()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550),
            new(200, 3000, 150),
            new(150, 5000, 250),
            new(500, 7000, 500)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest("Valid Name", 18, 10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Contains("Spas", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenSpasPriceMinIsDuplicate()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(100, 3000, 150)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest("Valid Name", 18, 10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Contains("Spas", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeDetailsCreateRequestValidator_ShouldThrowError_WhenSpasPriceMaxIsDuplicate()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(200, 5000, 150),
            new(300, 5000, 150)
        };
        var bathingTaxAgeAddRequest = new BathingTaxAgeCreateRequest("Valid Name", 18, 10, null, false, meta, spas);
        var validator = new BathingTaxAgeDetailsCreateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeAddRequest);

        // Assert
        Assert.Contains("Spas", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeChangeStatusRequestValidator_ShouldThrowError_WhenIdIsZeroOrNull()
    {
        // Arrange
        var request = new BathingTaxAgeChangeStatusRequest(
            0,
            true
        );

        var validator = new BathingTaxAgeChangeStatusRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeChangeStatusRequestValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var request = new BathingTaxAgeChangeStatusRequest(
            -1,
            true
        );

        var validator = new BathingTaxAgeChangeStatusRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, string.Empty, 99, 10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, new string('A', 256), 99, 10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Name", validation.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenAgeMaxIsLessThanZero()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, "Valid Name", -1, 10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Contains("AgeMax", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenAgeMinIsLessThanZero()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, "Valid Name", 1, -10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Contains("AgeMin", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenSpasCountExceedsFour()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(280, 6540, 550),
            new(200, 3000, 150),
            new(150, 5000, 250),
            new(500, 7000, 500)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, "Valid Name", 18, 10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Contains("Spas", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenSpasPriceMinIsDuplicate()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(100, 3000, 150)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, "Valid Name", 18, 10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Contains("Spas", validation.Errors.GetErrorField());
    }

    [Fact]
    public async Task BathingTaxAgeUpdateRequestValidator_ShouldThrowError_WhenSpasPriceMaxIsDuplicate()
    {
        // Arrange
        var meta = new MetaOfBathingTaxAgeUpdateRequest("Test", FoodBeds.Food, FoodBeds.Bed, PersonAgeGroups.Child);
        var spas = new List<SpaOfBathingTaxAgeUpdateRequest>
        {
            new(100, 1000, 50),
            new(200, 5000, 150),
            new(300, 5000, 150)
        };
        var bathingTaxAgeUpdateRequest = new BathingTaxAgeUpdateRequest(1, "Valid Name", 18, 10, false, meta, spas);
        var validator = new BathingTaxAgeDetailsUpdateRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(bathingTaxAgeUpdateRequest);

        // Assert
        Assert.Contains("Spas", validation.Errors.GetErrorField());
    }
}
