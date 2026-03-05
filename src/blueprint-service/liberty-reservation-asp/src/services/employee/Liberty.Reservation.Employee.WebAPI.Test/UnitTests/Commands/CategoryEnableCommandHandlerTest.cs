using Liberty.Cache.Services;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Category;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class CategoryEnableCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldEnableCategoryAndReturnCategoryId()
    {
        // Arrange

        var loggerMock = new Mock<ILogger<CategoryEnableCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var facilityServiceMock = new Mock<IFacilityService>();
        var facilityCategoryServiceMock = new Mock<IFacilityCategoryService>();

        var handler = new CategoryEnableCommandHandler(
            loggerMock.Object,
            MockUnitOfWork,
            mapperMock.Object,
            cacheServiceMock.Object,
            categoryServiceMock.Object,
            facilityServiceMock.Object,
            facilityCategoryServiceMock.Object
        );

        const int categoryId = 1;
        var request = new CategoryEnableCommand { Payload = new CategoryEnabledRequest(true) { Id = categoryId } };

        var category = new Category
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } },
            CategoryType = CategoryTypes.MealType,
            IsMaster = false,
            ParentId = null,
            Parent = null,
            Children = []
        };

        categoryServiceMock
            .Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(category);

        cacheServiceMock
            .Setup(x => x.ResetAsync(It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(categoryId, result);
    }
}
