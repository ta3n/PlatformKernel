using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class CancellationGetAllQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnCancellationId_WhenCommandValid()
    {
        // Arrange
        var cancellationId = 1;

        // Mocks
        var mockCancellationRepository = new Mock<ICancellationRepository>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        //var mockPage = new Mock<IPageable>();

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(1);

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Cancellation, CancellationPolicyResponse>();
            }
        );

        var cancellation = new List<Cancellation>
        {
            new()
            {
                Id = cancellationId,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                IsEnabled = true,
                FacilityCancellations =
                [
                    new()
                    {
                        FacilityId = 1,
                        CancellationId = cancellationId
                    }
                ]
            }
        };

        mockCancellationRepository.Setup(s => s.GetQueryableWithAsNoTracking())
            .Returns(cancellation.AsQueryable().BuildMock());

        var command = new CancellationPolicyGetAllQuery(PageableConstants.UnPaged);
        var handler = new CancellationPolicyGetAllQueryHandler(
            config.CreateMapper(),
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
