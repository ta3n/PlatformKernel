using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;
using Moq;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Exceptions;
using MockQueryable;
using Liberty.Reservation.Application.Constants;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class OptionItemGetQueryHandlerTest
{
    private readonly IMapper _mapper;

    public OptionItemGetQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<OptionItem, OptionItemDetailResponse>()
                    .ConstructUsing(
                        src => new OptionItemDetailResponse(
                            src.Id,
                            src.Name!.GetValueByHeader(),
                            src.Description,
                            src.BaseNumber,
                            src.MaxSupplyNumber,
                            src.Price,
                            src.OptionItemCategories!.Where(
                                    x => x.Category!.CategoryType == CategoryTypes.OptionItem && !x.Category!.IsMaster
                                )
                                .Select(
                                    t => new CategoryOfOptionItemDetailResponse(
                                        t.CategoryId,
                                        t.Category!.Name!.GetValueByHeader()
                                    )
                                ),
                            src.OptionItemCategories!.Where(
                                    x => x.Category!.CategoryType == CategoryTypes.OptionItem && x.Category!.IsMaster
                                )
                                .Select(
                                    t => new CategoryOfOptionItemDetailResponse(
                                        t.CategoryId,
                                        t.Category!.Name!.GetValueByHeader()
                                    )
                                ),
                            src.OptionItemQuestions!.Select(
                                t => new QuestionOfOptionItemDetailResponse(
                                    t.QuestionId,
                                    t.Question!.Name!.GetValueByHeader()
                                )
                            ),
                            src.FileOptionItems!.Select(
                                t => new FileOfOptionItemDetailResponse(
                                    t.FileId,
                                    t.File!.Code ?? string.Empty,
                                    0,
                                    true,
                                    "Description"
                                )
                            ),
                            src.IsEnabled
                        )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOptionItemDetailResponse_WhenItemExists()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockOptionItemRepository = new Mock<IOptionItemRepository>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        var facilityId = 1;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var optionItem = new OptionItem
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Option" } },
            Description = "Test Description",
            BaseNumber = 1,
            Price = 100,
            IsEnabled = true,
            FacilityOptionItems = new List<FacilityOptionItem> { new() { FacilityId = facilityId } },
            OptionItemCategories = new List<OptionItemCategory>
            {
                new()
                {
                    CategoryId = 101,
                    Category = new Category
                    {
                        Id = 101,
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Category 1" } },
                        CategoryType = CategoryTypes.OptionItem,
                        IsMaster = false
                    }
                },
                new()
                {
                    CategoryId = 102,
                    Category = new Category
                    {
                        Id = 102,
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Master Category" } },
                        CategoryType = CategoryTypes.OptionItem,
                        IsMaster = true
                    }
                }
            },
            OptionItemQuestions = new List<OptionItemQuestion>
            {
                new()
                {
                    QuestionId = 201,
                    Question = new Question
                    {
                        Id = 201,
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Question 1" } }
                    }
                }
            },
            FileOptionItems = new List<FileOptionItem>
            {
                new()
                {
                    FileId = 301,
                    File = new()
                    {
                        Id = 301,
                        Code = "File_123"
                    }
                }
            }
        };

        var mockDbSet = new List<OptionItem> { optionItem }.AsQueryable().AsEnumerable().BuildMock();

        mockOptionItemRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockDbSet);

        var query = new OptionItemGetQuery(optionItem.Id);
        var handler = new OptionItemGetQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockOptionItemRepository.Object
        );

        // Act
        var (_, response) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(optionItem.Id, response.Id);
        Assert.Equal(optionItem.Description, response.Description);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenItemDoesNotExist()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockOptionItemRepository = new Mock<IOptionItemRepository>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        var facilityId = 1;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var mockDbSet = new List<OptionItem>().AsQueryable().AsEnumerable().BuildMock();

        mockOptionItemRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockDbSet);

        var query = new OptionItemGetQuery(99);
        var handler = new OptionItemGetQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockOptionItemRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<OptionItemNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
