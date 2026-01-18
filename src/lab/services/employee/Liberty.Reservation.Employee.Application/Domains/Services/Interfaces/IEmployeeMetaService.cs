using Liberty.Reservation.Employee.Application.Contexts.Entities;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IEmployeeMetaService
{
    Task<EmployeeMeta> CreateEmployeeMeta(
        EmployeeMeta employeeMetaToCreate
    );

    Task<EmployeeMeta> UpdateEmployeeMeta(
        EmployeeMeta employeeMetaToUpdate
    );
}
