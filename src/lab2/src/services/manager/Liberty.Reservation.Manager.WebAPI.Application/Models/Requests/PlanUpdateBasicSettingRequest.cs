namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateBasicSettingRequest(
    string? Name,
    string? NameForImport,
    string? Summary,
    string? Description,
    List<FileOfPlanUpdateBasicSettingRequest>? Files
);

public record FileOfPlanUpdateBasicSettingRequest(
    long Id,
    int Index
);
