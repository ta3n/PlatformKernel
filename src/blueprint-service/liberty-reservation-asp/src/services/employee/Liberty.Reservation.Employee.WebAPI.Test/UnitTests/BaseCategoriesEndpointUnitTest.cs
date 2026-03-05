using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public abstract class BaseCategoriesEndpointUnitTest : BaseUnitTest
{
    protected ICategoryService MockCategoryService { get; set; } = null!;

    protected override void InitData()
    {
        var categoryService = new Mock<ICategoryService>();

        categoryService
            .Setup(
                x => x.CreateAsync(
                    It.IsAny<Category>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Category
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
                }
            );

        categoryService
            .Setup(
                x => x.UpdateAsync(
                    It.IsAny<Category>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Category, Category, Category>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Category
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
                }
            );

        categoryService
            .Setup(
                x => x.EnableAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Category
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
                }
            );

        categoryService
            .Setup(
                x => x.DeleteAsync(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Category
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
                }
            );

        categoryService
            .Setup(
                x => x.FindByIdAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Category
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } }
                }
            );

        categoryService
            .Setup(
                x => x.FindAllByTypeAsync(
                    It.IsAny<IPageable>(),
                    It.IsAny<CategoryTypes>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    IPageable pageable,
                    CategoryTypes _,
                    CancellationToken _
                ) =>
                {
                    var categories = new List<Category>
                    {
                        new()
                        {
                            Id = 1,
                            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } },
                            CategoryType = CategoryTypes.MealType
                        },
                        new()
                        {
                            Id = 2,
                            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestDes" } },
                            CategoryType = CategoryTypes.MealType
                        }
                    };

                    var mockPage = new Mock<IPage<Category>>();
                    mockPage.Setup(p => p.TotalPages).Returns((int)Math.Ceiling((double)categories.Count / pageable.PageSize));
                    mockPage.Setup(p => p.HasPrevious).Returns(pageable.PageNumber > 1);
                    mockPage.Setup(p => p.HasNext).Returns(pageable.PageNumber * pageable.PageSize < categories.Count);
                    mockPage.Setup(p => p.Content).Returns(categories);

                    return mockPage.Object;
                }
            );

        categoryService.Setup(
                x => x.ArrangeOrderAsync(
                    It.IsAny<List<long>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        MockCategoryService = categoryService.Object;
    }
}
