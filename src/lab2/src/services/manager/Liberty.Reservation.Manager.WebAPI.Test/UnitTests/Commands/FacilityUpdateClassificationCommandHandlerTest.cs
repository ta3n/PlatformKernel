using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.UnitOfWork.Abstractions;
using Moq;
using Microsoft.Extensions.Logging;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Exceptions;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class FacilityUpdateClassificationCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdateClassificationCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            _ =>
            {
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateFacilityClassification_WhenAllValid()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<FacilityUpdateClassificationCommandHandler>>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityAllergenService = new Mock<IFacilityAllergenService>();
        var mockFacilityCategoryService = new Mock<IFacilityCategoryService>();
        var mockAllergenService = new Mock<IAllergenService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockCacheService = new Mock<ICacheService>();
        var facilityId = 1L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var payload = new FacilityUpdateClassificationRequest(
            [1],
            [
                4,
                5,
                6
            ],
            [
                7,
                8
            ],
            [9],
            [
                10,
                11
            ],
            [12],
            [
                13,
                14
            ],
            [
                15,
                16,
                17
            ]
        );

        var command = new FacilityUpdateClassificationCommand { Payload = payload };

        // Mock service responses
        mockAllergenService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockCategoryService.Setup(
                s => s.CountByIdsWithoutFacilityAsync(
                    It.IsAny<long[]>(),
                    It.IsAny<CategoryTypes[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(14);

        mockFacilityAllergenService.Setup(
                s => s.ChangeFacilityAllergenAsync(
                    It.IsAny<long>(),
                    It.IsAny<long[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FacilityAllergen>().AsEnumerable(), new List<FacilityAllergen>().AsEnumerable()));

        mockFacilityCategoryService.Setup(
                s => s.ChangeFacilityCategoryAsync(
                    It.IsAny<long>(),
                    It.IsAny<List<long>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FacilityCategory>().AsEnumerable(), new List<FacilityCategory>().AsEnumerable()));

        mockCacheService.Setup(s => s.ResetAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var mockFacilityService = new Mock<IFacilityService>();
        mockFacilityService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityAllergenService)))
            .Returns(mockFacilityAllergenService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityCategoryService)))
            .Returns(mockFacilityCategoryService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAllergenService)))
            .Returns(mockAllergenService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICategoryService)))
            .Returns(mockCategoryService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityService)))
            .Returns(mockFacilityService.Object);

        var handler = new FacilityUpdateClassificationCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockLogger.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object,
            mockCacheService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(facilityId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowAllergenNotfoundException_WhenAllergensNotFound()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<FacilityUpdateClassificationCommandHandler>>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityAllergenService = new Mock<IFacilityAllergenService>();
        var mockFacilityCategoryService = new Mock<IFacilityCategoryService>();
        var mockAllergenService = new Mock<IAllergenService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockCacheService = new Mock<ICacheService>();

        var facilityId = 1L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var payload = new FacilityUpdateClassificationRequest(
            [1],
            [
                4,
                5,
                6
            ],
            [
                7,
                8
            ],
            [9],
            [
                10,
                11
            ],
            [12],
            [
                13,
                14
            ],
            [
                15,
                16,
                17
            ]
        );

        var command = new FacilityUpdateClassificationCommand { Payload = payload };

        // Mock service responses
        mockAllergenService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        mockCategoryService.Setup(
                s => s.CountByIdsWithoutFacilityAsync(
                    It.IsAny<long[]>(),
                    It.IsAny<CategoryTypes[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(14);

        mockFacilityAllergenService.Setup(
                s => s.ChangeFacilityAllergenAsync(
                    It.IsAny<long>(),
                    It.IsAny<long[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FacilityAllergen>().AsEnumerable(), new List<FacilityAllergen>().AsEnumerable()));

        mockFacilityCategoryService.Setup(
                s => s.ChangeFacilityCategoryAsync(
                    It.IsAny<long>(),
                    It.IsAny<List<long>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FacilityCategory>().AsEnumerable(), new List<FacilityCategory>().AsEnumerable()));

        mockCacheService.Setup(s => s.ResetAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockFacilityService = new Mock<IFacilityService>();
        mockFacilityService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityAllergenService)))
            .Returns(mockFacilityAllergenService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityCategoryService)))
            .Returns(mockFacilityCategoryService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAllergenService)))
            .Returns(mockAllergenService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICategoryService)))
            .Returns(mockCategoryService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityService)))
            .Returns(mockFacilityService.Object);

        var handler = new FacilityUpdateClassificationCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockLogger.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object,
            mockCacheService.Object
        );
        // Act & Assert
        await Assert.ThrowsAsync<AllergenNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCategoryNotfoundException_WhenCategoriesNotFound()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<FacilityUpdateClassificationCommandHandler>>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityAllergenService = new Mock<IFacilityAllergenService>();
        var mockFacilityCategoryService = new Mock<IFacilityCategoryService>();
        var mockAllergenService = new Mock<IAllergenService>();
        var mockCategoryService = new Mock<ICategoryService>();
        var mockCacheService = new Mock<ICacheService>();

        var facilityId = 1L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var payload = new FacilityUpdateClassificationRequest(
            [1],
            [
                4,
                5,
                6
            ],
            [
                7,
                8
            ],
            [9],
            [
                10,
                11
            ],
            [12],
            [
                13,
                14
            ],
            [
                15,
                16,
                17
            ]
        );

        var command = new FacilityUpdateClassificationCommand { Payload = payload };

        // Mock service responses
        mockAllergenService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockCategoryService.Setup(
                s => s.CountByIdsWithoutFacilityAsync(
                    It.IsAny<long[]>(),
                    It.IsAny<CategoryTypes[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(11);

        mockFacilityAllergenService.Setup(
                s => s.ChangeFacilityAllergenAsync(
                    It.IsAny<long>(),
                    It.IsAny<long[]>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FacilityAllergen>().AsEnumerable(), new List<FacilityAllergen>().AsEnumerable()));

        mockFacilityCategoryService.Setup(
                s => s.ChangeFacilityCategoryAsync(
                    It.IsAny<long>(),
                    It.IsAny<List<long>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<FacilityCategory>().AsEnumerable(), new List<FacilityCategory>().AsEnumerable()));

        mockCacheService.Setup(s => s.ResetAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var mockFacilityService = new Mock<IFacilityService>();
        mockFacilityService
            .Setup(f => f.UpdateLastModifiedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityAllergenService)))
            .Returns(mockFacilityAllergenService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityCategoryService)))
            .Returns(mockFacilityCategoryService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAllergenService)))
            .Returns(mockAllergenService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICategoryService)))
            .Returns(mockCategoryService.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IFacilityService)))
            .Returns(mockFacilityService.Object);

        var handler = new FacilityUpdateClassificationCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockLogger.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object,
            mockCacheService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
