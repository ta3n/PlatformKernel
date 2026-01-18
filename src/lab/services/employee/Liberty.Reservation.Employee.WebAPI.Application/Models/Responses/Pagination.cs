namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public abstract record Pagination<T>
{
    public int Count { get; init; }

    public int Take { get; init; }

    public int Skip { get; init; }

    public bool Complete => Count <= Skip + Take;

    public abstract IEnumerable<T>? Data { get; init; }
}
