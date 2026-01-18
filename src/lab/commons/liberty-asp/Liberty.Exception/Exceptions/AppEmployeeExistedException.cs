namespace Liberty.Exception.Exceptions;

[Serializable]
public class AppEmployeeExistedException(
    string email
) : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => $"Employee duplicated at {Email} ";

    public string? Email { get; } = email;
}
