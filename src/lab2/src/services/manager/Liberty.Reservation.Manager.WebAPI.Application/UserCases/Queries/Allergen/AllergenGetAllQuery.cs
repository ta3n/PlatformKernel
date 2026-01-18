using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Allergen;

public record AllergenGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<AllergenResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
