using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class FacilityGetMinimumPriceQueryHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityGetMinimumPriceQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Facility, FacilityMinimumPriceResponse>()
                    .ForMember(
                        dest => dest.Code,
                        opt => opt.MapFrom(
                            src => src.Code
                        )
                    )
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
    public async Task HandleAsync_ShouldReturnFacilityMinimumPriceResponse()
    {
        var mockFacilityRepository = new Mock<IFacilityRepository>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        var facilityId = 1;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);
        var facility = new Facility
        {
            Id = facilityId,
            IsEnabledMinimumPrice = false,
            MinimumPrice = 1000,
            IsEnabled = true,
            Code = "ABC123"
        };

        var mockFacilityBuild = new List<Facility> { facility }.AsQueryable().BuildMock();
        mockFacilityRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockFacilityBuild);

        var request = new FacilityGetMinimumPriceQuery();
        var handler = new FacilityGetMinimumPriceQueryHandler(
            _mapper,
            mockFacilityRepository.Object,
            mockSecurityContextAccessor.Object
        );
        // Act
        var (_, response) = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }
}
