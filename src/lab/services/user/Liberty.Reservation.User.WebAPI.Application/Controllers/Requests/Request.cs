using System.ComponentModel.DataAnnotations;

namespace Liberty.Reservation.Employee.WebAPI.Application.Controllers.Requests;

public class RequestEmployeePost
{
    // FIXME: 姓名をわけているが、多国籍の場合複雑であり、検索に影響はないためわけなくてよいのでは？
    [Required]
    public string Name { get; set; }

    [Required]
    public string EMail { get; set; }
}

public class RequestEmployeePut
{
    /// <summary>従業員Code</summary>
    [Required]
    public string Id { get; set; }

    /// <summary>従業員名</summary>
    [Required]
    public string Name { get; set; }

    /// <summary>従業員名仮名</summary>
    [Required]
    public string Kana { get; set; }
}

public class RequestEmployeeEmailPut
{
    /// <summary>従業員Code</summary>
    [Required]
    public string Id { get; set; }

    /// <summary>従業員Code</summary>
    [Required]
    [EmailAddress]
    public string EMail { get; set; }
}

public class RequestEmployeePasswordPut
{
    /// <summary>従業員Code</summary>
    [Required]
    public string Id { get; set; }

    /// <summary>password</summary>
    [Required]
    public string Password { get; set; }
}

public class RequestEmployeeDelete
{
    /// <summary>従業員Code</summary>
    [Required]
    public string Id { get; set; }
}

public class RequestSelfPut
{
    /// <summary>従業員名</summary>
    public string? Name { get; set; }

    /// <summary>従業員名仮名</summary>
    public string? Kana { get; set; }
}

public class RequestSelfEmailPut
{
    /// <summary>従業員Code</summary>
    [Required]
    [EmailAddress]
    public string EMail { get; set; }
}

public class RequestSelfPasswordPut
{
    /// <summary>従業員Code</summary>
    [Required]
    public string Id { get; set; }

    /// <summary>password</summary>
    [Required]
    public string Password { get; set; }
}

public class RequestSelfFilePost
{
    /// <summary>ファイルCode</summary>
    [Required]
    public string FileId { get; set; }

    /// <summary>ファイルシークレット</summary>
    [Required]
    public string FileSecret { get; set; }
}

public class RequestSelfFileDelete
{
    /// <summary>ファイルCode</summary>
    [Required]
    public string FileId { get; set; }

    /// <summary>ファイルシークレット</summary>
    [Required]
    public string FileSecret { get; set; }
}

public class RequestSelfAvatarPost
{
    /// <summary>ファイルCode</summary>
    [Required]
    public string FileId { get; set; }

    /// <summary>ファイルシークレット</summary>
    [Required]
    public string FileSecret { get; set; }
}

public class RequestTestPost
{
    [Required]
    public string Value1 { get; set; }
}

public class RequestTestPut
{
    [Required]
    public string Value1 { get; set; }
}

public class RequestTestDelete
{
    [Required]
    public string Value1 { get; set; }
}
