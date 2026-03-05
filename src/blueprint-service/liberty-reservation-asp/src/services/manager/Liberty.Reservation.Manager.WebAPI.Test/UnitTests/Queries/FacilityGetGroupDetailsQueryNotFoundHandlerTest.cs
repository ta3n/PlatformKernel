using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class FacilityGetGroupDetailsQueryNotFoundHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityGetGroupDetailsQueryNotFoundHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Facility, FacilityDetailAcceptResponse>()
                    .ForMember(
                        dest => dest.Code,
                        opt => opt.MapFrom(
                            src => src.Code
                        )
                    )
                    .ForMember(
                        dest => dest.IsAcceptChildren,
                        opt => opt.MapFrom(
                            src => src.Meta!.IsAcceptChildren
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptChildrenInfoComment,
                        opt => opt.MapFrom(
                            src => src.Meta!.AcceptChildrenInfoComment
                        )
                    )
                    .ForMember(
                        dest => dest.IsAcceptPet,
                        opt => opt.MapFrom(
                            src => src.Meta!.IsAcceptPet
                        )
                    )
                    .ForMember(
                        dest => dest.AcceptPetInfoComment,
                        opt => opt.MapFrom(
                            src => src.Meta!.AcceptPetInfoComment
                        )
                    )
                    .ForMember(
                        dest => dest.IsBarrierFree,
                        opt => opt.MapFrom(
                            src => src.Meta!.IsBarrierFree
                        )
                    )
                    .ForMember(
                        dest => dest.BarrierFreeInfoComment,
                        opt => opt.MapFrom(
                            src => src.BarrierFreeInfoComment!.GetValueByHeader()
                        )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityId = 1;
        var facilities = new List<Facility>
            {
                new()
                {
                    Id = 2, // Different ID to simulate non-existence of the requested facility
                    Code = "F002",
                    BarrierFreeInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                    IsEnabled = true
                },
                new()
                {
                    Id = 3, // Another facility to ensure the requested one is not found
                    Code = "F003",
                    BarrierFreeInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                    IsEnabled = true
                }
            }
            .AsQueryable()
            .BuildMock();

        var mockRepository = new Mock<IFacilityRepository>();
        mockRepository.Setup(r => r.GetQueryableWithAsNoTracking()).Returns(facilities);

        var mockCacheService = new Mock<ICacheService>();

        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var handler = new FacilityGetGroupDetailsQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockRepository.Object
        );

        var query = new FacilityGetGroupDetailsQuery(GroupOfFacility.Accept);

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
