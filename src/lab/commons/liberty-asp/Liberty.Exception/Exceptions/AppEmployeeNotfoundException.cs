namespace Liberty.Exception.Exceptions;

[Serializable]
public class AppEmployeeNotfoundException(
    string? code
) : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => $"Employee not found at {Code}";

    public string? Code { get; } = code;
}
