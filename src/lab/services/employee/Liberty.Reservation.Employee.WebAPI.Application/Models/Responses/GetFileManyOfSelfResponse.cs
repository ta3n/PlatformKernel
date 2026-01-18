namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record GetFileManyOfSelfResponse : Pagination<DataGetFileManyOfSelfResponse>
{
    public override IEnumerable<DataGetFileManyOfSelfResponse>? Data { get; init; }
}
