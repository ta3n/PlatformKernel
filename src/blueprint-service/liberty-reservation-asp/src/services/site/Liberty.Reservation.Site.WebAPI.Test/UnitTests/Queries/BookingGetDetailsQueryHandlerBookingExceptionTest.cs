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
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Queries;

public class BookingGetDetailsQueryHandlerBookingExceptionTest : BaseUnitTest
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
                                src.Name!.GetValueByHeader(DefaultValues.LanguageCode),
                                src.Tag,
                                src.IsDescriptionVisible
                                    ? MapMultilingualText(src.Description)
                                    : null,
                                src.IsOverviewVisible
                                    ? MapMultilingualText(src.Overview)
                                    : null,
                                src.CapacityMax ?? 1,
                                src.CapacityMin ?? 1,
                                src.IsRoomSizeVisible
                                    ? src.Size
                                    : null,
                                src.IsEnabledSmoking,
                                src.IsRoomSizeVisible,
                                src.IsBedTypeVisible,
                                src.IsRoomSizeVisible
                                    ? src.RoomGroupSizeUnitType
                                    : null,
                                src.IsBedTypeVisible
                                    ? src.RoomGroupBedTypes!
                                        .Where(x => x.Number > 0)
                                        .OrderBy(x => x.BedType!.Id)
                                        .Select(x => $"{x.BedType!.Name!} {x.Number}")
                                        .ToList()
                                    : new List<string>(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.RoomGroupEquipment
                                            && x.Category!.IsEnabled
                                            && x.Category!.IsMaster
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.RoomGroupEquipment
                                            && x.Category!.IsEnabled
                                            && !x.Category!.IsMaster
                                            && x.Category.FacilityCategories!.Any(
                                                y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                            )
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.Amenity
                                            && x.Category!.IsEnabled
                                            && x.Category!.IsMaster
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                                    .ToList(),
                                src.RoomGroupCategories!
                                    .Where(
                                        x => x.Category!.CategoryType == CategoryTypes.Amenity
                                            && x.Category!.IsEnabled
                                            && !x.Category!.IsMaster
                                            && x.Category.FacilityCategories!.Any(
                                                y => y.Facility!.Id == src.FacilityRoomGroups!.First().FacilityId
                                            )
                                    )
                                    .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                                    .ToList(),
                                src.FileRoomGroups!
                                    .OrderBy(x => x.Index)
                                    .Select(x => new FileOfBookingResponse(x.File!.Code, x.File.ContentType)),
                                src.UpdatedAt
                            )
                    );
            }
        );

        var mapper = new Mapper(config);
        MockMapper = mapper;
    }

    private static string? MapMultilingualText(
        MultilingualText? text
    )
    {
        return text?.GetValueByHeader(DefaultValues.LanguageCode);
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
                    IsEnabled = true,
                    UpdatedAt = 202510101,
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
