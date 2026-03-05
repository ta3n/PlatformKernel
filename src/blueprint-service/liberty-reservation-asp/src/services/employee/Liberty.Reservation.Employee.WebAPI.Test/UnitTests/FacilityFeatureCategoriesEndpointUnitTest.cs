using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException.Exceptions;
using Liberty.SysException;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Category;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class FacilityFeatureCategoriesEndpointUnitTest : BaseCategoriesEndpointUnitTest
{
    [Fact]
    public async Task CreateFacilityFeatureCategory_ReturnsCorrectResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryCreateRequest = new CategoryCreateRequest("TestName", "TestDes");
        var mockMapper = MockServices.MockMapper(
            (categoryCreateRequest, category)
        );

        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CreateCategory(categoryCreateRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal(category.Id, okResult.Value);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Category.Created", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(category.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityFeatureCategory_ReturnsCorrectResult()
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
        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.UpdateCategory(category.Id, categoryUpdateRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Category.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(category.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnableFacilityFeatureCategory_ReturnsCorrectResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryEnabledRequest = new CategoryEnabledRequest(true) { Id = category.Id };
        var mockMapper = MockServices.MockMapper(
            (categoryEnabledRequest, category)
        );

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<CategoryEnableCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category.Id);

        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, mockMediator.Object, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.EnableCategory(category.Id, categoryEnabledRequest, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Category.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(category.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task EnableFacilityFeatureCategory_ReturnAppRequestInvalidException()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryEnabledRequest = new CategoryEnabledRequest(true);
        var mockMapper = MockServices.MockMapper(
            (categoryEnabledRequest, category)
        );

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<CategoryEnableCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AppRequestInvalidException(ErrorCode.E0001, "The field is required", "Id"));

        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, mockMediator.Object, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Assert + Act
        var exception = await Assert.ThrowsAsync<AppRequestInvalidException>(
            async () =>
            {
                await controller.EnableCategory(-1000, categoryEnabledRequest, cancellationToken);
            }
        );
        Assert.Equal(ErrorCode.E0001, exception.ErrorCode);
        Assert.Equal("Id", exception.ErrorField);
    }

    [Fact]
    public async Task DeleteFacilityFeatureCategory_ReturnsCorrectResult()
    {
        // Arrange
        var categoryId = 2;
        var category = new Category
        {
            Id = categoryId,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var cancellationToken = CancellationToken.None;
        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);

        // Act
        var result = await controller.DeleteCategory(categoryId, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Category.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(category.Id.ToString(), headers["X-Liberty-params"]);
    }

    [Fact]
    public async Task GetFacilityFeatureCategory_ReturnsCorrectResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
        };
        var categoryResponse = new CategoryResponse(category.Id, "Code", "TestName", "TestDes", true);
        var mockMapper = MockServices.MockMapper(
            (category, categoryResponse)
        );
        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetCategory(category.Id, cancellationToken);

        // Assert
        var data = Assert.IsType<OkObjectResult>(result, false);
        var categoryData = Assert.IsType<CategoryResponse>(data.Value, false);
        Assert.Equal(categoryData.Id, category.Id);
        Assert.Equal((int)HttpStatusCode.OK, data.StatusCode);
    }

    [Fact]
    public async Task GetAllFacilityCategories_ReturnsCorrectResult()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                Code = "Code1",
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
            },
            new()
            {
                Id = 2,
                Code = "Code2",
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
            }
        };
        var mockMapper = MockServices.MockMapper(
            (categories, new List<CategoryResponse>
            {
                new(1, "Code1", "Category1", "Description1", true),
                new(1, "Code2", "Category2", "Description2", true)
            })
        );
        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;
        var mockPageable = new Mock<IPageable>();
        // Act
        var result = await controller.GetAllCategories(mockPageable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<CategoryResponse>>(okResult.Value, false);
        Assert.Equal(2, response.Count);
        Assert.Equal("Category1", response[0].Name);
        Assert.Equal("Category2", response[1].Name);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Total-Count"));
        Assert.True(headers.ContainsKey("X-Pagination"));
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderOfCategories_ReturnsCorrectResult()
    {
        // Arrange
        var request = new ItemUpdateOrderRequest(
            [
                1,
                2,
                3
            ]
        );
        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityFeatureCategoriesEndpoint(mockMapper, MockMediator, MockCategoryService);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ArrangeOrderOfCategories(request, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }
}
