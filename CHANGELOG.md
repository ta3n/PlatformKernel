# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.3] - 2026-04-19

### Added

- Firebase notification module (`SharedKernel.FirebaseNotification`) with test service ([5985325](https://github.com/ta3n/PlatformKernel/commit/5985325))
- Firebase integration documentation and quick start guide ([19c0e03](https://github.com/ta3n/PlatformKernel/commit/19c0e03))
- Workflow engine module (`SharedKernel.WorkflowEngineElsa`) with initial implementation ([7a86c84](https://github.com/ta3n/PlatformKernel/commit/7a86c84))
- `dotnet-counters` task for monitoring service performance ([b3eca17](https://github.com/ta3n/PlatformKernel/commit/b3eca17))
- GitHub agents configuration (`AGENTS.md`) and normalized line endings ([bfbb033](https://github.com/ta3n/PlatformKernel/commit/bfbb033))

### Changed

- Revamped README structure and updated bulk insert test data ([74845da](https://github.com/ta3n/PlatformKernel/commit/74845da))
- Replaced `global::Mediator` references with `Mediator` for namespace simplification ([7a86c84](https://github.com/ta3n/PlatformKernel/commit/7a86c84))
- Changed multiple `record`/`class` access modifiers to `internal` for improved encapsulation ([7a86c84](https://github.com/ta3n/PlatformKernel/commit/7a86c84))

### Fixed

- Formatting inconsistencies in Firebase documentation ([6b67da4](https://github.com/ta3n/PlatformKernel/commit/6b67da4))

## [0.0.2] - 2026-03-29

### Added

- Bulk insert pipeline module (`SharedKernel.BulkInsertPipeline`) with PostgreSQL support and lab endpoints ([9314ff2](https://github.com/ta3n/PlatformKernel/commit/9314ff2), [040070b](https://github.com/ta3n/PlatformKernel/commit/040070b))
- Fluent bulk insert module (`SharedKernel.BulkInsert`) with PostgreSQL support, options, and attributes ([c5565db](https://github.com/ta3n/PlatformKernel/commit/c5565db), [594671c](https://github.com/ta3n/PlatformKernel/commit/594671c), [9344dbd](https://github.com/ta3n/PlatformKernel/commit/9344dbd))
- Single Flight Cache service with integration tests ([832a516](https://github.com/ta3n/PlatformKernel/commit/832a516))
- TimescaleDB demo project with PgBouncer integration and guide ([d8e9c54](https://github.com/ta3n/PlatformKernel/commit/d8e9c54), [d5251be](https://github.com/ta3n/PlatformKernel/commit/d5251be))
- ADR-001 for keeping MassTransit 8.x and hybrid Rebus + CAP analysis ([02d141c](https://github.com/ta3n/PlatformKernel/commit/02d141c))
- Hangfire integration tests and Redis support ([0226545](https://github.com/ta3n/PlatformKernel/commit/0226545))
- ElasticSearch module (`SharedKernel.ElasticSearch`) with Testcontainers coverage ([715adf1](https://github.com/ta3n/PlatformKernel/commit/715adf1))
- Redis cache services with Testcontainers integration tests ([83ee8df](https://github.com/ta3n/PlatformKernel/commit/83ee8df))
- Mediator as modern CQRS alternative to MediatR ([822817f](https://github.com/ta3n/PlatformKernel/commit/822817f), [54e0170](https://github.com/ta3n/PlatformKernel/commit/54e0170))
- MassTransit Kafka support and end-to-end outbox test app ([fb6f2a4](https://github.com/ta3n/PlatformKernel/commit/fb6f2a4))
- Shared kernel test projects added to solution ([dbb50be](https://github.com/ta3n/PlatformKernel/commit/dbb50be))

### Changed

- Refactored ElasticSearch service and extensions for cleaner method calls ([7c6cf7c](https://github.com/ta3n/PlatformKernel/commit/7c6cf7c))
- Refactored code structure for improved readability and maintainability ([5e5ddb9](https://github.com/ta3n/PlatformKernel/commit/5e5ddb9))

### Removed

- Obsolete integration event classes for booking and email notifications ([5a15198](https://github.com/ta3n/PlatformKernel/commit/5a15198))
- Unused using directives across various files ([e12d921](https://github.com/ta3n/PlatformKernel/commit/e12d921))

### Fixed

- Updated Magick.NET package version and changed Kestrel protocols in appsettings ([5447efe](https://github.com/ta3n/PlatformKernel/commit/5447efe))

## [0.0.1] - 2026-02-24

### Added

- Core entity framework: `BaseEntity`, `BaseAuditLog`, pagination (`IPageable`, `IPage<T>`), and exception handling ([248f5fe](https://github.com/ta3n/PlatformKernel/commit/248f5fe))
- Base service and interface for generic CRUD operations with caching and logging ([fca35ad](https://github.com/ta3n/PlatformKernel/commit/fca35ad))
- Base entity type configurations for data and relation entities ([e4b989d](https://github.com/ta3n/PlatformKernel/commit/e4b989d))
- Initial project structure with essential configurations ([ddc9561](https://github.com/ta3n/PlatformKernel/commit/ddc9561))
- CQRS module (`SharedKernel.CQRS`) with command base classes and handlers ([9b51218](https://github.com/ta3n/PlatformKernel/commit/9b51218))
- Repository base module (`SharedKernel.RepositoryBase`) and service base module (`SharedKernel.ServiceBase`) ([9b51218](https://github.com/ta3n/PlatformKernel/commit/9b51218))
- `JsonSettings` utility in `SharedKernel.AppShared` replacing Newtonsoft.Json with System.Text.Json ([9b51218](https://github.com/ta3n/PlatformKernel/commit/9b51218))
- Dependency audit tool for dependency management and reporting ([8c4caa2](https://github.com/ta3n/PlatformKernel/commit/8c4caa2))
- Concurrency control documentation for booking using database and Redis locks ([8c4caa2](https://github.com/ta3n/PlatformKernel/commit/8c4caa2))
- Comprehensive project documentation and setup guide ([71883b4](https://github.com/ta3n/PlatformKernel/commit/71883b4))
- Docker support with multi-stage builds for .NET 8, 9, and 10 ([6ff5445](https://github.com/ta3n/PlatformKernel/commit/6ff5445))
- S3 upload performance documentation ([6dca90a](https://github.com/ta3n/PlatformKernel/commit/6dca90a))
- Blueprint service templates (clean-architecture, JHipster) ([ee816f8](https://github.com/ta3n/PlatformKernel/commit/ee816f8))

### Changed

- Renamed `SharedKernel.SysException` to `SharedKernel.Exception` across all namespaces ([a2df257](https://github.com/ta3n/PlatformKernel/commit/a2df257))
- Renamed `SharedKernel.ApplicationShared` to `SharedKernel.AppShared` ([9b51218](https://github.com/ta3n/PlatformKernel/commit/9b51218))
- Replaced Newtonsoft.Json with System.Text.Json across all projects ([9b51218](https://github.com/ta3n/PlatformKernel/commit/9b51218))
- Refactored namespaces and consolidated project structure under SharedKernel ([e376be4](https://github.com/ta3n/PlatformKernel/commit/e376be4))
- Updated `redis-commander` Docker image from `latest` to `redis-commander-210` ([4423cf8](https://github.com/ta3n/PlatformKernel/commit/4423cf8))
- Removed XML comments from interfaces and entities for consistency ([53e1954](https://github.com/ta3n/PlatformKernel/commit/53e1954))

### Removed

- `SharedKernel.SysIntegrationEvent` project (consolidated into `SharedKernel.Exception`) ([a2df257](https://github.com/ta3n/PlatformKernel/commit/a2df257))
- Blueprint Service projects temporarily removed from solution ([4be86a3](https://github.com/ta3n/PlatformKernel/commit/4be86a3))
- Outdated ASP.NET Core project rules ([601a008](https://github.com/ta3n/PlatformKernel/commit/601a008))

[0.0.3]: https://github.com/ta3n/PlatformKernel/compare/7a86c84...HEAD
[0.0.2]: https://github.com/ta3n/PlatformKernel/compare/fb6f2a4...7a86c84
[0.0.1]: https://github.com/ta3n/PlatformKernel/compare/248f5fe...ac385aa
