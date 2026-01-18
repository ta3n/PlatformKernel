using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class CancellationGetQueryHandlerTest
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
                cfg.CreateMap<Cancellation, CancellationPolicyDetailResponse>()
                    .ConstructUsing(
                        src => new CancellationPolicyDetailResponse(
                            src.Id,
                            src.Name!.GetValueByHeader(),
                            src.Description!.GetValueByHeader(),
                            src.RuleDetail!.GetValueByHeader(),
                            new CancellationMeta(src.TableSource!.GetValueByHeader()),
                            src.CancellationCancellationDatas!
                                .Select(
                                    x => new CancellationDataResponse(
                                        x.CancellationData!.Id,
                                        x.CancellationData.DayStart,
                                        x.CancellationData.DayEnd,
                                        x.CancellationData.Rate,
                                        x.CancellationData.Description!.GetValueByHeader()
                                    )
                                )
                        )
                    );
            }
        );

        var cancellation = new List<Cancellation>
        {
            new()
            {
                Id = cancellationId,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                RuleDetail = new MultilingualText { { TestUtil.DefaultLanguageCode, "RuleDetail" } },
                TableSource = new MultilingualText { { TestUtil.DefaultLanguageCode, "TableSource" } },
                IsEnabled = true,
                FacilityCancellations =
                [
                    new()
                    {
                        FacilityId = 1,
                        CancellationId = cancellationId
                    }
                ],
                CancellationCancellationDatas =
                [
                    new()
                    {
                        CancellationId = cancellationId,
                        CancellationData = new CancellationData
                        {
                            Id = 1,
                            DayStart = 20250101,
                            DayEnd = 20250202,
                            Rate = 10,
                            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } }
                        }
                    }
                ]
            }
        };

        mockCancellationRepository.Setup(s => s.GetQueryableWithAsNoTracking())
            .Returns(cancellation.AsQueryable().BuildMock());

        var command = new CancellationPolicyGetQuery(1);
        var handler = new CancellationPolicyGetQueryHandler(
            config.CreateMapper(),
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationRepository.Object
        );

        var (_, result) = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }
}
