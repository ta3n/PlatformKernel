namespace SharedKernel.Cache.Test.Integration;

public sealed record TestCachePayload(
    string Name,
    int Version
);

public sealed record TestCacheListItem(
    string Code
);
