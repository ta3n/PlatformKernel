Run tests for this repository. Accepts an optional argument: a module name or project path.

Usage:
- `/test` — runs all tests in the solution
- `/test Entity` — runs tests for SharedKernel.Entity.Test
- `/test src/shared-kernel/SharedKernel.Cache.Test` — runs a specific project path

## Steps

1. If an argument is provided, resolve it to the correct `.csproj` path under `src/shared-kernel/`. Module names map to `SharedKernel.{Name}.Test`.
2. Run the appropriate command:

```bash
# All tests
dotnet test --collect:"XPlat Code Coverage;Format=opencover"

# Single module (example)
dotnet test src/shared-kernel/SharedKernel.Entity.Test/SharedKernel.Entity.Test.csproj
```

3. Report: total passed/failed/skipped, any failed test names with their error messages.
4. If tests fail with connection errors (PostgreSQL, Redis, RabbitMQ), remind the user to run `docker compose up -d`.

Note: `*.Test.Service` projects require running infrastructure services.
