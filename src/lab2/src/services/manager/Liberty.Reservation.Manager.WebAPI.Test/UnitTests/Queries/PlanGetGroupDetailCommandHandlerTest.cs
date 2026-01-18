using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanGetGroupDetailCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var planId = 1;

        var mockPlanRepository = new Mock<IPlanRepository>();
        var mockAccessor = new Mock<ISecurityContextAccessor>();

        mockAccessor.Setup(s => s.FacilityKey).Returns(1);

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                cfg.CreateMap<Plan, PlanBasicSettingResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(src => src.Id)
                    )
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(src => src.Name!.GetValueByHeader())
                    )
                    .ForMember(
                        dest => dest.Summary,
                        opt => opt.MapFrom(src => src.Summary!.GetValueByHeader())
                    )
                    .ForMember(
                        dest => dest.NameForImport,
                        opt => opt.MapFrom(src => src.NameForImport!.GetValueByHeader())
                    )
                    .ForMember(
                        dest => dest.Description,
                        opt => opt.MapFrom(src => src.Description!.GetValueByHeader())
                    )
                    .ForMember(
                        dest => dest.Files,
                        opt =>
                            opt.MapFrom(
                                src => src.FilePlans!.Select(
                                    x => new FileOfPlanBasicSettingResponse(
                                        x.FileId,
                                        x.File!.Code ?? string.Empty,
                                        x.Index,
                                        x.IsEnabled,
                                        "Description"
                                    )
                                )
                            )
                    );
                cfg.CreateMap<Plan, PlanCancelResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.IsCancelSameAccept,
                        opt => opt.MapFrom(
                            src => src.IsCancelSameAccept
                        )
                    )
                    .ForMember(
                        dest => dest.CancelDayLimit,
                        opt => opt.MapFrom(
                            src => src.CancelDayLimit
                        )
                    )
                    .ForMember(
                        dest => dest.CancelLimit,
                        opt => opt.MapFrom(
                            src => src.CancelLimit
                        )
                    )
                    .ForMember(
                        dest => dest.CancellationId,
                        opt => opt.MapFrom(
                            src => src.CancellationId
                        )
                    );
                cfg.CreateMap<Plan, PlanDisplayResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest =>
                            dest.Tags,
                        opt => opt.MapFrom(
                            src => string.IsNullOrEmpty(src.Tag!.GetValueByHeader())
                                ? Array.Empty<string>()
                                : src.Tag!.GetValueByHeader().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        )
                    )
                    .ForMember(
                        dest => dest.MasterCategories,
                        opt => opt.MapFrom(
                            src => src!.PlanCategories!
                                .Where(
                                    x => x.Category!.CategoryType == CategoryTypes.Plan
                                        && x.Category!.IsMaster
                                )
                                .Select(
                                    x => new CategoryOfPlanDisplayResponse(
                                        x.CategoryId,
                                        x.Category!.Name!.GetValueByHeader(),
                                        x.Category!.IsEnabled
                                    )
                                )
                        )
                    )
                    .ForMember(
                        dest => dest.PlanCategories,
                        opt => opt.MapFrom(
                            src => src!.PlanCategories!
                                .Where(
                                    x => x.Category!.CategoryType == CategoryTypes.Plan
                                        && !x.Category!.IsMaster
                                        && x.Category.FacilityCategories!.Any(
                                            y => y.Facility!.Id == src.FacilityPlans!.First().FacilityId
                                        )
                                )
                                .Select(
                                    x => new CategoryOfPlanDisplayResponse(
                                        x.CategoryId,
                                        x.Category!.Name!.GetValueByHeader(),
                                        x.Category!.IsEnabled
                                    )
                                )
                        )
                    );
                cfg.CreateMap<Plan, PlanImportantNoteResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.Payment,
                        opt => opt.MapFrom(
                            src => src.Payment
                        )
                    )
                    .ForMember(
                        dest => dest.Meal,
                        opt => opt.MapFrom(
                            src => src.Meal
                        )
                    )
                    .ForMember(
                        dest => dest.Other,
                        opt => opt.MapFrom(
                            src => src.Other
                        )
                    );
                cfg.CreateMap<Plan, PlanMealResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.MealTypes,
                        opt =>
                            opt.MapFrom(
                                src => src.PlanMealTypes!.Select(
                                    x => new TypeOfPlanMealResponse(
                                        x.MealTypeId,
                                        x.MealTypeEatType
                                    )
                                )
                            )
                    );

                cfg.CreateMap<Plan, PlanOptionResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.UseFixedOptionItem,
                        opt => opt.MapFrom(
                            src => src.UseFixedOptionItem
                        )
                    )
                    .ForMember(
                        dest => dest.OptionItems,
                        opt => opt.MapFrom(
                            src => src!.PlanOptionItems!.Select(
                                x => new OptionItemOfPlanOptionResponse(
                                    x.OptionItemId,
                                    x.OptionItem!.Name!.GetValueByHeader(),
                                    x.OptionItem!.IsEnabled
                                )
                            )
                        )
                    );
                cfg.CreateMap<Plan, PlanPaymentMethodResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.IsOnSidePayment,
                        opt => opt.MapFrom(
                            src => src.IsOnSidePayment
                        )
                    )
                    .ForMember(
                        dest => dest.IsOnLinePayment,
                        opt => opt.MapFrom(
                            src => src.IsOnLinePayment
                        )
                    );
                cfg.CreateMap<Plan, PlanPublishAcceptResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.UseDisplayDate,
                        opt => opt.MapFrom(
                            src => src.UseDisplayDate
                        )
                    )
                    .ForMember(
                        dest => dest.DisplayDateStart,
                        opt => opt.MapFrom(
                            src => src.DisplayDateStart
                        )
                    )
                    .ForMember(
                        dest => dest.DisplayDateEnd,
                        opt => opt.MapFrom(
                            src => src.DisplayDateEnd
                        )
                    )
                    .ForMember(
                        dest => dest.UseAcceptDate,
                        opt => opt.MapFrom(
                            src => src.UseAcceptDate
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptDateStart,
                        opt => opt.MapFrom(
                            src => src.AcceptDateStart
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptDateEnd,
                        opt => opt.MapFrom(
                            src => src.AcceptDateEnd
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptDays,
                        opt => opt.MapFrom(
                            src => src.AcceptDays
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptMonths,
                        opt => opt.MapFrom(
                            src => src.AcceptMonths
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptEndLimitType,
                        opt => opt.MapFrom(
                            src => src.AcceptEndLimitType
                        )
                    )
                    .ForMember(
                        dest => dest.ReceptionDayLimit,
                        opt => opt.MapFrom(
                            src => src.ReceptionDayLimit
                        )
                    )
                    .ForMember(
                        dest => dest.ReceptionLimit,
                        opt => opt.MapFrom(
                            src => src.ReceptionLimit
                        )
                    )
                    .ForMember(
                        dest => dest.Sites,
                        opt => opt.MapFrom(
                            src => src!.PlanSites!.Select(
                                x => new SiteOfPlanPublishAcceptResponse(
                                    x.SiteId,
                                    x.Site!.Name!.GetValueByHeader(),
                                    x.Site.IsEnabled
                                )
                            )
                        )
                    );
                cfg.CreateMap<Plan, PlanQuestionResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.Questions,
                        opt => opt.MapFrom(
                            src => src!.PlanQuestions!.Select(
                                x => new QuestionOfPlanQuestionResponse(
                                    x.QuestionId,
                                    x.Question!.Name!.GetValueByHeader(),
                                    x.Question.IsEnabled
                                )
                            )
                        )
                    );

                cfg.CreateMap<Plan, PlanRoomTypeResponse>()
                    .ForMember(
                        dest => dest.RoomGroups,
                        opt => opt.MapFrom(
                            src => src!.PlanRoomGroups!.Select(
                                x => new RoomGroupOfPlanRoomTypeResponse(
                                    x.RoomGroupId,
                                    x.RoomGroup!.Name!.GetValueByHeader(),
                                    x.RoomGroup.IsEnabled
                                )
                            )
                        )
                    );

                cfg.CreateMap<Plan, PlanSaleResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.CheckInStart,
                        opt => opt.MapFrom(
                            src => src.CheckInStart
                        )
                    )
                    .ForMember(
                        dest => dest.CheckInEnd,
                        opt => opt.MapFrom(
                            src => src.CheckInEnd
                        )
                    )
                    .ForMember(
                        dest => dest.RoomNumberDaySaleLimit,
                        opt => opt.MapFrom(
                            src => src.RoomNumberDaySaleLimit
                        )
                    )
                    .ForMember(
                        dest => dest.GroupNumberDaySaleLimit,
                        opt => opt.MapFrom(
                            src => src.GroupNumberDaySaleLimit
                        )
                    )
                    .ForMember(
                        dest => dest.PlanDaySaleLimitType,
                        opt => opt.MapFrom(
                            src => src.PlanDaySaleLimitType
                        )
                    )
                    .ForMember(
                        dest => dest.UseDaySaleLimit,
                        opt => opt.MapFrom(
                            src => src.UseDaySaleLimit
                        )
                    )
                    .ForMember(
                        dest => dest.UseAcceptPersonNumber,
                        opt => opt.MapFrom(
                            src => src.UseAcceptPersonNumber
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptPersonNumberMin,
                        opt => opt.MapFrom(
                            src => src.AcceptPersonNumberMin
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptPersonNumberMax,
                        opt => opt.MapFrom(
                            src => src.AcceptPersonNumberMax
                        )
                    )
                    .ForMember(
                        dest => dest.NumberOfStayLimitMin,
                        opt => opt.MapFrom(
                            src => src.NumberOfStayLimitMin
                        )
                    )
                    .ForMember(
                        dest => dest.NumberOfStayLimitMax,
                        opt => opt.MapFrom(
                            src => src.NumberOfStayLimitMax
                        )
                    )
                    .ForMember(
                        dest => dest.PointRate,
                        opt => opt.MapFrom(
                            src => src.PointRate!.Rate
                        )
                    )
                    .ForMember(
                        dest => dest.PointExpire,
                        opt => opt.MapFrom(
                            src => src.PointRate!.Expire
                        )
                    )
                    .ForMember(
                        dest => dest.MaxRoomNumberDaySaleLimit,
                        opt => opt.MapFrom(
                            src => src.PlanRoomGroups!.Sum(y => y.RoomGroup!.BaseNumber)
                        )
                    );
                cfg.CreateMap<Plan, PlanSpecialResponse>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.MapFrom(
                            src => src.Id
                        )
                    )
                    .ForMember(
                        dest => dest.IsSecret,
                        opt => opt.MapFrom(
                            src => src.IsSecret
                        )
                    )
                    .ForMember(
                        dest => dest.SecretWord,
                        opt => opt.MapFrom(
                            src => src.SecretWord
                        )
                    );
            }
        );

        var plans = new List<Plan>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
                Summary = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "summary" } },
                Payment = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "payment details" } },
                Meal = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "meal details" } },
                Other = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "other details" } },
                Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description of the plan" } },
                Tag = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "tag" } },
                NameForImport = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "NameForImport" } },
                FilePlans = [],
                PlanCategories = [],
                PlanMealTypes = [],
                FacilityPlans =
                [
                    new()
                    {
                        FacilityId = 1,
                        PlanId = 1
                    }
                ],
                PlanOptionItems = [],
                PlanQuestions = [],
                PlanSites = [],
                PlanRoomGroups =
                [
                    new()
                    {
                        PlanId = 1,
                        RoomGroup = new()
                        {
                            Id = 1,
                            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                            IsEnabled = true,
                            BaseNumber = 1
                        }
                    }
                ],
                UseFixedOptionItem = false,
                UseDaySaleLimit = false,
                UseAcceptPersonNumber = false,
                IsSecret = false,
                PointRate = new() { Expire = 1 }
            }
        };

        mockPlanRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(plans.AsQueryable().BuildMock());

        List<GroupOfPlan> groupsOfPlan =
        [
            GroupOfPlan.BasicSetting,
            GroupOfPlan.Cancel,
            GroupOfPlan.Display,
            GroupOfPlan.ImportantNote,
            GroupOfPlan.Meal,
            GroupOfPlan.Option,
            GroupOfPlan.PaymentMethod,
            GroupOfPlan.PublishAccept,
            GroupOfPlan.Question,
            GroupOfPlan.RoomType,
            GroupOfPlan.Sale,
            GroupOfPlan.Special
        ];

        foreach (var group in groupsOfPlan)
        {
            var command = new PlanGetGroupDetailsQuery(planId, group);
            var handler = new PlanGetGroupDetailsQueryHandler(
                config.CreateMapper(),
                mockAccessor.Object,
                mockPlanRepository.Object
            );

            var (_, result) = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
        }
    }
}
