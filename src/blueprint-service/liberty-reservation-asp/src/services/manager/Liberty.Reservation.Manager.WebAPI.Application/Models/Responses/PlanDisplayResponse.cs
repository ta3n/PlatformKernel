namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanDisplayResponse
{
    public long? Id { get; init; }
    public string[]? Tags { get; init; }
    public IEnumerable<CategoryOfPlanDisplayResponse>? MasterCategories { get; init; }
    public IEnumerable<CategoryOfPlanDisplayResponse>? PlanCategories { get; init; }
}

public record CategoryOfPlanDisplayResponse(
    long Id,
    string? Name,
    bool IsEnabled
);
