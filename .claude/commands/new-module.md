Scaffold a new SharedKernel module. Requires one argument: the module name (without the `SharedKernel.` prefix).

Usage: `/new-module Cache2` → creates `SharedKernel.Cache2` and `SharedKernel.Cache2.Test`

## What to create

Follow the standard layout in `src/shared-kernel/`. Use an existing simple module (e.g., `SharedKernel.Cache`) as a reference.

### Library project structure
```
src/shared-kernel/SharedKernel.{Name}/
  Abstractions/     ← interfaces only (I{Name}Service.cs)
  Extensions/       ← IServiceCollection extension ({Name}ServiceCollectionExtensions.cs)
  Options/          ← {Name}Options.cs (if config-driven)
  Services/         ← {Name}Service.cs implementation
  SharedKernel.{Name}.csproj
  README.md
```

### Test project structure
```
src/shared-kernel/SharedKernel.{Name}.Test/
  Unit/
  SharedKernel.{Name}.Test.csproj
```

## csproj templates

**Library:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="8.0.0" />
  </ItemGroup>
</Project>
```

**Test:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\SharedKernel.{Name}\SharedKernel.{Name}.csproj" />
  </ItemGroup>
</Project>
```

## DI extension pattern

```csharp
public static class {Name}ServiceCollectionExtensions
{
    public static IServiceCollection Add{Name}(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<{Name}Options>(configuration.GetSection("{SectionName}"));
        services.TryAddSingleton<I{Name}Service, {Name}Service>();
        return services;
    }
}
```

## Rules
- Types are `internal sealed` by default
- Use `TryAdd*` for DI registrations
- No business logic — infrastructure only
- No cross-module dependencies (except `SharedKernel.Entity` or `SharedKernel.Exception`)
- After creating files, add both projects to `PlatformKernel.sln` using `dotnet sln add`
