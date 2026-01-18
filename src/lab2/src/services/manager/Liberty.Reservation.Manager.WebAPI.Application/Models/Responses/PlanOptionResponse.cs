namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanOptionResponse
{
    public long Id { get; init; }
    public bool UseFixedOptionItem { get; init; }
    public IEnumerable<OptionItemOfPlanOptionResponse>? OptionItems { get; init; }
}

public record OptionItemOfPlanOptionResponse(
    long Id,
    string? Name,
    bool IsEnabled
);
