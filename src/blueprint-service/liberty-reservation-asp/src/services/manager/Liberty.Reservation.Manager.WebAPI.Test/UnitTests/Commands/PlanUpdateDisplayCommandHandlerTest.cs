using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanUpdateDisplayCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        // Arrange
        var facilityKey = 1;
        var planId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<PlanUpdateDisplayCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanService = new Mock<IPlanService>();
        var mockPlanCategoryService = new Mock<IPlanCategoryService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockCacheService = new Mock<ICacheService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockCategoryService.Setup(
                s => s.CountMasterByIdsAsync(
                    It.IsAny<long[]>(),
                    It.IsAny<CategoryTypes[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        mockCategoryService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PlanCreateRequest, Plan>()
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(
                            src => src.Name
                        )
                    )
                    .ForMember(
                        dest => dest.Summary,
                        opt => opt.MapFrom(
                            src => src.Summary
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

        var request = new PlanUpdateDisplayRequest([], [1], [2]);
        var command = new PlanUpdateDisplayCommand(1) { Payload = request };
        var handler = new PlanUpdateDisplayCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockPlanService.Object,
            mockPlanCategoryService.Object,
            mockCategoryService.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
