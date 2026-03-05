using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class IntegrationEventOutbox : EntityData
{
    public required string ServiceName { get; set; }
    public required string EventName { get; set; }
    public required string JobName { get; set; }
    public required string JsonData { get; set; }
}
