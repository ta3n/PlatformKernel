Build the solution and report any errors or warnings.

Run the following commands in sequence and report the result:

```bash
docker compose ps --format json
dotnet restore
dotnet build --no-restore --tl --no-incremental -warnaserror -maxcpucount
dotnet list package --vulnerable
```

If Docker services are not running, warn the user that integration tests will fail and suggest running `docker compose up -d` first.

Report any build errors with file path and line number. All warnings are treated as errors — do not ignore them.
