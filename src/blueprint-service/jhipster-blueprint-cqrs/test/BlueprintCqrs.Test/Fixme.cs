using BlueprintCqrs.Infrastructure.Data;
using BlueprintCqrs.Domain.Entities;
using BlueprintCqrs.Test.Setup;

namespace BlueprintCqrs.Test;

public static class Fixme
{
    public static User ReloadUser<TEntryPoint>(
        AppWebApplicationFactory<TEntryPoint> factory,
        User user
    )
        where TEntryPoint : class, IStartup, new()
    {
        var applicationDatabaseContext = factory.GetRequiredService<ApplicationDatabaseContext>();
        applicationDatabaseContext.Entry(user).Reload();
        return user;
    }
}
