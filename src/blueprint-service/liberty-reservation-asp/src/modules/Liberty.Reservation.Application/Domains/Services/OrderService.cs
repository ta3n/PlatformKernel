using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Order = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;

namespace Liberty.Reservation.Application.Domains.Services;

public class OrderService(
    ILogger<OrderService> logger,
    IBookingOrderRepository bookingOrderRepository
) : BaseService<Order>(logger, bookingOrderRepository, new OrderNotfoundException()), IOrderService
{
    public override Task<Order> UpdateAsync(
        Order entityToUpdate,
        bool autoSave = true,
        Func<Order, Order, Order>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.AccessID = updateEntity.AccessID;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }
};
