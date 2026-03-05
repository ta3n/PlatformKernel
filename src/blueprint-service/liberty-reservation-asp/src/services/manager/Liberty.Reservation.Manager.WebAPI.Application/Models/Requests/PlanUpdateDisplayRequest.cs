namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateDisplayRequest(
    string[]? Tags,
    List<long>? PlanCategoryIds,
    List<long>? MasterCategoryIds
);
