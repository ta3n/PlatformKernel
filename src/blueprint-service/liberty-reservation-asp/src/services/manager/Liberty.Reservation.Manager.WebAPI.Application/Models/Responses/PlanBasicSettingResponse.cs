namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanBasicSettingResponse
{
    public long Id { get; init; }
    public string? Name { get; init; }
    public string? NameForImport { get; init; }
    public string? Summary { get; init; }
    public PlanTypes? PlanType { get; init; }
    public string? Description { get; init; }
    public List<FileOfPlanBasicSettingResponse>? Files { get; init; }
}

public record FileOfPlanBasicSettingResponse(
    long Id,
    string? Code,
    int Index,
    bool IsEnabled,
    string? Description
);
