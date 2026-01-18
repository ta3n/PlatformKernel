using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;
using File = Liberty.Reservation.Employee.Application.Contexts.Entities.File;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FileService(
    ILogger<FileService> logger,
    IFileRepository fileRepository
) : IFileService
{
    public async Task<File> CreateFile(
        File fileToCreate
    )
    {
        var file = new File
        {
            Code = Guid.NewGuid().ToString(),
            Secret = fileToCreate.Secret
        };

        await fileRepository.AddAsync(file);

        logger.LogDebug("Created information for file: {Code}", file.Code);

        return file;
    }

    public Task DeleteFile(
        string secret
    )
    {
        var exitingFile = fileRepository.GetBySecret(secret);

        throw new NotImplementedException();
    }
}
