using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class OptionItemDeleteCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOptionItemId_WhenDeletionIsSuccessful()
    {
        // Arrange
        var optionItemId = 123L;
        var expectedDeletedOptionItemId = 123L;

        var mockLogger = new Mock<ILogger<OptionItemDeleteCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockFacilityOptionItemService = new Mock<IFacilityOptionItemService>();
        var mockOptionItemCategoryService = new Mock<IOptionItemCategoryService>();
        var mockOptionItemQuestionService = new Mock<IOptionItemQuestionService>();
        var mockFileOptionItemService = new Mock<IFileOptionItemService>();

        mockFacilityOptionItemService.Setup(s => s.FindAllByOptionItemIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FacilityOptionItem>());

        mockOptionItemCategoryService.Setup(s => s.FindAllByOptionItemIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OptionItemCategory>());

        mockOptionItemQuestionService.Setup(s => s.FindAllByOptionItemIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OptionItemQuestion>());

        mockFileOptionItemService.Setup(s => s.FindAllByOptionItemIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FileOptionItem>());

        mockOptionItemService.Setup(s => s.DeleteAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OptionItem { Id = expectedDeletedOptionItemId });

        mockFacilityOptionItemService.Setup(
                s => s.DeleteRangeAsync(It.IsAny<IEnumerable<FacilityOptionItem>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<FacilityOptionItem>());

        mockOptionItemCategoryService.Setup(
                s => s.DeleteRangeAsync(It.IsAny<IEnumerable<OptionItemCategory>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<OptionItemCategory>());

        mockOptionItemQuestionService.Setup(
                s => s.DeleteRangeAsync(It.IsAny<IEnumerable<OptionItemQuestion>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<OptionItemQuestion>());

        mockFileOptionItemService.Setup(
                s => s.DeleteRangeAsync(It.IsAny<IEnumerable<FileOptionItem>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<FileOptionItem>());

        var command = new OptionItemDeleteCommand { Payload = new OptionItemDeleteRequest(optionItemId) };

        var handler = new OptionItemDeleteCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockOptionItemService.Object,
            mockFacilityOptionItemService.Object,
            mockOptionItemCategoryService.Object,
            mockOptionItemQuestionService.Object,
            mockFileOptionItemService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedDeletedOptionItemId, result);
    }
}
