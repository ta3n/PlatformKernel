namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFileService
{
    Task<Contexts.Entities.File> CreateFile(
        Contexts.Entities.File fileToCreate
    );

    Task DeleteFile(
        string secret
    );
}
