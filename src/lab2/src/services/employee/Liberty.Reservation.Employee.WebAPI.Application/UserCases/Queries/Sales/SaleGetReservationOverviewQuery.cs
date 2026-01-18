using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Sales;

public record SaleGetReservationOverviewQuery(
    SaleGetAllRequest Request
) : IQuerySingleBase<SaleOverviewResponse>;
