<<<<<<<< HEAD:src/shared-kernel/SharedKernel.CQRS.Test/MediatRTests.cs
using global::MediatR;
using Microsoft.AspNetCore.Http;
using SharedKernel.CQRS.MediatR;
using SharedKernel.CQRS.MediatR.BaseCommand.Implementations;
using SharedKernel.CQRS.MediatR.BaseQuery.Implementations;
========
using Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.CQRS.Mediator;
using SharedKernel.CQRS.Mediator.BaseCommand.Implementations;
using SharedKernel.CQRS.Mediator.BaseQuery.Implementations;
>>>>>>>> origin/develop:src/shared-kernel/SharedKernel.CQRS.Test/UnitTest1.cs
using SharedKernel.Pagination;

namespace SharedKernel.CQRS.Test;

<<<<<<<< HEAD:src/shared-kernel/SharedKernel.CQRS.Test/MediatRTests.cs
public class MediatRCqrsTests
========
public class MediatorCqrsTests
>>>>>>>> origin/develop:src/shared-kernel/SharedKernel.CQRS.Test/UnitTest1.cs
{
    [Fact]
    public async Task MediatR_CommandBaseHandler_InvokesHandleAsyncAndRemoveCaches()
    {
        var handler = new TestCommandHandler();

        var result = await handler.Handle(new SampleCommand(), CancellationToken.None);

        Assert.Equal("handled", result);
        Assert.True(handler.HandleAsyncCalled);
        Assert.True(handler.RemoveCachesCalled);
    }

    [Fact]
    public async Task MediatR_CommandBaseWithAuditEventHandler_PublishesAuditEventWhenPresent()
    {
        var mediator = new RecordingMediator();
        var handler = new AuditCommandHandler(mediator);

        var result = await handler.Handle(new SampleCommand(), CancellationToken.None);

        Assert.Equal("audit", result);
        Assert.Single(mediator.PublishedNotifications);
        Assert.IsType<SampleAuditEvent>(mediator.PublishedNotifications.Single());
    }

    [Fact]
    public async Task MediatR_QueryBaseHandler_HandlesRequestWithoutCache()
    {
        var handler = new TestQueryHandler();

        var (headers, response) = await handler.Handle(new SampleQuery(), CancellationToken.None);

        Assert.Equal("processed", response);
        Assert.Equal("true", headers["X-Test"]);
        Assert.Equal(1, handler.HandleCount);
    }

    [Fact]
    public void MediatR_QueryCacheLockManager_ReturnsSameSemaphoreForSameKey()
    {
        var first = QueryCacheLockManager.GetCacheLockForKey("shared-key");
        var second = QueryCacheLockManager.GetCacheLockForKey("shared-key");

        Assert.Same(first, second);
    }

    [Fact]
    public void MediatR_QueryPagedBase_PreservesProvidedPageable()
    {
        var pageable = Pageable.Of(1, 20);
        var query = new SamplePagedQuery(pageable);

        Assert.Same(pageable, query.Pageable);
    }

    private sealed record SampleCommand : CommandBase<string>;

    private sealed class TestCommandHandler : CommandBaseHandler<SampleCommand, string>
    {
        public bool HandleAsyncCalled { get; private set; }

        public bool RemoveCachesCalled { get; private set; }

        protected override Task<string> HandleAsync(SampleCommand request, CancellationToken cancellationToken)
        {
            HandleAsyncCalled = true;
            return Task.FromResult("handled");
        }

        protected override void RemoveCaches(SampleCommand request)
        {
            RemoveCachesCalled = true;
        }
    }

    private sealed class AuditCommandHandler(IMediator mediator) : CommandBaseWithAuditEventHandler<SampleCommand, string>(mediator)
    {
        protected override Task<string> HandleAsync(SampleCommand request, CancellationToken cancellationToken)
        {
            AuditEventData = new SampleAuditEvent();
            return Task.FromResult("audit");
        }
    }

    private sealed class SampleAuditEvent : IAuditEventData;

    private sealed record SampleQuery : QuerySingleBase<string>;

    private sealed class TestQueryHandler : QuerySingleBaseHandler<SampleQuery, string>
    {
        public int HandleCount { get; private set; }

        protected override Task<(IHeaderDictionary, string)> HandleAsync(SampleQuery request, CancellationToken cancellationToken)
        {
            HandleCount++;
            IHeaderDictionary headers = new HeaderDictionary
            {
                ["X-Test"] = "true"
            };
            return Task.FromResult((headers, "processed"));
        }
    }

    private sealed record SamplePagedQuery(IPageable Pageable) : QueryPagedBase<string>(Pageable);

    private sealed class RecordingMediator : IMediator
    {
        public List<INotification> PublishedNotifications { get; } = [];

        public ValueTask Publish(object notification, CancellationToken cancellationToken = default)
        {
            PublishedNotifications.Add((INotification)notification);
            return ValueTask.CompletedTask;
        }

        public ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            PublishedNotifications.Add(notification);
            return ValueTask.CompletedTask;
        }

        public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public ValueTask<object?> Send(object message, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamCommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object message, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
