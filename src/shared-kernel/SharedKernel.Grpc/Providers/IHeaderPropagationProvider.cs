using Grpc.Core;

namespace SharedKernel.Grpc.Providers;

public interface IHeaderPropagationProvider
{
    Metadata GetGrpcMetadata();
    IDictionary<string, string> GetHttpHeaders();
}
