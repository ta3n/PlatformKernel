using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class EmployeeRepository(
    ILogger<EmployeeRepository> logger,
    EmployeeDataContext dbContext
) : RepositoryBase<Contexts.Entities.Employee>(logger, dbContext), IEmployeeRepository
{
    public async Task<Contexts.Entities.Employee> GetByCode(
        string code
    )
    {
        var res = await dbContext.Employees
            .Where(a => a.Code == code)
            .SingleOrDefaultAsync() ?? throw new AppEmployeeNotfoundException(code);

        return res;
    }

    public async Task Update(
        string code,
        string? name,
        string? kana
    )
    {
        var data = await dbContext.Employees
            .Include(x => x.EmployeeMeta)
            .SingleOrDefaultAsync(x => x.Code == code) ?? throw new AppEmployeeNotfoundException(code);

        if (data.EmployeeMeta is null)
        {
            throw new AppEmployeeMetaNotfoundException();
        }

        data.EmployeeMeta.Name = name!;
        data.EmployeeMeta.Kana = kana;
    }
}
