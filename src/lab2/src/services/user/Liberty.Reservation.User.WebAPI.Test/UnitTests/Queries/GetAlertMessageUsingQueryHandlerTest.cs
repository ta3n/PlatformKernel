using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.AlertMessage;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Queries;

public class GetAlertMessageUsingQueryHandlerTest
{
    private readonly IMapper _mapper;

    public GetAlertMessageUsingQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<AlertMessage, AlertMessageResponse>();
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAlertMessageResponses_WhenDataExists()
    {
        var mockAlertMessageRepository = new Mock<IAlertMessageRepository>();
        var mockCacheService = new Mock<ICacheService>();

        var alertMessages = new List<AlertMessage>
        {
            new()
            {
                Id = 1,
                Title = new MultilingualText { { TestUtil.DefaultLanguageCode, "Alert 1" } },
                Content = new MultilingualText { { TestUtil.DefaultLanguageCode, "Content 1" } },
                IsEnabled = true
            }
        };
        mockAlertMessageRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(alertMessages.AsQueryable().BuildMock());

        var handler = new GetAllAlertMessageActiveQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockAlertMessageRepository.Object
        );

        var query = new GetAllAlertMessageActiveQuery(TestUtil.DefaultLanguageCode);

        // Act
        var (_, response) = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.NotNull(response);
        Assert.Single(response);
    }
}
