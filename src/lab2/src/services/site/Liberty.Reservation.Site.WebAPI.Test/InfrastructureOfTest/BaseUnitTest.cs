using System.Data;
using System.Linq.Expressions;
using AutoMapper;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

public abstract class BaseUnitTest
{
    protected IUnitOfWork MockUnitOfWork { get; set; }
    protected IMediator MockMediator { get; set; }
    protected IMapper MockMapper { get; set; }
    protected DefaultHttpContext HttpContext { get; private set; }
    protected ActionContext ActionContext { get; private set; }
    protected Mock<IHttpContextAccessor> HttpContextAccessor { get; private set; }
    protected Mock<IActionResultExecutor<ObjectResult>> MockActionResultExecutor { get; private set; }
    protected Mock<ILoggerFactory> MockLoggerFactory { get; private set; }

    protected BaseUnitTest()
    {
        HttpContext = new DefaultHttpContext();
        HttpContextAccessor = MockServices.MockHttpContextAccessor();
        MockActionResultExecutor = MockServices.MockActionResultExecutor();
        MockLoggerFactory = MockServices.MockLoggerFactory();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(HttpContextAccessor.Object);
        serviceCollection.AddSingleton(MockActionResultExecutor.Object);
        serviceCollection.AddSingleton(MockLoggerFactory.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        HttpContext.RequestServices = serviceProvider;
        ActionContext = new ActionContext { HttpContext = HttpContext };
        MockMediator = new Mock<IMediator>().Object;
        MockMapper = new Mock<IMapper>().Object;
        MockUnitOfWork = new Mock<IUnitOfWork>().Object;

        InitData();
        SetupUnitOfWork();
    }

    protected virtual void InitData()
    {
    }

    private void SetupUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(uow => uow.Set<It.IsAnyType>())
            .Returns((DbSet<It.IsAnyType>?)null!);

        // Mock BeginTransactionAsync
        mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Mock CommitAsync
        mockUnitOfWork.Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Mock RollbackAsync
        mockUnitOfWork.Setup(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Mock SaveChanges
        mockUnitOfWork.Setup(uow => uow.SaveChanges())
            .Returns(1); // Customize return value as needed.

        // Mock SaveChangesAsync
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Customize return value as needed.

        // Mock UpdateState
        mockUnitOfWork.Setup(uow => uow.UpdateState(It.IsAny<object>(), It.IsAny<EntityState>()));

        // Mock SetEntityStateModified
        mockUnitOfWork.Setup(
            uow => uow.SetEntityStateModified(
                It.IsAny<object>(),
                It.IsAny<Expression<Func<object, object>>?>()
            )
        );

        // Mock GetDbContext
        mockUnitOfWork.Setup(uow => uow.GetDbContext())
            .Returns(new Mock<DbContext>().Object);

        MockUnitOfWork = mockUnitOfWork.Object;
    }
}
