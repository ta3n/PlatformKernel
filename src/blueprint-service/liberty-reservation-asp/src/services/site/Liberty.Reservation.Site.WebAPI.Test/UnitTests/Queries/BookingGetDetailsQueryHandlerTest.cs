using System.Text;
using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.Application.Models;
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Queries;

public class BookingGetDetailsQueryHandlerTest : BaseUnitTest
{
    protected override void InitData()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                MapperBookingDetailsResponse(cfg);
                cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                cfg.CreateMap<RoomGroup, RoomGroupResponse>()
                    .ConstructUsing(
                        src =>
                            new RoomGroupResponse(
                                src.Id,
                                src.Code,
                                src.Name!.GetValueByHeader(),
                                src.Tag,
                                src.Description!.GetValueByHeader(),
                                src.Overview!.GetValueByHeader(),
                                src.CapacityMax ?? 1,
                                src.CapacityMin ?? 1,
                                src.Size,
                                src.IsEnabledSmoking,
                                src.IsRoomSizeVisible,
                                src.IsBedTypeVisible,
                                src.RoomGroupSizeUnitType,
                                src.RoomGroupBedTypes!
                                    .Where(x => x.Number > 0)
                                    .Select(x => $"{x.BedType!.Name!} {x.Number}")
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.RoomGroup
                                            && x.Category!.IsMaster
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader())
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.RoomGroup
                                            && !x.Category!.IsMaster
                                            && x.Category.FacilityCategories!.Any(
                                                y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                            )
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader())
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.Amenity
                                            && x.Category!.IsMaster
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader())
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.Amenity
                                            && !x.Category!.IsMaster
                                            && x.Category.FacilityCategories!.Any(
                                                y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                            )
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader())
                                    .ToList(),
                                Array.Empty<FileOfBookingResponse>(),
                                src.UpdatedAt
                            )
                    );
            }
        );

        var mapper = new Mapper(config);
        MockMapper = mapper;
    }

    private static void MapperBookingDetailsResponse(
        IMapperConfigurationExpression cfg
    )
    {
        cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
        cfg.CreateMap<Plan, BookingDetailsResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(src => src.Code)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.CheckInStart,
                opt => opt.MapFrom(src => src.CheckInStart)
            )
            .ForMember(
                dest => dest.CheckInEnd,
                opt => opt.MapFrom(src => src.CheckInEnd)
            )
            .ForMember(
                dest => dest.CheckOut,
                opt => opt.MapFrom(src => src.CheckOut)
            )
            .ForMember(
                dest => dest.Tag,
                opt => opt.MapFrom(src => src.Tag!.GetValueByCode(DefaultValues.LanguageCode))
            )
            .ForMember(
                dest => dest.Summary,
                opt => opt.MapFrom(src => src.Summary!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Description!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Payment,
                opt => opt.MapFrom(src => src.Payment!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Meal,
                opt => opt.MapFrom(src => src.Meal!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.Other,
                opt => opt.MapFrom(src => src.Other!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.IsOnSidePayment,
                opt => opt.MapFrom(src => src.IsOnSidePayment)
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                        && src.FacilityPlans != null
                        && src.FacilityPlans.Count > 0
                        && src.FacilityPlans.First().Facility!.CanOnLinePayment
                        && src.FacilityPlans.First().Facility!.IsOnLinePayment
                )
            )
            .ForMember(
                dest => dest.SiteId,
                opt => opt.MapFrom(
                    src => src.PlanSites != null && src.PlanSites.Count != 0
                        ? src.PlanSites.First().SiteId
                        : 0
                )
            )
            .ForMember(
                dest => dest.Files,
                opt => opt.MapFrom(
                    src => src.FilePlans!.Select(
                            x => new FileOfBookingResponse(
                                x.File!.Code,
                                x.File!.ContentType
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Meals,
                opt => opt.MapFrom(
                    src => src.PlanMealTypes!
                        .Select(
                            x => new MealOfBookingResponse(
                                x.MealTypeId,
                                x.IsEnabled,
                                x.MealTypeEatType,
                                x.MealType!.Name
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Cancellation,
                opt => opt.MapFrom(
                    src => new CancellationResponse(
                        src.CancellationId,
                        src.Cancellation!.Name!.GetValueByHeader(),
                        src.Cancellation!.Description!.GetValueByHeader(),
                        src.Cancellation!.TableSource!.GetValueByHeader()
                    )
                )
            )
            .ForMember(
                dest => dest.Categories,
                opt => opt.MapFrom(
                    src => src.PlanCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.Plan && !x.Category!.IsMaster)
                        .Select(x => x.Category!.Name!.GetValueByHeader())
                )
            )
            .ForMember(
                dest => dest.LastUpdateString,
                opt => opt.MapFrom(
                    src =>
                        string.Join(
                            ';',
                            src.UpdatedAt.ToString(),
                            src.PlanSites != null && src.PlanSites.Count != 0
                                ? src.PlanSites.First().Site!.UpdatedAt.ToString()
                                : string.Empty,
                            src.Cancellation != null ? src.Cancellation!.UpdatedAt.ToString() : string.Empty
                        )
                )
            )
            .ForMember(
                dest => dest.Questions,
                opt => opt.MapFrom(
                    src => src.PlanQuestions!.Select(
                            x => new QuestionOfBookingResponse(
                                x.QuestionId,
                                x.Question!.Name != null ? x.Question!.Name!.GetValueByHeader() : string.Empty,
                                x.Question!.Description != null ? x.Question!.Description!.GetValueByHeader() : string.Empty,
                                x.Question!.FormData != null ? x.Question!.FormData!.GetValueByHeader() : string.Empty,
                                x.Question!.QuestionType
                            )
                        )
                        .ToList()
                )
            );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBookingDetails_WhenDataIsValid()
    {
        // Arrange
        var facilityCode = "FAC123";
        var facilityId = 1;
        var planId = 1;
        var roomGroupId = 1;

        var mockPlanData = new List<Plan>
            {
                new()
                {
                    Id = planId,
                    Code = "Code1",
                    IsEnabled = true,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Plan Name" } },
                    Tag = new MultilingualText { { TestUtil.DefaultLanguageCode, "Tag1" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description of the plan" } },
                    Payment = new MultilingualText { { TestUtil.DefaultLanguageCode, "payment details" } },
                    Meal = new MultilingualText { { TestUtil.DefaultLanguageCode, "meal details" } },
                    Other = new MultilingualText { { TestUtil.DefaultLanguageCode, "other details" } },
                    Summary = new MultilingualText { { TestUtil.DefaultLanguageCode, "other details" } },
                    IsOnLinePayment = true,
                    IsOnSidePayment = false,
                    CheckInStart = new TimeSpan(2, 30, 0),
                    CheckInEnd = new TimeSpan(2, 30, 0),
                    CheckOut = new TimeSpan(2, 30, 0),
                    FilePlans =
                    [
                        new()
                        {
                            File = new()
                            {
                                Code = "FileCode",
                                ContentType = "Type"
                            }
                        }
                    ],
                    PlanMealTypes =
                    [
                        new()
                        {
                            MealTypeId = 1,
                            IsEnabled = true,
                            MealTypeEatType = MealTypeEatTypes.Box,
                            MealType = new() { Name = "Meal" }
                        }
                    ],
                    PlanCategories =
                    [
                        new PlanCategory
                        {
                            Category = new()
                            {
                                CategoryType = CategoryTypes.Plan,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "category Name" } }
                            }
                        }
                    ],
                    PlanSites =
                    [
                        new()
                        {
                            SiteId = 1,
                            Site = new()
                            {
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Sample Site" } },
                                ShortName = "SS",
                                Url = "https://sample-site.com",
                                Description = "Sample site description",
                                UseSitePoint = true,
                                IsMaster = false,
                                Tag = "SampleTag",
                                PlanRoomGroupSites =
                                [
                                    new()
                                    {
                                        RoomGroupId = 1,
                                        SiteId = 1,
                                        IsEnabled = true
                                    }
                                ],
                                UpdatedAt = 20250101
                            }
                        }
                    ],
                    FacilityPlans =
                    [
                        new()
                        {
                            Facility = new Facility
                            {
                                CanOnLinePayment = true,
                                IsOnLinePayment = true
                            },
                            IsEnabled = true,
                            FacilityId = facilityId
                        }
                    ],
                    PlanRoomGroupSitePersonAgeTypes =
                    [
                        new()
                        {
                            SiteId = 1,
                            RoomGroupId = roomGroupId
                        }
                    ],
                    Cancellation = new()
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation" } },
                        Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                        TableSource = new MultilingualText { { TestUtil.DefaultLanguageCode, "TableSource" } }
                    },
                    Meta = new()
                    {
                        Heading1 = null,
                        Description = null,
                        BarrierFree = null,
                        SpaTax = null,
                        SpaTaxTable = null,
                        Cancelling = null,
                        CancellingTable = null
                    },
                    PlanQuestions =
                    [
                        new()
                        {
                            Question = new Question
                            {
                                Id = 1,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Question 2" } },
                                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description 2" } },
                                FormData = new MultilingualText { { TestUtil.DefaultLanguageCode, "Form Data 2" } },
                                QuestionType = QuestionTypes.Select
                            }
                        }
                    ],
                    PlanRoomGroupSites =
                    [
                        new PlanRoomGroupSite
                        {
                            PlanId = 1,
                            RoomGroupId = 1,
                            SiteId = 1,
                            IsEnabled = true
                        }
                    ],
                    PlanRoomGroups =
                    [
                        new PlanRoomGroup
                        {
                            PlanId = 1,
                            RoomGroupId = 1,
                            RoomGroup = new() { IsEnabled = true }
                        }
                    ]
                }
            }
            .AsQueryable()
            .BuildMock();

        var mockRoomGroupData = new List<RoomGroup>
            {
                new()
                {
                    Id = roomGroupId,
                    Code = "RG001",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Deluxe Room" } },
                    Tag = "Deluxe",
                    CapacityMax = 4,
                    CapacityMin = 2,
                    Size = 30,
                    IsEnabledSmoking = true,
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room description" } },
                    Overview = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room overview" } },
                    IsEnabled = true,
                    PlanRoomGroups =
                    [
                        new()
                        {
                            PlanId = planId,
                            RoomGroupId = roomGroupId
                        }
                    ],
                    FacilityRoomGroups =
                    [
                        new()
                        {
                            FacilityId = facilityId,
                            RoomGroupId = roomGroupId
                        }
                    ],
                    RoomGroupSizeUnitType = RoomGroupSizeUnitTypes.M2,
                    UpdatedAt = 202510101,
                    RoomGroupBedTypes =
                    [
                        new()
                        {
                            BedType = new BedType { Name = "BedType" },
                            Number = 1
                        }
                    ],
                    RoomGroupCategories =
                    [
                        new()
                        {
                            Category = new Category
                            {
                                CategoryType = CategoryTypes.RoomGroup,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroupCategory" } },
                                IsMaster = true
                            }
                        },
                        new()
                        {
                            Category = new Category
                            {
                                CategoryType = CategoryTypes.Amenity,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "AmenityCategory" } },
                                IsMaster = true
                            }
                        }
                    ],
                    PlanRoomGroupSites =
                    [
                        new()
                        {
                            PlanId = planId,
                            SiteId = 1,
                            RoomGroupId = roomGroupId,
                            IsEnabled = true
                        }
                    ]
                }
            }
            .AsQueryable()
            .BuildMock();

        var planRoomGroupSitePersonAgeTypeData = new List<PlanRoomGroupSitePersonAgeType>
            {
                new()
                {
                    PlanId = planId,
                    RoomGroupId = roomGroupId,
                    SiteId = 1,
                    IsEnabled = true,
                    UpdatedAt = 202510101,
                    PersonAgeTypeId = 1,
                    PersonAgeType = new PersonAgeType
                    {
                        Id = 1,
                        Name = new MultilingualText
                        {
                            { TestUtil.DefaultLanguageCode, "Adult" },
                            { "en", "Adult" }
                        },
                        IsMain = true,
                        AgeMin = 1,
                        AgeMax = 2,
                        IsEnabled = true,
                        IsVisible = true
                    }
                }
            }
            .AsQueryable()
            .BuildMock();

        var expectedFacilityUpdatedTime = AppDate.GetId(DateTime.UtcNow);

        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var cacheServiceMock = new Mock<ICacheService>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();
        var planRoomGroupSitePersonAgeTypeMock = new Mock<IBookingPlanRoomGroupSitePersonAgeTypeRepository>();

        securityContextMock.Setup(x => x.GetFacilityCodeSelected()).Returns(facilityCode);
        securityContextMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextMock.Setup(x => x.GetSiteIdSelected()).Returns(1);

        facilityRepositoryMock
            .Setup(
                x => x.GetFacilityUpdatedTimeAvailableAsync(
                    It.IsAny<long>(),
                    It.IsAny<DbContext>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedFacilityUpdatedTime);

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(mockPlanData);
        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(mockRoomGroupData);
        planRoomGroupSitePersonAgeTypeMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(planRoomGroupSitePersonAgeTypeData);

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var query = new BookingGetDetailsQuery(planId, roomGroupId);

        var handler = new BookingGetDetailsQueryHandler(
            MockMapper,
            securityContextMock.Object,
            planRoomGroupSitePersonAgeTypeMock.Object,
            cacheServiceMock.Object,
            planRepositoryMock.Object,
            roomGroupRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Item2);
        Assert.IsType<BookingDetailsResponse>(result.Item2);
        Assert.Equal(planId, result.Item2.Id);
        Assert.NotNull(result.Item2.RoomGroup);
        Assert.Equal(roomGroupId, result.Item2.RoomGroup.Id);

        var json = JsonConvert.SerializeObject(lastUpdateObject);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        Assert.Contains(lastUpdateString, result.Item2.LastUpdateString);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupNotfoundException()
    {
        // Arrange
        var facilityCode = "FAC123";
        var facilityId = 1;
        var planId = 1;
        var roomGroupId = 1;

        var mockPlanData = new List<Plan>
            {
                new()
                {
                    Id = planId,
                    Code = "Code1",
                    IsEnabled = true,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Plan Name" } },
                    Tag = new MultilingualText { { TestUtil.DefaultLanguageCode, "Tag1" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description of the plan" } },
                    Payment = new MultilingualText { { TestUtil.DefaultLanguageCode, "payment details" } },
                    Meal = new MultilingualText { { TestUtil.DefaultLanguageCode, "meal details" } },
                    Other = new MultilingualText { { TestUtil.DefaultLanguageCode, "other details" } },
                    Summary = new MultilingualText { { TestUtil.DefaultLanguageCode, "other details" } },
                    IsOnLinePayment = true,
                    IsOnSidePayment = false,
                    CheckInStart = new TimeSpan(2, 30, 0),
                    CheckInEnd = new TimeSpan(2, 30, 0),
                    CheckOut = new TimeSpan(2, 30, 0),
                    FilePlans =
                    [
                        new()
                        {
                            File = new()
                            {
                                Code = "FileCode",
                                ContentType = "Type"
                            }
                        }
                    ],
                    PlanMealTypes =
                    [
                        new()
                        {
                            MealTypeId = 1,
                            IsEnabled = true,
                            MealTypeEatType = MealTypeEatTypes.Box,
                            MealType = new() { Name = "Meal" }
                        }
                    ],
                    PlanCategories =
                    [
                        new PlanCategory
                        {
                            Category = new()
                            {
                                CategoryType = CategoryTypes.Plan,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "category Name" } }
                            }
                        }
                    ],
                    PlanSites =
                    [
                        new()
                        {
                            SiteId = 1,
                            Site = new()
                            {
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Sample Site" } },
                                ShortName = "SS",
                                Url = "https://sample-site.com",
                                Description = "Sample site description",
                                UseSitePoint = true,
                                IsMaster = false,
                                Tag = "SampleTag",
                                PlanRoomGroupSites =
                                [
                                    new()
                                    {
                                        RoomGroupId = 1,
                                        SiteId = 1,
                                        IsEnabled = true
                                    }
                                ],
                                UpdatedAt = 20250101
                            }
                        }
                    ],
                    FacilityPlans =
                    [
                        new()
                        {
                            Facility = new Facility
                            {
                                CanOnLinePayment = true,
                                IsOnLinePayment = true
                            },
                            IsEnabled = true,
                            FacilityId = facilityId
                        }
                    ],
                    PlanRoomGroupSitePersonAgeTypes =
                    [
                        new()
                        {
                            SiteId = 1,
                            RoomGroupId = roomGroupId
                        }
                    ],
                    Cancellation = new()
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Cancellation" } },
                        Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                        TableSource = new MultilingualText { { TestUtil.DefaultLanguageCode, "FakeTableSource" } }
                    },
                    Meta = new()
                    {
                        Heading1 = null,
                        Description = null,
                        BarrierFree = null,
                        SpaTax = null,
                        SpaTaxTable = null,
                        Cancelling = null,
                        CancellingTable = null
                    },
                    PlanQuestions =
                    [
                        new()
                        {
                            Question = new()
                            {
                                Id = 1,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Question 2" } },
                                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description 2" } },
                                FormData = new MultilingualText { { TestUtil.DefaultLanguageCode, "Form Data 2" } },
                                QuestionType = QuestionTypes.Select
                            }
                        }
                    ],
                    PlanRoomGroupSites =
                    [
                        new PlanRoomGroupSite
                        {
                            PlanId = 1,
                            RoomGroupId = 1,
                            SiteId = 1,
                            IsEnabled = true
                        }
                    ],
                    PlanRoomGroups =
                    [
                        new PlanRoomGroup
                        {
                            PlanId = 1,
                            RoomGroupId = 1,
                            RoomGroup = new() { IsEnabled = true }
                        }
                    ]
                }
            }.AsQueryable()
            .BuildMock();

        var mockRoomGroupData = new List<RoomGroup>()
            .AsQueryable()
            .BuildMock();

        var planRoomGroupSitePersonAgeTypeData = new List<PlanRoomGroupSitePersonAgeType>
            {
                new()
                {
                    PlanId = planId,
                    RoomGroupId = roomGroupId,
                    SiteId = 1,
                    IsEnabled = true,
                    UpdatedAt = 202510101,
                    PersonAgeTypeId = 1,
                    PersonAgeType = new PersonAgeType
                    {
                        Id = 1,
                        IsMain = true,
                        AgeMin = 1,
                        AgeMax = 2,
                        IsEnabled = true,
                        IsVisible = true
                    }
                }
            }.AsQueryable()
            .BuildMock();

        var expectedFacilityUpdatedTime = AppDate.GetId(DateTime.UtcNow);

        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var cacheServiceMock = new Mock<ICacheService>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        securityContextMock.Setup(x => x.GetFacilityCodeSelected()).Returns(facilityCode);
        securityContextMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextMock.Setup(x => x.GetSiteIdSelected()).Returns(1);

        facilityRepositoryMock.Setup(
                x => x.GetFacilityUpdatedTimeAvailableAsync(It.IsAny<long>(), It.IsAny<DbContext>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedFacilityUpdatedTime);

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(mockPlanData);
        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(mockRoomGroupData);
        var planRoomGroupSitePersonAgeTypeMock = new Mock<IBookingPlanRoomGroupSitePersonAgeTypeRepository>();
        planRoomGroupSitePersonAgeTypeMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(planRoomGroupSitePersonAgeTypeData);
        var query = new BookingGetDetailsQuery(1, 1);

        var handler = new BookingGetDetailsQueryHandler(
            new Mapper(
                new MapperConfiguration(
                    cfg =>
                    {
                        MapperBookingDetailsResponse(cfg);
                        cfg.CreateMap<RoomGroup, RoomGroupResponse>()
                            .ConstructUsing(
                                src =>
                                    new RoomGroupResponse(
                                        0,
                                        "Code",
                                        "Name",
                                        "Tag",
                                        "Description",
                                        "Overview",
                                        1,
                                        1,
                                        1,
                                        true,
                                        true,
                                        true,
                                        RoomGroupSizeUnitTypes.M2,
                                        null,
                                        null,
                                        null,
                                        null,
                                        null,
                                        null,
                                        1
                                    )
                            );
                    }
                )
            ),
            securityContextMock.Object,
            planRoomGroupSitePersonAgeTypeMock.Object,
            cacheServiceMock.Object,
            planRepositoryMock.Object,
            roomGroupRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        // Assert
        await Assert.ThrowsAsync<RoomGroupNotfoundException>(
            () => handler.Handle(query, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBookingNotfoundException()
    {
        // Arrange
        var facilityCode = "FAC123";
        var facilityId = 1;
        var planId = 1;
        var roomGroupId = 1;

        var mockPlanData = new List<Plan>().AsQueryable().BuildMock();

        var mockRoomGroupData = new List<RoomGroup>
            {
                new()
                {
                    Id = roomGroupId,
                    Code = "RG001",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Deluxe Room" } },
                    Tag = "Deluxe",
                    CapacityMax = 4,
                    CapacityMin = 2,
                    Size = 30,
                    IsEnabledSmoking = true,
                    IsEnabled = true,
                    PlanRoomGroups =
                    [
                        new() { PlanId = planId }
                    ],
                    RoomGroupSizeUnitType = RoomGroupSizeUnitTypes.M2,
                    UpdatedAt = 202510101,
                    RoomGroupBedTypes =
                    [
                        new() { BedType = new() { Name = "BedType" } }
                    ],
                    RoomGroupCategories =
                    [
                        new()
                        {
                            Category = new()
                            {
                                CategoryType = CategoryTypes.MealType,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Category" } }
                            }
                        }
                    ]
                }
            }
            .AsQueryable()
            .BuildMock();

        var planRoomGroupSitePersonAgeTypeData = new List<PlanRoomGroupSitePersonAgeType>
            {
                new()
                {
                    PlanId = planId,
                    RoomGroupId = roomGroupId,
                    SiteId = 1,
                    IsEnabled = true,
                    UpdatedAt = 202510101,
                    PersonAgeTypeId = 1,
                    PersonAgeType = new PersonAgeType
                    {
                        Id = 1,
                        IsMain = true,
                        AgeMin = 1,
                        AgeMax = 2,
                        IsEnabled = true,
                        IsVisible = true
                    }
                }
            }
            .AsQueryable()
            .BuildMock();

        var expectedFacilityUpdatedTime = AppDate.GetId(DateTime.UtcNow);

        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var cacheServiceMock = new Mock<ICacheService>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        securityContextMock.Setup(x => x.GetFacilityCodeSelected()).Returns(facilityCode);
        securityContextMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);

        facilityRepositoryMock.Setup(
                x => x.GetFacilityUpdatedTimeAvailableAsync(It.IsAny<long>(), It.IsAny<DbContext>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedFacilityUpdatedTime);

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(mockPlanData);
        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(mockRoomGroupData);
        var planRoomGroupSitePersonAgeTypeMock = new Mock<IBookingPlanRoomGroupSitePersonAgeTypeRepository>();
        planRoomGroupSitePersonAgeTypeMock.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(planRoomGroupSitePersonAgeTypeData);
        var query = new BookingGetDetailsQuery(1, 1);

        var handler = new BookingGetDetailsQueryHandler(
            MockMapper,
            securityContextMock.Object,
            planRoomGroupSitePersonAgeTypeMock.Object,
            cacheServiceMock.Object,
            planRepositoryMock.Object,
            roomGroupRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        await Assert.ThrowsAsync<BookingNotfoundException>(
            () => handler.Handle(query, CancellationToken.None)
        );
    }
}
