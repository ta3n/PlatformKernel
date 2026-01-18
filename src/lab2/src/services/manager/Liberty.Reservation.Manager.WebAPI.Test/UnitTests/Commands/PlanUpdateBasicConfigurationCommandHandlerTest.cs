using AutoMapper;
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

public class PlanUpdateBasicConfigurationCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPlanId_WhenCommandValid()
    {
        // Arrange
        var facilityKey = 1;
        var planId = 1;

        // Mocks
        var mockLogger = new Mock<ILogger<PlanUpdateBasicSettingCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockPlanService = new Mock<IPlanService>();
        var mockFilePlanService = new Mock<IFilePlanService>();
        var mockFileService = new Mock<IFileService>();

        // InfrastructureOfTest ISecurityContextAccessor
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        mockMapper.SetupGet(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<MultilingualText, string>()
                            .ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                    }
                )
            );

        mockMapper.Setup(m => m.Map<Plan>(It.IsAny<PlanUpdateBasicSettingRequest>()))
            .Returns(
                (
                        PlanUpdateBasicSettingRequest request
                    ) =>
                    new Plan
                    {
                        Name =
                            new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), request.Name ?? string.Empty } },
                        NameForImport =
                            new MultilingualText
                            {
                                { LanguageHeaderUtil.GetLanguageCodeFromHeader(), request.NameForImport ?? string.Empty }
                            },
                        Summary = new MultilingualText
                        {
                            { LanguageHeaderUtil.GetLanguageCodeFromHeader(), request.Summary ?? string.Empty }
                        },
                        Description = new MultilingualText
                        {
                            { LanguageHeaderUtil.GetLanguageCodeFromHeader(), request.Description ?? string.Empty }
                        }
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
                x => x.UpdateBasicSettingAsync(
                    It.IsAny<Plan>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Plan, Plan, Plan>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(plan);

        var request = new PlanUpdateBasicSettingRequest("Test", "Name For Import", "Summary", "Description", []);
        var command = new PlanUpdateBasicSettingCommand(1) { Payload = request };
        var handler = new PlanUpdateBasicSettingCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockPlanService.Object,
            mockFilePlanService.Object,
            mockFileService.Object
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(plan.Id, result);
    }
}
