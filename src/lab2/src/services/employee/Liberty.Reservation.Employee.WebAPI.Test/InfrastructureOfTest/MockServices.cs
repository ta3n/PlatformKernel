using AutoMapper;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

public abstract class MockServices
{
    public static Mock<IHttpContextAccessor> MockHttpContextAccessor()
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        return mockHttpContextAccessor;
    }

    public static Mock<IActionResultExecutor<ObjectResult>> MockActionResultExecutor()
    {
        return new Mock<IActionResultExecutor<ObjectResult>>();
    }

    public static Mock<ILoggerFactory> MockLoggerFactory()
    {
        var mockLoggerFactory = new Mock<ILoggerFactory>();
        var mockLogger = new Mock<ILogger>();

        mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);

        return mockLoggerFactory;
    }

    public static IMailTypeService MockMailTypeService(
        List<(string IoType, string Name)> data
    )
    {
        var mockMailTypeService = new Mock<IMailTypeService>();

        mockMailTypeService.Setup(x => x.FindAll())
            .Returns(data);

        return mockMailTypeService.Object;
    }

    public static IMapper MockMapper<TSource, TDestination>(
        params (TSource source, TDestination destination)[] mappings
    )
    {
        var mockMapper = new Mock<IMapper>();

        foreach (var (_, destination) in mappings)
        {
            mockMapper.Setup(m => m.Map<TDestination>(It.IsAny<TSource>()))
                .Returns(destination);
        }

        return mockMapper.Object;
    }

    public static IMapper MockMapper()
    {
        var mockMapper = new Mock<IMapper>();

        return mockMapper.Object;
    }

    public static IMediator MockMediator<TRequest, TResponse>(
        params (TRequest request, TResponse response)[] mappings
    )
        where TRequest : IRequest<TResponse>
    {
        var mockMediator = new Mock<IMediator>();

        foreach (var (request, response) in mappings)
        {
            mockMediator.Setup(m => m.Send(It.Is<TRequest>(r => r.Equals(request)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
        }

        return mockMediator.Object;
    }
}
