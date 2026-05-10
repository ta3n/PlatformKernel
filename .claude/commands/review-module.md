Review a SharedKernel module for architecture, conventions, and quality. Accepts one argument: the module name or path.

Usage: `/review-module Cache` or `/review-module src/shared-kernel/SharedKernel.Cache`

## Review checklist

**Architecture**
- [ ] Module has `Abstractions/`, `Extensions/`, `Services/` folders
- [ ] DI extension uses `Add{Name}()` on `IServiceCollection`
- [ ] Uses `TryAdd*` for registrations (no duplicate registration risk)
- [ ] No cross-module dependencies (except `SharedKernel.Entity`/`SharedKernel.Exception`)
- [ ] No business logic — infrastructure only

**C# Conventions**
- [ ] Types are `internal sealed` unless a broader scope is justified
- [ ] Explicit types used; `var` only when assignment makes type evident
- [ ] `is null` / `is not null` — not `== null`
- [ ] All I/O is `async`/`await` with `CancellationToken`
- [ ] Nullable reference types respected; no `!` suppressions without comment
- [ ] Audit timestamps are `long` Unix values (not `DateTime`)

**Build Quality**
- [ ] No compiler warnings (TreatWarningsAsErrors=true)
- [ ] No `// TODO` left in committed code
- [ ] Configuration read from `IConfiguration` via named sections

**Tests**
- [ ] Test project exists (`SharedKernel.{Name}.Test`)
- [ ] Uses xUnit `[Fact]`/`[Theory]`
- [ ] Integration tests use Testcontainers, not mocks for infrastructure
- [ ] Coverage contributes toward the ≥55% SonarQube gate

## Output format

```
## Module Review: SharedKernel.{Name}

### Summary
<2–3 sentence overview>

### Critical Issues 🔴
<must-fix before merging>

### Warnings ⚠️
<should-fix — convention violations, missing tests>

### Suggestions 💡
<nice-to-have>
```

After the report, ask if fixes should be applied.
