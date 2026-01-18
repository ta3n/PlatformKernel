using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.Application.Domains.Services;

public class OrderGmoPaymentService(
    ILogger<OrderGmoPaymentService> logger,
    IOrderGmoPaymentRepository gmoPaymentRepository
) : BaseServiceRelation<OrderGmoPaymentResultRequest>(logger, gmoPaymentRepository), IOrderGmoPaymentService;
