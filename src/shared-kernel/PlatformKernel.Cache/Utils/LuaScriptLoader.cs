namespace PlatformKernel.Cache.Utils;

public static class LuaScriptLoader
{
    public static string Load(
        string scriptName
    )
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "LuaScripts",
            $"{scriptName}.lua"
        );
        return File.ReadAllText(path);
    }
}
