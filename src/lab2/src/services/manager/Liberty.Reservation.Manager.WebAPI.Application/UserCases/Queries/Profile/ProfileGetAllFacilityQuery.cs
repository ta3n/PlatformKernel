using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Profile;

public record ProfileGetAllFacilityQuery : IQueryListBase<FacilityManagementResponse>;
