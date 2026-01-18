using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FileOptionItem : EntityRelation
{
    public long FileId { get; set; }
    public File? File { get; set; }

    /// <summary>
    /// オプションアイテムID
    /// </summary>
    public long OptionItemId { get; set; }

    /// <summary>
    /// オプションアイテム
    /// </summary>
    public OptionItem? OptionItem { get; set; }

    public int Index { get; set; }

    public FileOptionItem()
    {
    }

    public FileOptionItem(
        File file,
        OptionItem optionItem
    )
    {
        FileId = file.Id;
        File = file;
        OptionItemId = optionItem.Id;
        OptionItem = optionItem;
    }
}
