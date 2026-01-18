using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityFaxService : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long FaxServiceId { get; set; }
    public FaxService? FaxService { get; set; }

    public long? EnabledStart { get; set; }

    /// <summary>
    /// 運用終了日時
    /// </summary>
    public long? EnabledEnd { get; set; }

    /// <summary>
    /// 最低請求発生金額
    /// </summary>
    public int ClaimActionPrice { get; set; }

    /// <summary>
    /// Fax通知単価
    /// </summary>
    public float Price { get; set; }

    public FacilityFaxService()
    {
    }

    public FacilityFaxService(
        Facility facility,
        FaxService faxService
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        FaxServiceId = faxService.Id;
        FaxService = faxService;
    }
}
