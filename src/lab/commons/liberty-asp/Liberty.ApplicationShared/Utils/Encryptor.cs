namespace Liberty.ApplicationShared.Utils;

public class Encryptor
{
    public static Encryptor? Instance { get; set; }

    private Encryptor()
    {
    }

    public static Encryptor GetInstance()
    {
        return Instance ??= new Encryptor();
    }

    public string Create()
    {
        // FIXME not implement
        return "abc123";
    }
}
