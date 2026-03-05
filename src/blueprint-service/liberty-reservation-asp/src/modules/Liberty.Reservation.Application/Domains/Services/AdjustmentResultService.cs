using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class AdjustmentResultService(
    ILogger<AdjustmentResultService> logger,
    IAdjustmentResultRepository adjustmentResultRepository
) : BaseService<AdjustmentResult>(logger, adjustmentResultRepository, new AdjustmentResultNotfoundException()), IAdjustmentResultService;
