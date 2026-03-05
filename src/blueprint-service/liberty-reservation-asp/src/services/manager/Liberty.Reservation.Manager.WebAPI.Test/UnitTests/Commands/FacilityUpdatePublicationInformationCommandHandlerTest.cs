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

public class FacilityUpdatePublicationInformationCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdatePublicationInformationCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<FacilityUpdatePublicationInformationRequest, Facility>()
                    .ForPath(
                        dest => dest.Heading1,
                        opt => opt.MapFrom(
                            src => new MultilingualText
                            {
                                { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Heading1.HtmlSanitize() ?? string.Empty }
                            }
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.PRPointComment,
                        opt => opt.MapFrom(
                            src => src.PrPointComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.EquipmentInfoComment,
                        opt => opt.MapFrom(
                            src => src.EquipmentInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.RoomInfoComment,
                        opt => opt.MapFrom(
                            src => src.RoomInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.AmenityInfoComment,
                        opt => opt.MapFrom(
                            src => src.AmenityInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.LeisureInfoComment,
                        opt => opt.MapFrom(
                            src => src.LeisureInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.FAQInfoComment,
                        opt => opt.MapFrom(
                            src => src.FaqInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.OtherInfoComment,
                        opt => opt.MapFrom(
                            src => src.OtherInfoComment.HtmlSanitize()
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.MapUrl,
                        opt => opt.MapFrom(
                            src => src.MapUrl
                        )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdatePublicationInformation_WhenFacilityExists()
    {
        // Arrange
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        // InfrastructureOfTest: facility exists.
        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var payload = new FacilityUpdatePublicationInformationRequest("New Publication Info", "", "", "", "", "", "", "", "");
        var command = new FacilityUpdatePublicationInformationCommand { Payload = payload };

        var updatedFacility = new Facility { Id = facilityId };

        mockFacilityService
            .Setup(
                s => s.UpdatePublicationInformationAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        var handler = new FacilityUpdatePublicationInformationCommandHandler(
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
        // Arrange
        var facilityId = 1L;
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        // InfrastructureOfTest: facility exists.
        mockFacilityService
            .Setup(s => s.CountByIdsAsync(It.Is<long[]>(ids => ids.Length == 1 && ids[0] == facilityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var payload = new FacilityUpdatePublicationInformationRequest("New Publication Info", "", "", "", "", "", "", "", "");
        var command = new FacilityUpdatePublicationInformationCommand { Payload = payload };

        var updatedFacility = new Facility { Id = facilityId };

        mockFacilityService
            .Setup(
                s => s.UpdatePublicationInformationAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedFacility);

        var handler = new FacilityUpdatePublicationInformationCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );
        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
