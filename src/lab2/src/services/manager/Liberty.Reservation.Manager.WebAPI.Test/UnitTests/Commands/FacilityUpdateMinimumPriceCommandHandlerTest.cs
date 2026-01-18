using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.UnitOfWork.Abstractions;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class FacilityUpdateMinimumPriceCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdateMinimumPriceCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilityUpdateMinimumPriceRequest, Facility>()
                    .ForMember(
                        dest => dest.IsEnabledMinimumPrice,
                        opt => opt.MapFrom(
                            src => src.IsEnabledMinimumPrice
                        )
                    )
                    .ForMember(
                        dest => dest.MinimumPrice,
                        opt => opt.MapFrom(
                            src => src.MinimumPrice
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityId_WhenFacilityUpdateMinimumPrice()
    {
        // Arrange
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var updatedFacility = new Facility
        {
            Id = facilityId,
            IsEnabledMinimumPrice = true,
            MinimumPrice = 1000,
            Code = "ABC123"
        };
        mockFacilityService
            .Setup(
                s => s.UpdateMinimumPriceAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new FacilityUpdateMinimumPriceCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateMinimumPriceRequest(true, 1000);
        var command = new FacilityUpdateMinimumPriceCommand { Payload = payload };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(facilityId, result);
    }
}
