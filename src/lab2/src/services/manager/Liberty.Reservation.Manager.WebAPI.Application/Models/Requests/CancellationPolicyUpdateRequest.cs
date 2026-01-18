namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record CancellationPolicyUpdateRequest(
    string Name,
    string? Description,
    string? RuleDetail,
    IEnumerable<DataOfCancellationUpdateRequest> Data
)
{
    public long? Id { get; set; }
}

public record DataOfCancellationUpdateRequest(
    long? Id,
    int DayStart,
    int DayEnd,
    float Rate,
    string? Description,
    long DisplayOrder
);
