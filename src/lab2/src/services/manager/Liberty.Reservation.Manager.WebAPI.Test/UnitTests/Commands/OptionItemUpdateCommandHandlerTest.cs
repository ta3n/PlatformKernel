using AutoMapper;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class OptionItemUpdateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnUpdatedOptionItemId_WhenCommandIsValid()
    {
        // Arrange
        var optionItemId = 123L;
        var expectedUpdatedOptionItemId = 123L;
        var payload = new OptionItemUpdateRequest(
            "Test Option",
            "Option Description",
            10,
            2,
            100,
            [1L, 2L],
            [3L],
            [4L, 5L],
            [new(10L, 1), new(11L, 2)]
        ) { Id = optionItemId };

        var command = new OptionItemUpdateCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<OptionItemUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockQuestionService = new Mock<IQuestionService>();
        var mockOptionItemCategoryService = new Mock<IOptionItemCategoryService>();
        var mockOptionItemQuestionService = new Mock<IOptionItemQuestionService>();
        var mockFileOptionItemService = new Mock<IFileOptionItemService>();

        mockCategoryService.Setup(
                s => s.CountMasterByIdsAsync(
                    It.IsAny<long[]>(),
                    It.IsAny<CategoryTypes[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        mockCategoryService.Setup(
                s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(2);

        mockQuestionService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        mockOptionItemService.Setup(
                s => s.UpdateAsync(
                    It.IsAny<OptionItem>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<OptionItem, OptionItem, OptionItem>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new OptionItem { Id = expectedUpdatedOptionItemId });

        mockOptionItemCategoryService
            .Setup(
                s => s.ChangeCategoriesOfOptionItemAsync(
                    It.IsAny<long>(),
                    It.IsAny<long[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<OptionItemCategory>().AsEnumerable(), new List<OptionItemCategory>().AsEnumerable()));

        mockOptionItemQuestionService.Setup(
                s => s.ChangeQuestionsOfOptionItemAsync(
                    It.IsAny<long>(),
                    It.IsAny<long[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<OptionItemQuestion>().AsEnumerable(), new List<OptionItemQuestion>().AsEnumerable()));

        mockFileOptionItemService.Setup(
                s => s.ChangeFilesOfOptionItemAsync(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<(long, int)>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FileOptionItem>().AsEnumerable(), new List<FileOptionItem>().AsEnumerable()));

        var handler = new OptionItemUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockOptionItemService.Object,
            mockCategoryService.Object,
            mockQuestionService.Object,
            mockOptionItemCategoryService.Object,
            mockOptionItemQuestionService.Object,
            mockFileOptionItemService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedUpdatedOptionItemId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCategoryNotfoundException_WhenCategoryValidationFails()
    {
        // Arrange
        var optionItemId = 123L;
        var payload = new OptionItemUpdateRequest(
            "Test Option",
            "Option Description",
            10,
            2,
            100,
            [1L, 2L],
            [3L],
            [4L, 5L],
            [new(10L, 1), new(11L, 2)]
        ) { Id = optionItemId };

        var command = new OptionItemUpdateCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<OptionItemUpdateCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockOptionItemService = new Mock<IOptionItemService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockQuestionService = new Mock<IQuestionService>();
        var mockOptionItemCategoryService = new Mock<IOptionItemCategoryService>();
        var mockOptionItemQuestionService = new Mock<IOptionItemQuestionService>();
        var mockFileOptionItemService = new Mock<IFileOptionItemService>();

        mockCategoryService.Setup(
                s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CategoryTypes[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);

        var handler = new OptionItemUpdateCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockOptionItemService.Object,
            mockCategoryService.Object,
            mockQuestionService.Object,
            mockOptionItemCategoryService.Object,
            mockOptionItemQuestionService.Object,
            mockFileOptionItemService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotfoundException>(
            () =>
                handler.Handle(command, CancellationToken.None)
        );
    }
}
