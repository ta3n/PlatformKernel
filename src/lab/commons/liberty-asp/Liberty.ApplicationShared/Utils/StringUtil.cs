namespace Liberty.ApplicationShared.Utils;

public class StringUtil
{
    public static string ToFirstUpper(
        string str
    )
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException();
        }

        var array = str.ToCharArray();
        var up = char.ToUpper(array[0]);
        array[0] = up;
        return new string(array);
    }
}
