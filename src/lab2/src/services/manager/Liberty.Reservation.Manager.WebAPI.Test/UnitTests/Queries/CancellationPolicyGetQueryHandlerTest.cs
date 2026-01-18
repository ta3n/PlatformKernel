using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class CancellationPolicyGetQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnCancellationPolicy_WhenValidRequest()
    {
        // Arrange
        var facilityId = 100;
        var cancellationId = 1;
        var request = new CancellationPolicyGetQuery(cancellationId);

        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationRepository = new Mock<ICancellationRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var fakeCancellations = new List<Cancellation>
            {
                new()
                {
                    Id = cancellationId,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "TestName" } },
                    Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                    RuleDetail = new MultilingualText { { TestUtil.DefaultLanguageCode, "RuleDetail" } },
                    TableSource = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                    FacilityCancellations = new List<FacilityCancellation> { new() { FacilityId = facilityId } },
                    CancellationCancellationDatas = new List<CancellationCancellationData>
                    {
                        new()
                        {
                            CancellationData = new CancellationData
                            {
                                Id = 1,
                                DayStart = 1,
                                DayEnd = 2,
                                Rate = 10,
                                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } }
                            }
                        }
                    }
                }
            }.AsQueryable()
            .BuildMock();

        var mockDbSet = new Mock<DbSet<Cancellation>>();
        mockDbSet.As<IQueryable<Cancellation>>().Setup(m => m.Provider).Returns(fakeCancellations.Provider);
        mockDbSet.As<IQueryable<Cancellation>>().Setup(m => m.Expression).Returns(fakeCancellations.Expression);
        mockDbSet.As<IQueryable<Cancellation>>().Setup(m => m.ElementType).Returns(fakeCancellations.ElementType);
        using var enumerator = fakeCancellations.GetEnumerator();
        mockDbSet.As<IQueryable<Cancellation>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        mockCancellationRepository.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(fakeCancellations);

        mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
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
                )
            );

        var handler = new CancellationPolicyGetQueryHandler(
            mockMapper.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationRepository.Object
        );

        // Act
        var (_, data) = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(data);
        Assert.Equal(cancellationId, data.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCancellationNotFoundException_WhenCancellationDoesNotExist()
    {
        // Arrange
        var facilityId = 100;
        var request = new CancellationPolicyGetQuery(999);

        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockCancellationRepository = new Mock<ICancellationRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);

        var fakeCancellations = new List<Cancellation>().AsQueryable().BuildMock();

        mockCancellationRepository.Setup(x => x.GetQueryableWithAsNoTracking()).Returns(fakeCancellations);
        mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
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
                )
            );

        var handler = new CancellationPolicyGetQueryHandler(
            mockMapper.Object,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockCancellationRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<CancellationNotfoundException>(() => handler.Handle(request, CancellationToken.None));
    }
}
