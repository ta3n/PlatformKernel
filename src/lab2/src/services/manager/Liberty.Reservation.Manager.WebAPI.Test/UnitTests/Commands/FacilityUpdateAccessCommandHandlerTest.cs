using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class FacilityUpdateAccessCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdateAccessCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilityUpdateAccessRequest, Facility>()
                    .ForMember(
                        dest => dest.Latitude,
                        opt => opt.MapFrom(
                            src => src.Latitude
                        )
                    )
                    .ForMember(
                        dest => dest.Longitude,
                        opt => opt.MapFrom(
                            src => src.Longitude
                        )
                    )
                    .ForPath(
                        dest => dest!.AccessInfoComment,
                        opt => opt.MapFrom(
                            src => new MultilingualText
                            {
                                { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.AccessInfoComment.HtmlSanitize() ?? string.Empty }
                            }
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.ExistsParking,
                        opt => opt.MapFrom(
                            src => src.ExistsParking
                        )
                    )
                    .ForPath(
                        dest => dest!.ParkingInfoComment,
                        opt => opt.MapFrom(
                            src => new MultilingualText
                            {
                                {
                                    LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.ParkingInfoComment.HtmlSanitize() ?? string.Empty
                                }
                            }
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.CanTransfer,
                        opt => opt.MapFrom(
                            src => src.CanTransfer
                        )
                    )
                    .ForPath(
                        dest => dest!.TransferComment,
                        opt => opt.MapFrom(
                            src => new MultilingualText
                            {
                                { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.TransferComment.HtmlSanitize() ?? string.Empty }
                            }
                        )
                    )
                    .ForPath(
                        dest => dest!.NearStationInfoComment,
                        opt => opt.MapFrom(
                            src => new MultilingualText
                            {
                                {
                                    LanguageHeaderUtil.GetLanguageCodeFromHeader(),
                                    src.NearStationInfoComment.HtmlSanitize() ?? string.Empty
                                }
                            }
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
                s => s.UpdateAccessAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(facility);

        var handler = new FacilityUpdateAccessCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateAccessRequest(
            40.7128f,
            -74.0060f,
            "Accessible facility with ramps.",
            true,
            "Parking available in the back.",
            true,
            "Transfer service available at the entrance.",
            "5 minutes walk from the nearest subway station."
        );

        var command = new FacilityUpdateAccessCommand { Payload = payload };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(facilityId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotFoundException_WhenFacilityDoesNotExist()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        var facilityId = 1L;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockFacilityService.Setup(s => s.CountByIdsAsync(It.IsAny<long[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var facility = new Facility { Id = facilityId };
        mockFacilityService.Setup(
                s => s.UpdateAccessAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(facility);

        var handler = new FacilityUpdateAccessCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateAccessRequest(
            40.7128f,
            -74.0060f,
            "Accessible facility with ramps.",
            true,
            "Parking available in the back.",
            true,
            "Transfer service available at the entrance.",
            "5 minutes walk from the nearest subway station."
        );

        var command = new FacilityUpdateAccessCommand { Payload = payload };

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
