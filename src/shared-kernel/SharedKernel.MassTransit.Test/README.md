# SharedKernel.MassTransit.Test

Sample end-to-end test stack for validating the `SharedKernel.MassTransit` base with:

- RabbitMQ transport
- PostgreSQL transactional outbox
- PostgreSQL saga state persistence
- Multiple service roles from a single project image

## Roles

- `Api`: exposes HTTP endpoints, writes orders to PostgreSQL, and publishes `OrderSubmitted` through the bus outbox.
- `Saga`: orchestrates the order flow with a MassTransit state machine.
- `Processor`: consumes commands from the saga and publishes completion events.

## What this validates

### Outbox pattern

`POST /orders` writes the order and publishes the initial event through the EF bus outbox.  
`POST /orders/fail-after-publish` simulates a failure after `Publish` and before `SaveChanges`, so the message should not be delivered.

### Saga pattern

Order flow:

1. `OrderSubmitted`
2. saga publishes `ReserveInventory`
3. processor publishes `InventoryReserved`
4. saga publishes `ProcessPayment`
5. processor publishes `PaymentProcessed`
6. saga publishes `OrderCompleted`
7. API consumer updates order status to `Completed`

## Run with Docker Compose

From repository root:

```bash
docker compose -f src/shared-kernel/SharedKernel.MassTransit.Test/docker-compose.yml up --build
```

## Test commands

Create an order:

```bash
curl -X POST http://localhost:8088/orders \
  -H "Content-Type: application/json" \
  -d '{"customerId":"customer-01","amount":125.50}'
```

Get all orders:

```bash
curl http://localhost:8088/orders
```

Get one order with saga snapshot and processing logs:

```bash
curl http://localhost:8088/orders/<ORDER_ID>
```

Verify outbox behavior on failure:

```bash
curl -X POST http://localhost:8088/orders/fail-after-publish \
  -H "Content-Type: application/json" \
  -d '{"customerId":"customer-fail","amount":50.00}'
```

If the outbox is working correctly, the failed request should not create an order row and should not start the saga flow.
