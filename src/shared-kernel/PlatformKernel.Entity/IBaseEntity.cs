using PlatformKernel.Entity.Auditing;

namespace PlatformKernel.Entity;

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
