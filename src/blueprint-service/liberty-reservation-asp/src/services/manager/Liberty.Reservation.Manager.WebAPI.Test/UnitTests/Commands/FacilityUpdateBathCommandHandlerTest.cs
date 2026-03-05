using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class FacilityUpdateBathCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdateBathCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilityUpdateBathRequest, Facility>()
                    .ForPath(
                        dest => dest.Meta!.SpaType,
                        opt => opt.MapFrom(
                            src => src.SpaType.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.SpaName,
                        opt => opt.MapFrom(
                            src => src.SpaName.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.SpaInfoComment,
                        opt => opt.MapFrom(
                            src => src.SpaInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.SpaDescription,
                        opt => opt.MapFrom(
                            src => src.SpaDescription.HtmlSanitize()
                        )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateFacility_WhenFacilityExists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        var facilityId = 1L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockFacilityService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var facility = new Facility { Id = facilityId };
        mockFacilityService.Setup(
                s => s.UpdateBathAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(facility);

        var handler = new FacilityUpdateBathCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateBathRequest(
            "Type",
            "Spacious bath available",
            "Comment",
            "Shower with hot water"
        );

        var command = new FacilityUpdateBathCommand { Payload = payload };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(facilityId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotFoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        var facilityId = 1L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockFacilityService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var facility = new Facility { Id = facilityId };
        mockFacilityService.Setup(
                s => s.UpdateBathAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(facility);

        var handler = new FacilityUpdateBathCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateBathRequest(
            "Type",
            "Spacious bath available",
            "Comment",
            "Shower with hot water"
        );

        var command = new FacilityUpdateBathCommand { Payload = payload };

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
