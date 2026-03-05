using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class PlanCreateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        // Arrange
        var facilityKey = 1;
        var planId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<PlanCreateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanService = new Mock<IPlanService>();
        var mockFacilityPlanService = new Mock<IFacilityPlanService>();
        var mockFacilityService = new Mock<IFacilityService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        var facility = new Facility
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Code = "Code"
        };
        // InfrastructureOfTest IFacilityService: trả về count > 0
        mockFacilityService.Setup(s => s.FindByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                cfg.CreateMap<PlanCreateRequest, Plan>()
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                        )
                    )
                    .ForMember(
                        dest => dest.Summary,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Summary ?? string.Empty } }
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
        mockPlanService.Setup(x => x.CreateAsync(It.IsAny<Plan>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var request = new PlanCreateRequest("Test", "Description", false, PlanTypes.Combo);
        var command = new PlanCreateCommand { Payload = request };
        var handler = new PlanCreateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            config.CreateMapper(),
            mockSecurityContextAccessor.Object,
            mockPlanService.Object,
            mockFacilityPlanService.Object,
            mockFacilityService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
