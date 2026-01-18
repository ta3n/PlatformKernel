namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record GetEmployeeManyResponse : Pagination<DataGetEmployeeManyResponse>
{
    public override IEnumerable<DataGetEmployeeManyResponse>? Data { get; init; }
}
