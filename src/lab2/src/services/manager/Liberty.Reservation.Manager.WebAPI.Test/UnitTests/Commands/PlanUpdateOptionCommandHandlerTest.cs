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

public class PlanUpdateOptionCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockLogger = new Mock<ILogger<PlanUpdateOptionCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanOptionItemService = new Mock<IPlanOptionItemService>();
        var mockOptionItemService = new Mock<IOptionItemService>();

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanUpdateOptionRequest, Plan>()
                    .ForMember(
                        dest => dest.UseFixedOptionItem,
                        opt => opt.MapFrom(
                            src => src.UseFixedOptionItem
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

        var request = new PlanUpdateOptionRequest(true, []);

        var command = new PlanUpdateOptionCommand(1) { Payload = request };
        var handler = new PlanUpdateOptionCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockPlanService.Object,
            mockOptionItemService.Object,
            mockPlanOptionItemService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
