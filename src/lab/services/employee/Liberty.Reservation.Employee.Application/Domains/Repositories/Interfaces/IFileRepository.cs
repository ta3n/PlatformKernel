using Liberty.UnitOfWork.Abstractions;
using File = Liberty.Reservation.Employee.Application.Contexts.Entities.File;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;

public interface IFileRepository : IRepositoryBase<File>
{
    Task<File?> GetBySecret(
        string secret
    );

    Task<File> Create(
        string fileCode,
        string secret
    );
}
