using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class FacilityUpdateReservationSettingCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdateReservationSettingCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilityUpdateReservationSettingRequest, Facility>()
                    .ForMember(
                        dest => dest.UseDailyPerson,
                        opt => opt.MapFrom(
                            src => src.UseDailyPerson
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
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var payload = new FacilityUpdateReservationSettingRequest(true);
        var command = new FacilityUpdateReservationSettingCommand { Payload = payload };

        var updatedFacility = new Facility { Id = facilityId };
        mockFacilityService
            .Setup(
                s => s.UpdateReservationSettingAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        var handler = new FacilityUpdateReservationSettingCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(facilityId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityDoesNotExist()
    {
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var payload = new FacilityUpdateReservationSettingRequest(true);
        var command = new FacilityUpdateReservationSettingCommand { Payload = payload };

        var updatedFacility = new Facility { Id = facilityId };
        mockFacilityService
            .Setup(
                s => s.UpdateReservationSettingAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        var handler = new FacilityUpdateReservationSettingCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
