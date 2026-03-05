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

public class PlanUpdateMealCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        var planId = 1;

        var mockLogger = new Mock<ILogger<PlanUpdateMealCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanMealTypeService = new Mock<IPlanMealTypeService>();
        var mockMealTypeService = new Mock<IMealTypeService>();

        var mockMapper = new Mock<IMapper>();

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

        var request = new PlanUpdateMealRequest([]);

        var command = new PlanUpdateMealCommand(1) { Payload = request };
        var handler = new PlanUpdateMealCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockPlanService.Object,
            mockPlanMealTypeService.Object,
            mockMealTypeService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
