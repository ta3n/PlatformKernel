using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IMailTemplateService
{
    Task<SystemConfig> UpdateAsync(
        SystemConfig mailTemplateToUpdate,
        CancellationToken cancellationToken = default
    );

    Task<TemplateFormatData> FindOneAsync(
        CancellationToken cancellationToken = default
    );
}
