using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;
using Moq;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using MockQueryable;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class MailTemplateGetQueryHandlerTests
{
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<ICacheService> _mockCacheService = new();
    private readonly Mock<ISystemConfigRepository> _mockSystemConfigRepository = new();

    [Fact]
    public async Task HandleAsync_ShouldReturnMailTemplateResponse_WhenTemplateFound()
    {
        // Arrange
        var query = new MailTemplateGetQuery("IO10001");
        var systemConfig = new SystemConfig { TemplateFormatData = new TemplateFormatData() };

        _mockSystemConfigRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(Enumerable.Repeat(systemConfig, 1).AsQueryable().BuildMock());

        // Act
        var handler = new MailTemplateGetQueryHandler(_mockMapper.Object, _mockCacheService.Object, _mockSystemConfigRepository.Object);
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Item2);
    }
}
