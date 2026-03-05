using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Category;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.SysException;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class CategoriesEndpointValidationUnitTest : BaseUnitTest
{
    private ICategoryService MockCategoryService { get; set; } = null!;

    [Fact]
    public async Task CreateFacilityCategory_ShouldThrowError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryCreateRequest = new CategoryCreateRequest(string.Empty, "Des");
        var mockMapper = MockServices.MockMapper(
            (categoryCreateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.CreateCategory(categoryCreateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0001, exception.ErrorCode);
        Assert.Equal("Name", exception.ErrorField);
    }

    [Fact]
    public async Task CreateFacilityCategory_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryCreateRequest = new CategoryCreateRequest(new string('a', 251), "Des");
        var mockMapper = MockServices.MockMapper(
            (categoryCreateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.CreateCategory(categoryCreateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0002, exception.ErrorCode);
        Assert.Equal("Name", exception.ErrorField);
    }

    [Fact]
    public async Task CreateFacilityCategory_ShouldThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryCreateRequest = new CategoryCreateRequest("Name", new string('a', 501));
        var mockMapper = MockServices.MockMapper(
            (categoryCreateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.CreateCategory(categoryCreateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0002, exception.ErrorCode);
        Assert.Equal("Description", exception.ErrorField);
    }

    [Fact]
    public async Task UpdateCategory_ShouldThrowError_WhenIdIsNullOrInvalid()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryUpdateRequest = new CategoryUpdateRequest("TestName", "TestDes");
        var mockMapper = MockServices.MockMapper(
            (categoryUpdateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.UpdateCategory(-1, categoryUpdateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0001, exception.ErrorCode);
        Assert.Equal("Id", exception.ErrorField);
    }

    [Fact]
    public async Task UpdateCategory_ShouldThrowError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryUpdateRequest = new CategoryUpdateRequest(string.Empty, "TestDes");
        var mockMapper = MockServices.MockMapper(
            (categoryUpdateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.UpdateCategory(1, categoryUpdateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0001, exception.ErrorCode);
        Assert.Equal("Name", exception.ErrorField);
    }

    [Fact]
    public async Task UpdateCategory_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryUpdateRequest = new CategoryUpdateRequest(new string('a', 251), "TestDes");
        var mockMapper = MockServices.MockMapper(
            (categoryUpdateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.UpdateCategory(1, categoryUpdateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0002, exception.ErrorCode);
        Assert.Equal("Name", exception.ErrorField);
    }

    [Fact]
    public async Task UpdateCategory_ShouldThrowError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryUpdateRequest = new CategoryUpdateRequest("Name", new string('a', 501));
        var mockMapper = MockServices.MockMapper(
            (categoryUpdateRequest, category)
        );
        var controller = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.UpdateCategory(1, categoryUpdateRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0002, exception.ErrorCode);
        Assert.Equal("Description", exception.ErrorField);
    }

    [Fact]
    public async Task EnableCategory_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var categoryEnabledRequest = new CategoryEnabledRequest(true);
        var mockMapper = MockServices.MockMapper();
        _ = new FacilityCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        _ = CancellationToken.None;

        // Act
        var validation = await new CategoryEnableCommandValidator().ValidateAsync(
            new CategoryEnableCommand { Payload = categoryEnabledRequest }
        );

        Assert.Equal(ErrorCode.E0001, validation.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }
}
