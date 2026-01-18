using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Entity;

//public abstract class BaseData<T> : IBaseData<T>, ISorter
//public abstract class BaseData<T> : ISorter
public class EntityData : BaseEntity, IEntityData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string? Code { get; set; }

    protected EntityData()
    {
        Code = Guid.NewGuid().ToString("N");
        IsEnabled = false;
    }

    public virtual void Delete()
    {
        IsDeleted = true;
    }
}
