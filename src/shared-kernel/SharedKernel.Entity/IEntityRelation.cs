namespace SharedKernel.Entity;

public interface IEntityRelation : IBaseEntity
{
    new string? RecordMemo { get; set; }
}
