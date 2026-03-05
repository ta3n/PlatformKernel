using Liberty.Entity.Auditing;

namespace Liberty.Entity;

public interface IBaseEntity
    : IEnable,
        IVisible,
        ILogicalDelete,
        ISorter,
        IRecordMemo,
        IHasCreationTime<long>,
        IHasCreator<string?>,
        IIHasModificationTime<long?>,
        IHasEditor<string?>,
        IHasDeletionTime<long?>,
        IHasEraser<string?>;
