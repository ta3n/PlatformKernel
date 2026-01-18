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

public class PlanUpdateCancelCommandHandlerTest
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
                cfg.CreateMap<PlanUpdateCancelRequest, Plan>()
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

        var request = new PlanUpdateCancelRequest(true, 1, new TimeSpan(0, 10, 0), 1);
        var command = new PlanUpdateCancelCommand(1) { Payload = request };
        var handler = new PlanUpdateCancelCommandHandler(
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockPlanService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
