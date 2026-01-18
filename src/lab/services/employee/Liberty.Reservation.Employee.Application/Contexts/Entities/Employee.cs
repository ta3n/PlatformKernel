using Liberty.Entity;
using Liberty.Reservation.Employee.Application.Constants;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application.Contexts.Entities;

[Comment("リバティー社員")]
public class Employee : IdentityUser, IBaseEntity
{
    /// <summary>会員Code</summary>
    [Comment("会員Code")]
    public string? Code { get; set; }

    /// <summary>住所情報情報のリレーションID</summary>
    [Comment("AddressId")]
    public long? AddressId { get; set; }

    /// <summary>住所情報</summary>
    [Comment("住所情報")]
    public Address? Address { get; set; }

    /// <summary>状態</summary>
    [Comment("状態")]
    public EmployeeStatus State { get; set; }

    /// <summary>メタ情報のリレーションID</summary>
    public long? EmployeeMetaId { get; set; }

    /// <summary>メタ情報</summary>
    [Comment("メタ情報")]
    public EmployeeMeta? EmployeeMeta { get; set; }

    [Comment("社員とのリレーション")]
    public ICollection<FileEmployee>? FileEmployees { get; set; }

    public string? GetMailConfirmationToken(
        string code,
        string hash,
        DateTime date
    )
    {
        var data = EmployeeMeta?.Temporarily
            ?.EmailConfirmations
            .Where(a => a.Code == code)
            .Where(a => a.Hash == hash)
            .SingleOrDefault(a => a.Expired > date);

        return data?.Token;
    }

    public string? GetResetPasswordToken(
        string code,
        string hash,
        DateTime date
    )
    {
        var data = EmployeeMeta?.Temporarily
            ?.PasswordResets
            .Where(a => a.Code == code)
            .Where(a => a.Hash == hash)
            .SingleOrDefault(a => a.Expired > date);

        return data?.Token;
    }

    public bool IsEnabled { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDeleted { get; set; }
    public long DisplayOrder { get; set; }
    public string? RecordMemo { get; set; }
    public long CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public long? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public long? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
