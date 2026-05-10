Audit NuGet dependencies for known CVEs and check for version constraint violations.

## Steps

1. **CVE audit:**
   ```bash
   dotnet list package --vulnerable
   ./dependency-audit/dependency-audit.sh
   ```

2. **Version lock check** — verify these packages are pinned to their required versions:
   - `MassTransit` family → must be exactly `8.2.3` (do NOT suggest upgrading to 9.x)
   - `Microsoft.*` packages → must target `net8.0` / `8.x.x`

3. **Report findings:**
   - List any vulnerable packages with CVE IDs and severity
   - Flag any package that violates version constraints
   - Suggest safe remediation (patch version bumps only where they don't conflict with constraints)

## Critical constraint reminder

MassTransit is intentionally locked at 8.2.3. Do NOT suggest upgrading to v9+ under any circumstances — see `docs/adr-001-keep-masstransit-8x.md`.
