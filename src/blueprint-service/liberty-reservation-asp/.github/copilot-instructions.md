## General

* Make only high confidence suggestions when reviewing code changes.
* Always use the latest version C#, currently C# 12 features.
* Never change global.json unless explicitly asked to.
* Never change package.json or package-lock.json files unless explicitly asked to.
* Follow the project structure in src/ directory:
  * liberty-asp/: Main ASP.NET Core application
  * services/: Microservices and service implementations
  * modules/: Reusable modules and shared components

## Formatting

* Apply code-formatting style defined in `.editorconfig`.
* Prefer file-scoped namespace declarations and single-line using directives.
* Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
* Ensure that the final return statement of a method is on its own line.
* Use pattern matching and switch expressions wherever possible.
* Use `nameof` instead of string literals when referring to member names.
* Ensure that XML doc comments are created for any public APIs. When applicable, include `<example>` and `<code>` documentation in the comments.

### Nullable Reference Types

* Declare variables non-nullable, and check for `null` at entry points.
* Always use `is null` or `is not null` instead of `== null` or `!= null`.
* Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

### Testing

* We use xUnit SDK v3 for tests.
* Do not emit "Act", "Arrange" or "Assert" comments.
* Use Moq for mocking in tests.
* Copy existing style in nearby files for test method names and capitalization.
* Follow the test organization in the project:
  * Unit tests should be in the same directory structure as the source code
  * Integration tests should be in dedicated test projects
  * Use docker-compose.override.test.yml for test environment setup

### Docker and Infrastructure

* Follow the Docker setup in docker/ directory
* Use docker-compose.yml for local development
* Use docker-compose.override.dev.yml for development-specific overrides
* Follow the buildspec.yml for CI/CD pipeline configurations

### Documentation

* Keep documentation up to date in docs/ directory
* Update README.md when making significant changes
* Document API changes in detail-design/ directory
* Follow the existing documentation style and format
