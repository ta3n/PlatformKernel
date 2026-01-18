using AutoMapper;
using Liberty.Fax.Models;
using Liberty.Fax.Services.Interfaces;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingAggregateFaxAuditLog;
using MockQueryable.Moq;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class SearchBookingAggregateFaxAuditLogQueryHandlerTest
{
    [Fact]
    public async Task HandlerAsync_GetEmptyFaxAuditLogSearchResult()
    {
        var mapperMock = new Mock<IMapper>();
        var faxAuditLogRepositoryMock = new Mock<IBookingAggregateFaxAuditLogRepository>();
        var faxErrorServiceMock = new Mock<IFaxErrorService>();
        var pageableMock = new Mock<IPageable>();

        var faxAuditLogsMock = new List<BookingAggregateFaxAuditLog>();

        faxAuditLogRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(faxAuditLogsMock.AsQueryable().BuildMockDbSet().Object);

        faxErrorServiceMock
            .Setup(x => x.GetAll())
            .Returns(new List<FaxErrorItem>());

        var cancellationToken = CancellationToken.None;

        var query = new SearchBookingAggregateFaxAuditLogQuery(pageableMock.Object);

        var queryHander = new SearchBookingAggregateFaxAuditLogQueryHandler(
            mapperMock.Object,
            faxAuditLogRepositoryMock.Object,
            faxErrorServiceMock.Object
        );

        var (_, result) = await queryHander.Handle(query, cancellationToken);

        Assert.Empty(result);
    }
}
