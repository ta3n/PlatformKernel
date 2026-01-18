using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = Liberty.Reservation.Employee.Application.Contexts.Entities.File;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class FileRepository(
    ILogger<FileRepository> logger,
    EmployeeDataContext dbContext
) : RepositoryBase<File>(logger,dbContext), IFileRepository
{
    public async Task<File?> GetBySecret(
        string secret
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Where(x => x.Secret == secret);

        return await queryable.SingleOrDefaultAsync();
    }

    public async Task<File> Create(
        string fileCode,
        string secret
    )
    {
        var file = new File
        {
            Code = fileCode,
            Secret = secret
        };

        await dbContext.Files.AddAsync(file);

        return file;
    }
}
