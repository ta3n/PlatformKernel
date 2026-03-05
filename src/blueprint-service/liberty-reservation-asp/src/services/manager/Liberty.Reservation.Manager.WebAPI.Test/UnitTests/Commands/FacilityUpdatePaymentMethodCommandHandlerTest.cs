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

public class FacilityUpdatePaymentMethodCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdatePaymentMethodCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilityUpdatePaymentMethodRequest, Facility>()
                    .ForMember(
                        dest => dest.IsOnSidePayment,
                        opt => opt.MapFrom(
                            src => src.IsOnSidePayment
                        )
                    )
                    .ForMember(
                        dest => dest.IsOnLinePayment,
                        opt => opt.MapFrom(
                            src => src.IsOnLinePayment
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.OnSidePaymentComment,
                        opt => opt.MapFrom(
                            src => src.OnSidePaymentComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.OnLinePaymentComment,
                        opt => opt.MapFrom(
                            src => src.OnLinePaymentComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.PaymentComment,
                        opt => opt.MapFrom(
                            src => src.PaymentComment.HtmlSanitize()
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityId_WhenFacilityExists()
    {
        // Arrange
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        // InfrastructureOfTest the security context to return our facility ID.
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        // Simulate that the facility exists.
        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // InfrastructureOfTest the update service to return an updated facility.
        var updatedFacility = new Facility { Id = facilityId };
        mockFacilityService
            .Setup(
                s => s.UpdatePaymentMethodAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        var handler = new FacilityUpdatePaymentMethodCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdatePaymentMethodRequest(true, false, "", "", "comment");
        var command = new FacilityUpdatePaymentMethodCommand { Payload = payload };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(facilityId, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var updatedFacility = new Facility { Id = facilityId };
        mockFacilityService
            .Setup(
                s => s.UpdatePaymentMethodAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        var handler = new FacilityUpdatePaymentMethodCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdatePaymentMethodRequest(true, false, "", "", "comment");
        var command = new FacilityUpdatePaymentMethodCommand { Payload = payload };
        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
