using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class CancellationPolicyGetAllQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnCancellationPolicies_WhenValidRequest()
    {
        // Arrange
        var facilityId = 100;
        var request = new CancellationPolicyGetAllQuery(PageableBinderConfig.DefaultPageable);

        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationRepository = new Mock<ICancellationRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var fakeCancellations = new List<Cancellation>
            {
                new()
                {
                    Id = 1,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                    IsEnabled = true,
                    DisplayOrder = 1,
                    FacilityCancellations = new List<FacilityCancellation> { new() { FacilityId = facilityId } }
                },
                new()
                {
                    Id = 2,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                    IsEnabled = true,
                    DisplayOrder = 2,
                    FacilityCancellations = new List<FacilityCancellation> { new() { FacilityId = facilityId } }
                }
            }.AsQueryable()
            .BuildMock();

        mockCancellationRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(fakeCancellations);

        mockMapper.Setup(m => m.Map<IEnumerable<CancellationPolicyResponse>>(It.IsAny<IEnumerable<Cancellation>>()))
            .Returns(
                new List<CancellationPolicyResponse>
                {
                    new(1, "Policy 1", true, string.Empty, "a"),
                    new(2, "Policy 2", true, string.Empty, "a")
                }
            );
        mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                        cfg.CreateMap<Cancellation, CancellationPolicyResponse>();
                    }
                )
            );

        var handler = new CancellationPolicyGetAllQueryHandler(
            mockMapper.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationRepository.Object
        );

        // Act
        var (_, data) = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(data);
    }
}
