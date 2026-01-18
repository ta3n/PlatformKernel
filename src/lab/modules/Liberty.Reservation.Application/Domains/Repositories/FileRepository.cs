using Liberty.Reservation.Application.Contexts.DataContexts;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Domains.Repositories;

public interface IFileRepository
{
    Task<File> Create(
        string fileCode,
        string secret
    );

    Task SaveAsync();
}

public class FileRepository(
    ReservationDataContext reservationDataContext
) : IFileRepository
{
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

        await reservationDataContext.Files.AddAsync(file);

        return file;
    }

    public async Task SaveAsync()
    {
        await reservationDataContext.SaveChangesAsync();
    }
}
