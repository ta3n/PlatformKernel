using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface IMailTemplateService
{
    Task<TemplateFormatData?> FindMailTemplateAsync(
        CancellationToken cancellationToken = default
    );
}
