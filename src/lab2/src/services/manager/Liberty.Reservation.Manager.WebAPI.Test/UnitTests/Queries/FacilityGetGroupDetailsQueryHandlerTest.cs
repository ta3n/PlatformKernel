using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class FacilityGetGroupDetailsQueryHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityGetGroupDetailsQueryHandlerTest()
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
    public async Task HandleAsync_ShouldReturnFacilityDetailAcceptResponse_WhenFacilityExists()
    {
        // Arrange
        var facilityId = 1L;
        var facility = new Facility
        {
            Id = facilityId,
            Code = "F001",
            BarrierFreeInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Meta = new FacilityMeta
            {
                IsAcceptChildren = true,
                AcceptChildrenInfoComment = "Children are welcome",
                IsAcceptPet = false,
                AcceptPetInfoComment = "Pets are not allowed",
                IsBarrierFree = true
            }
        };

        var facilities = new List<Facility> { facility }.AsQueryable().BuildMock();

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

        // Act
        var (_, response) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }
}
