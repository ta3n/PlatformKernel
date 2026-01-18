using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanUpdateSaleCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanService = new Mock<IPlanService>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanUpdateSaleRequest, Plan>()
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
                    .ForPath(
                        dest => dest.PointRate!.Rate,
                        opt => opt.MapFrom(
                            src => src.PointRate
                        )
                    )
                    .ForPath(
                        dest => dest.PointRate!.Expire,
                        opt => opt.MapFrom(
                            src => src.PointExpire
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
        mockPlanService.Setup(x => x.FindByIdWithIncludeAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        mockPlanService.Setup(
                x => x.UpdateAsync(It.IsAny<Plan>(), It.IsAny<bool>(), It.IsAny<Func<Plan, Plan, Plan>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(plan);

        var request = new PlanUpdateSaleRequest(
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(3),
            TimeSpan.FromHours(3),
            0,
            1,
            1,
            PlanDaySaleLimitTypes.RoomGroup,
            true,
            1,
            3,
            true,
            1,
            2,
            7,
            3
        );

        var command = new PlanUpdateSaleCommand(1) { Payload = request };
        var handler = new PlanUpdateSaleCommandHandler(
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockPlanService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
