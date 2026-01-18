using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;

public interface IEmployeeRepository : IRepositoryBase<Contexts.Entities.Employee>
{
    Task<Contexts.Entities.Employee> GetByCode(
        string code
    );

    Task Update(
        string code,
        string? name,
        string? kana
    );
}
