using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.User.Application.Domains.Services.Interfaces;

public interface IMailTemplateService
{
    Task<TemplateFormatData?> FindMailTemplateAsync(
        CancellationToken cancellationToken = default
    );
}
