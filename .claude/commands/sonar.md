Run SonarQube static analysis for the solution.

## Prerequisites check

Before running, verify:
1. Docker services are running: `docker compose ps`
2. Required tools are installed:
   ```bash
   dotnet tool list -g | grep -E "sonarscanner|coverlet|dotnet-coverage"
   ```
   If missing, install them:
   ```bash
   dotnet tool install --global dotnet-sonarscanner
   dotnet tool install --global coverlet.console
   dotnet tool install --global dotnet-coverage
   ```

## Run analysis

```bash
# Linux/macOS
./sonar-analysis.sh

# Windows
./SonarAnalysis.ps1
```

## Interpret results

After analysis completes, report:
- Overall quality gate status (pass/fail)
- Code coverage percentage (must be ≥55% to pass the gate)
- Any new bugs, code smells, or security hotspots introduced
- Link to the SonarQube dashboard (typically http://localhost:9000)

If the quality gate fails due to coverage, identify which modules have the lowest coverage and suggest where to add tests.
