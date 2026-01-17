namespace SharedKernel.Entity;

public interface IEntityData : IBaseEntity
{
    long Id { get; set; }
    string? Code { get; set; }
    void Delete();
}
