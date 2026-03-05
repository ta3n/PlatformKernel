using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Moq;
using MockQueryable.Moq;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using MockQueryable;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class OptionItemGetQueryAllHandlerTest
{
    private readonly IMapper _mapper;

    public OptionItemGetQueryAllHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<OptionItem, OptionItemResponse>()
                    .ConstructUsing(
                        src => new OptionItemResponse(
                            src.Id,
                            src.Name!.GetValueByHeader(),
                            src.Description,
                            src.BaseNumber,
                            src.Price,
                            src.IsEnabled,
                            src.PlanOptionItems!.Any(
                                x => x.Plan!.PlanType == PlanTypes.Combo
                                    || x.Plan!.PlanType == PlanTypes.RoomOnly
                            ),
                            src.FileOptionItems!.Select(
                                t => new ImageOfOptionItemResponse(
                                    t.FileId,
                                    t.File!.Code ?? string.Empty,
                                    t.Index,
                                    true,
                                    "Description"
                                )
                            )
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOptionItemResponses_WhenDataExists()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockOptionItemRepository = new Mock<IOptionItemRepository>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        var facilityId = 1;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var optionItems = new List<OptionItem>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Option 1" } },
                Description = "This is option 1",
                BaseNumber = 5,
                Price = 100,
                IsEnabled = true,
                FacilityOptionItems = new List<FacilityOptionItem> { new() { FacilityId = facilityId } },
                PlanOptionItems = new List<PlanOptionItem> { new() { Plan = new Plan { PlanType = PlanTypes.Combo } } },
                FileOptionItems = new List<FileOptionItem>
                {
                    new()
                    {
                        FileId = 101,
                        File = new() { Code = "IMG001" },
                        Index = 0
                    },
                    new()
                    {
                        FileId = 102,
                        File = new() { Code = "IMG002" },
                        Index = 1
                    }
                }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Option 1" } },
                Description = "This is option 2",
                BaseNumber = 3,
                Price = 80,
                IsEnabled = false,
                FacilityOptionItems = new List<FacilityOptionItem> { new() { FacilityId = facilityId } },
                PlanOptionItems = new List<PlanOptionItem> { new() { Plan = new Plan { PlanType = PlanTypes.RoomOnly } } },
                FileOptionItems = new List<FileOptionItem>
                {
                    new()
                    {
                        FileId = 103,
                        File = new() { Code = "IMG003" },
                        Index = 0
                    }
                }
            }
        };

        mockOptionItemRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(optionItems.AsQueryable().BuildMock());

        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new OptionItemGetAllQuery(pageable);

        var handler = new OptionItemGetAllQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockOptionItemRepository.Object
        );

        // Act
        var (_, responses) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(responses);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenNoDataMatches()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockOptionItemRepository = new Mock<IOptionItemRepository>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        var facilityId = 1;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var optionItems = new List<OptionItem>();

        mockOptionItemRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(optionItems.AsQueryable().BuildMockDbSet().Object);

        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new OptionItemGetAllQuery(pageable);

        var handler = new OptionItemGetAllQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockOptionItemRepository.Object
        );

        // Act
        var (_, responses) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(responses);
    }
}
