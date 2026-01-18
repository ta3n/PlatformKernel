using Liberty.Entity;
using Liberty.Reservation.Employee.Application.Constants;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;

public class FileEmployee : EntityRelation
{
    [Comment("ファイル")]
    public File? File { get; set; }

    [Comment("ファイルID")]
    public long FileId { get; set; }

    [Comment("従業員")]
    public Employee? Employee { get; set; }

    [Comment("従業員ID")]
    public string? EmployeeId { get; set; }

    [Comment("関係種類")]
    public EmployeeFileTypes Type { get; set; }

    public FileEmployee()
    {
    }

    public FileEmployee(
        File manager,
        Employee employee
    )
    {
        File = manager;
        FileId = manager.Id;
        Employee = employee;
        EmployeeId = employee.Id;
    }
}
