using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FileCategory : EntityRelation
{
    public long FileId { get; set; }
    public File? File { get; set; }

    public long CategoryId { get; set; }
    public Category? Category { get; set; }

    public FileCategory()
    {
    }

    public FileCategory(
        File file,
        Category category
    )
    {
        FileId = file.Id;
        File = file;
        CategoryId = category.Id;
        Category = category;
    }
}
