using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class PointRateService(
    ILogger<PointRateService> logger,
    IPointRateRepository pointRateRepository
) : BaseService<PointRate>(logger, pointRateRepository, new PointRateNotfoundException()), IPointRateService
{
    public override Task<PointRate> CreateAsync(
        PointRate entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(
            entityToCreate,
            autoSave,
            cancellationToken
        );
    }
}
