using System.ComponentModel.DataAnnotations;

namespace Liberty.Reservation.Employee.WebAPI.Application.Controllers.Responses;

public abstract class Pagination<T>
{
    public int Count { get; set; }

    public int Take { get; set; }

    public int Skip { get; set; }

    public bool Complete => Count <= Skip + Take;

    public abstract List<T> Datas { get; set; }
}

public class ResponseEmployeeManyGet : Pagination<ResponseEmployeeManyGetData>
{
    public override List<ResponseEmployeeManyGetData> Datas { get; set; }
}

public class ResponseEmployeeManyGetData
{
    /// <summary>従業員Code</summary>
    public string Id { get; set; }

    /// <summary>従業員名</summary>
    public string? Name { get; set; }

    /// <summary>従業員仮名</summary>
    public string? Kana { get; set; }

    /// <summary>Email</summary>
    public string EMail { get; set; }
}

public class ResponseEmployeeGet
{
    /// <summary>従業員名</summary>
    public string? Name { get; set; }

    /// <summary>従業員仮名</summary>
    public string? Kana { get; set; }

    /// <summary>Email</summary>
    public string EMail { get; set; }
}

public class ResponseEmployeePost
{
    /// <summary>従業員Code</summary>
    public string Id { get; set; }

    /// <summary>発行されたパスワード</summary>
    /// FIXME:結果画面に発行されたパスワードの表示が必要
    public string Password { get; set; }
}

public class ResponseSelfFileManyGet : Pagination<ResponseSelfFileManyGetData>
{
    public override List<ResponseSelfFileManyGetData> Datas { get; set; }
}

public class ResponseSelfFileManyGetData
{
    /// <summary>ファイル</summary>
    public ResponseSelfFileManyGetDataFile File { get; set; }
}

public class ResponseSelfFileManyGetDataFile
{
    /// <summary>ファイルCode</summary>
    [Required]
    public string FileId { get; set; }

    /// <summary>ファイルシークレット</summary>
    [Required]
    public string FileSecret { get; set; }
}

public class ResponseTestGet
{
    public required string AppName { get; set; }

    public required string AppVersion { get; set; }

    public required DateTime Date { get; set; }
}

/// <summary>
/// throw するのでResponseは実質不要
/// </summary>
public class ResponseTestErrorGet
{
}

public class ResponseTestPost
{
    public string Value1 { get; set; }

    public string Value2 { get; set; }

    public string Value3 { get; set; }

    public List<ResponseTestCreateData> Datas { get; set; }
}

public class ResponseTestCreateData
{
    public string Value1 { get; set; }

    public string Value2 { get; set; }

    public string Value3 { get; set; }
}
