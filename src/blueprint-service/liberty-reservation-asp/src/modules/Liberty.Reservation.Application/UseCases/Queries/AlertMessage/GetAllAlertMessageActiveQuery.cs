using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.UseCases.Queries.AlertMessage;

public record GetAllAlertMessageActiveQuery(
    string Language
) : IQueryListBase<AlertMessageResponse>;
