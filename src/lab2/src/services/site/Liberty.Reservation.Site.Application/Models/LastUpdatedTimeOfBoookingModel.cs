namespace Liberty.Reservation.Site.Application.Models;

public class LastUpdatedTimeOfBoookingModel
{
    public long? FacilityUpdatedAt { get; set; }
    public long? SiteUpdatedAt { get; set; }
    public long? PlanUpdatedAt { get; set; }
    public long? CancellationUpdatedAt { get; set; }
    public long? RoomGroupUpdatedAt { get; set; }
    public List<long?>? QuestionsUpdatedAt { get; set; }
    public List<long?>? FilesUpdatedAt { get; set; }
    public List<long?>? PersonAgeTypeUpdatedAt { get; set; }
}
