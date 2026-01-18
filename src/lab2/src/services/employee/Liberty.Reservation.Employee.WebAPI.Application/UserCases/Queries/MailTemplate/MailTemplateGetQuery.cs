using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;

public record MailTemplateGetQuery(
    string IoType
) : IQuerySingleBase<MailTemplateResponse>;
