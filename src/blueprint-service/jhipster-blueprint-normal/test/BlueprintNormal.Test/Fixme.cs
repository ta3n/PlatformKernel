using BlueprintNormal.Infrastructure.Data;
using BlueprintNormal.Domain.Entities;
using BlueprintNormal.Test.Setup;

namespace BlueprintNormal.Test;

public static class Fixme
{
    public static User ReloadUser<TEntryPoint>(AppWebApplicationFactory<TEntryPoint> factory, User user)
        where TEntryPoint : class, IStartup, new()
    {
        var applicationDatabaseContext = factory.GetRequiredService<ApplicationDatabaseContext>();
        applicationDatabaseContext.Entry(user).Reload();
        return user;
    }
}
