using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityFile : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long FileId { get; set; }
    public File? File { get; set; }

    public FilePurposeTypes FilePurposeType { get; set; }

    /// <summary>
    /// ファイル設定順番
    /// </summary>
    public int Index { get; set; }

    public FacilityFile()
    {
    }

    public FacilityFile(
        Facility facility,
        File file
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        FileId = file.Id;
        File = file;
    }
}
