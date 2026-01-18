using AutoMapper;
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

public class FacilityUpdateBasicSettingCommandHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityUpdateBasicSettingCommandHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                // Map your request to the entity
                cfg.CreateMap<FacilityUpdateBasicSettingRequest, Facility>()
                    .ForMember(
                        dest => dest.Description,
                        opt => opt.MapFrom(
                            src => src.Description
                        )
                    )
                    .ForMember(
                        dest => dest.Fax,
                        opt => opt.MapFrom(
                            src => src.Fax
                        )
                    )
                    .ForMember(
                        dest => dest.Url,
                        opt => opt.MapFrom(
                            src => src.Url
                        )
                    )
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                        )
                    )
                    .ForMember(
                        dest => dest.Kana,
                        opt => opt.MapFrom(
                            src => src.Kana
                        )
                    )
                    .ForMember(
                        dest => dest.Phone,
                        opt => opt.MapFrom(
                            src => src.Phone
                        )
                    )
                    .ForMember(
                        dest => dest.Postcode,
                        opt => opt.MapFrom(
                            src => src.Postcode
                        )
                    )
                    .ForMember(
                        dest => dest.Address1,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address1 ?? string.Empty } }
                        )
                    )
                    .ForMember(
                        dest => dest.Address2,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address2 ?? string.Empty } }
                        )
                    )
                    .ForMember(
                        dest => dest.Address3,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address3 ?? string.Empty } }
                        )
                    )
                    .ForMember(
                        dest => dest.Address4,
                        opt => opt.MapFrom(
                            src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Address4 ?? string.Empty } }
                        )
                    )
                    .ForMember(
                        dest => dest.AreaId,
                        opt => opt.MapFrom(
                            src => src.AreaId
                        )
                    )
                    .ForPath(
                        dest => dest.CategoryId,
                        opt => opt.MapFrom(
                            src => src.FacilityTypeId
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.RoomNumberWesternStyle,
                        opt => opt.MapFrom(
                            src => src.RoomNumberWesternStyle
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.RoomNumberJapaneseStyle,
                        opt => opt.MapFrom(
                            src => src.RoomNumberJapaneseStyle
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.RoomNumberJapaneseWesternStyle,
                        opt => opt.MapFrom(
                            src => src.RoomNumberJapaneseWesternStyle
                        )
                    )
                    .ForPath(
                        dest => dest.Meta!.RoomNumberOtherStyle,
                        opt => opt.MapFrom(
                            src => src.RoomNumberOtherStyle
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
                s => s.UpdateBasicSettingAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(facility);

        var handler = new FacilityUpdateBasicSettingCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateBasicSettingRequest(
            "A beautiful facility.",
            "123-456-7890",
            "https://example.com",
            "Test Facility",
            "テスト施設",
            "123-4567",
            "123 Test St",
            "Suite 101",
            "Shibuya",
            "Tokyo",
            "098-765-4321",
            10,
            5,
            3,
            2,
            1L,
            2L,
            new FileOfFacilityRequest("test"),
            TimeSpan.Zero,
            "Asia/Tokyo"
        );

        var command = new FacilityUpdateBasicSettingCommand { Payload = payload };

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
                s => s.UpdateBasicSettingAsync(
                    It.IsAny<Facility>(),
                    It.IsAny<bool>(),
                    It.IsAny<Func<Facility, Facility, Facility>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(facility);

        var handler = new FacilityUpdateBasicSettingCommandHandler(
            mockUnitOfWork.Object,
            _mapper,
            mockSecurityContextAccessor.Object,
            mockFacilityService.Object
        );

        var payload = new FacilityUpdateBasicSettingRequest(
            "A beautiful facility.",
            "123-456-7890",
            "https://example.com",
            "Test Facility",
            "テスト施設",
            "123-4567",
            "123 Test St",
            "Suite 101",
            "Shibuya",
            "Tokyo",
            "098-765-4321",
            10,
            5,
            3,
            2,
            1L,
            2L,
            new FileOfFacilityRequest("test"),
            TimeSpan.Zero,
            "Asia/Tokyo"
        );

        var command = new FacilityUpdateBasicSettingCommand { Payload = payload };

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
