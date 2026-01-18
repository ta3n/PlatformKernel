using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class PlanGetAllDestinationQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnResponse_WhenCommandValid()
    {
        var planId = 1;

        var mockSiteRepository = new Mock<ISiteRepository>();
        var mockAccessor = new Mock<ISecurityContextAccessor>();
        var mockPage = new Mock<IPageable>();

        mockAccessor.Setup(s => s.FacilityKey).Returns(1);

        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Site, SiteResponse>();
            }
        );

        var sites = new List<Site>
        {
            new()
            {
                Id = 1,
                PlanSites =
                [
                    new()
                    {
                        PlanId = planId,
                        SiteId = 1
                    }
                ],
                FacilitySites =
                [
                    new()
                    {
                        FacilityId = 1,
                        SiteId = 1,
                        IsDeleted = false
                    }
                ],
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                ShortName = "Test",
                Url = "Test.com",
                Description = "Test"
            }
        };

        mockSiteRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(sites.AsQueryable().BuildMock());

        var command = new PlanGetAllDestinationsQuery(1, mockPage.Object);
        var handler = new PlanGetAllDestinationsQueryHandler(
            config.CreateMapper(),
            mockAccessor.Object,
            mockSiteRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
