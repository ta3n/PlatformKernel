using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class OptionItemCreateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOptionItemId_WhenCommandIsValid()
    {
        // Arrange
        var facilityKey = 123L;
        var expectedOptionItemId = 100L;

        // Mocks
        var mockLogger = new Mock<ILogger<OptionItemCreateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockFacilityOptionItemService = new Mock<IFacilityOptionItemService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityKey);

        var optionItem = new OptionItem
        {
            Id = expectedOptionItemId,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Option Item" } }
        };
        mockOptionItemService.Setup(s => s.CreateAsync(It.IsAny<OptionItem>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(optionItem);

        mockFacilityOptionItemService.Setup(
                s => s.CreateAsync(It.IsAny<FacilityOptionItem>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new FacilityOptionItem());

        mockMapper.Setup(m => m.Map<OptionItem>(It.IsAny<object>())).Returns(optionItem);

        var payload = new OptionItemCreateRequest("Name", "Des", 1, 100);

        var command = new OptionItemCreateCommand { Payload = payload };

        var handler = new OptionItemCreateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockOptionItemService.Object,
            mockFacilityOptionItemService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedOptionItemId, result);
    }
}
