using AutoMapper;
using Moq;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MasterCalendar;
using Liberty.Cache.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using MockQueryable;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class MasterCalendarGetAllQueryHandlerTests
{
    private readonly IMapper _mapper;

    public MasterCalendarGetAllQueryHandlerTests()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<AppDate, DateOfMasterCalendarResponse>()
                    .ConstructUsing(
                        src => new DateOfMasterCalendarResponse(
                            src.Id,
                            src.AppDateAppDateTypes!.FirstOrDefault()!.AppDateType!.Name,
                            src.AppDateAppDateTypes!.FirstOrDefault()!.AppDateType!.ShortName,
                            src.AppDateAppDateTypes!.FirstOrDefault()!.AppDateType!.Color,
                            src.AppDateAppDateDatas!.FirstOrDefault()!.AppDateData!.Name
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedResponse_WhenDataExists()
    {
        // Arrange
        var mockCacheService = new Mock<ICacheService>();
        var mockAppDateRepository = new Mock<IAppDateRepository>();

        var sampleData = new List<AppDate>
        {
            new()
            {
                Id = 10,
                AppDateAppDateDatas = new List<AppDateAppDateData> { new() { AppDateData = new AppDateData { Name = "Data1" } } },
                AppDateAppDateTypes = new List<AppDateAppDateType> { new() { AppDateType = new AppDateType { IsEnabled = false } } }
            },
            new()
            {
                Id = 20,
                AppDateAppDateDatas = new List<AppDateAppDateData>(),
                AppDateAppDateTypes = new List<AppDateAppDateType> { new() { AppDateType = new AppDateType { IsEnabled = false } } }
            },
            new()
            {
                Id = 30,
                AppDateAppDateDatas = new List<AppDateAppDateData>(),
                AppDateAppDateTypes = new List<AppDateAppDateType> { new() { AppDateType = new AppDateType { IsEnabled = true } } }
            }
        };

        mockAppDateRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(sampleData.AsQueryable().BuildMock());

        var query = new MasterCalendarGetAllQuery(
            PageableBinderConfig.DefaultPageable,
            20250101,
            20250102
        );

        var handler = new MasterCalendarGetAllQueryHandler(_mapper, mockCacheService.Object, mockAppDateRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var data = result.Item2.ToList();
        Assert.NotNull(data);
    }
}
