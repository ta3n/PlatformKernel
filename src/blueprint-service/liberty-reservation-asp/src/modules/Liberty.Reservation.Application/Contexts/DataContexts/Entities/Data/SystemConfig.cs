using Liberty.Entity;
using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class SystemConfig : EntityData
{
    public TemplateFormatData? TemplateFormatData { get; set; }
    public bool? CanOnlinePayment { get; set; }
}
