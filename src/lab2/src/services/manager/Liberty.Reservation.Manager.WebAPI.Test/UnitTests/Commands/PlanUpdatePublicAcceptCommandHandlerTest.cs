using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanUpdatePublicAcceptCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockLogger = new Mock<ILogger<PlanUpdatePublishAcceptCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanSiteService = new Mock<IPlanSiteService>();
        var mockSiteService = new Mock<ISiteService>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanUpdatePublishAcceptRequest, Plan>()
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
                    );
            }
        );

        var plan = new Plan
        {
            Id = planId,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            PlanType = PlanTypes.Combo,
            ReceptionDayLimit = 1,
            ReceptionLimit = new TimeSpan(0, 2, 0),
            Code = "Code"
        };
        mockPlanService.Setup(x => x.FindByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        mockPlanService.Setup(
                x => x.UpdateAsync(It.IsAny<Plan>(), It.IsAny<bool>(), It.IsAny<Func<Plan, Plan, Plan>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(plan);

        var request = new PlanUpdatePublishAcceptRequest(
            false,
            null,
            null,
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(1)),
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(1)),
            3,
            1,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            TimeSpan.Zero,
            false,
            1,
            TimeSpan.Zero,
            []
        );

        var command = new PlanUpdatePublishAcceptCommand(1) { Payload = request };
        var handler = new PlanUpdatePublishAcceptCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockPlanService.Object,
            mockSiteService.Object,
            mockPlanSiteService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
