namespace Liberty.ApplicationShared.Utils;

public static class EntityUtil
{
    public static string CreateCode()
    {
        return Guid.NewGuid().ToString();
    }
}
