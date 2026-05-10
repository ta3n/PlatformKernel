Manage Docker infrastructure services for local development. Accepts subcommands.

Usage:
- `/docker up` — start all required services
- `/docker down` — stop all services
- `/docker status` — show running containers and health
- `/docker logs [service]` — tail logs for a specific service
- `/docker reset` — stop, remove volumes, and restart (clean state)

## Commands

### Start services
```bash
docker compose up -d
```

### Stop services
```bash
docker compose down
```

### Check status
```bash
docker compose ps
```

### Tail logs
```bash
# All services
docker compose logs -f

# Specific service (postgres | redis | rabbitmq | seq | minio)
docker compose logs -f {service}
```

### Full reset (destroys all data)
```bash
docker compose down -v && docker compose up -d
```

## Service URLs

| Service | URL | Credentials |
|---|---|---|
| pgAdmin | http://localhost:5050 | `admin@localhost.com` / `admin.localhost` |
| Redis Commander | http://localhost:8081 | — |
| Seq (logs) | http://localhost:5341 | — |
| MinIO Console | http://localhost:9011 | `minioadmin` / `minioadmin123` |
| RabbitMQ Management | http://localhost:15672 | see `.env` file |

## Rules

- Required services for integration tests: **PostgreSQL, Redis, RabbitMQ, Seq, MinIO**
- Environment variables are loaded from `.env` in the project root (not committed)
- Use `docker compose` (v2) not `docker-compose` (v1)
- `/docker reset` destroys all data — confirm with user before running
