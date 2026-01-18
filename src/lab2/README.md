Liberty Project

## Table of Contents

- [Table of Contents](#table-of-contents)
- [Introduction](#introduction)
  - [Purpose](#purpose)
  - [Scope](#scope)
- [Environment Setup](#environment-setup)
  - [Installing .NET 8 SDK](#installing-net-8-sdk)
  - [Development Tools](#development-tools)
- [Project Structure](#project-structure)
  - [Folder code base](#folder-code-base)
  - [Solution explorer](#solution-explorer)
  - [Components of a service](#components-of-a-service)
  - [Detail components of a service](#detail-components-of-a-service)
  - [Layer Dependencies of a service](#layer-dependencies-of-a-service)
  - [Implement the business domain flow of a service](#implement-the-business-domain-flow-of-a-service)
  - [Implement the normarl flow of a service](#implement-the-normarl-flow-of-a-service)
- [Git Convention](#git-convention)
  - [General Rules](#general-rules)
  - [Semantic Subjects](#semantic-subjects)
  - [Tags](#tags)
  - [Branch Name](#branch-name)
  - [Pull Request Name](#pull-request-name)
- [Code Review Checklist](#code-review-checklist)
  - [General](#general)
  - [Commenting](#commenting)
  - [Source Code](#source-code)
- [Coding Guideline](#coding-guideline)
  - [Get liberty-asp submodules](#get-liberty-asp-submodules)
  - [Migration](#migration)
    - [Step 1: Navigate to the project containing the DbContext](#step-1-navigate-to-the-project-containing-the-dbcontext)
    - [Step 2: Create a new migration](#step-2-create-a-new-migration)
    - [Step 3: Generate the SQL script](#step-3-generate-the-sql-script)
    - [Step 4: Run the migration to update the database using DbUp](#step-4-run-the-migration-to-update-the-database-using-dbup)
  - [Code quality](#code-quality)
    - [Monitoring and Tracking the development local](#monitoring-and-tracking-the-development-local)
- [Version Control and CI/CD](#version-control-and-cicd)
- [References](#references)

---

## Introduction

#### Purpose

Liberty Microservices is a Sample application for PMS(Property Management System). This application based on different
software architecture and technologies like .Net 8, C# 12, CQRS, Clean architecture + DDD, Vertical Slice Architecture,
Docker, masstransit, RabbitMQ, Redis, MySql, Entity Framework Core and different level of testing.

#### Scope

This document is intended for software developers, architects, and project managers involved in .NET development
projects. It covers essential aspects such as environment setup, project structure, coding conventions, best practices,
version control strategies, and CI/CD implementation. Whether you're starting a new project or looking to optimize an
existing code base, this guide will equip you with the knowledge needed to establish a robust foundation and maintain a
high standard of code quality throughout the development lifecycle.

---

## Environment Setup

#### Installing .NET 8 SDK

1. Windows: https://github.com/dotnet/core/blob/main/release-notes/8.0/install-windows.md
2. macOS: https://github.com/dotnet/core/blob/main/release-notes/8.0/install-macos.md
3. Linux: https://github.com/dotnet/core/blob/main/release-notes/8.0/install-linux.md

#### Development Tools

- [Visual Studio 2022 or later](https://visualstudio.microsoft.com/): This is a popular integrated development
  environment (IDE) for .NET. Download and install it from Microsoft's official website. Extensions should install:
  - [SonarLint for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=SonarSource.SonarLintforVisualStudio2022)
  - [Visual Studio Spell Checker (VS2022 and Later)](https://marketplace.visualstudio.com/items?itemName=EWoodruff.VisualStudioSpellCheckerVS2022andLater)
- [Visual Studio Code](https://code.visualstudio.com/): A lighter-weight but powerful editor with extensions for .NET
  development. Install it from the official website or through extensions in VS Code. Extensions should install:
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
  - [Test Explorer UI](https://marketplace.visualstudio.com/items?itemName=hbenl.vscode-test-explorer)
    and [Test Adapter Converter](https://marketplace.visualstudio.com/items?itemName=ms-vscode.test-adapter-converter)
  - [EditorConfig for VS Code](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)
  - [SonarLint](https://marketplace.visualstudio.com/items?itemName=SonarSource.sonarlint-vscode)
  - [Code Spell Checker](https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker)
  - [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

---

## Project Structure

#### Folder code base

<div style="display: flex; justify-content: space-between;">
  <div style="width: 48%;">
    <h5>Current structure</h5>

    + liberty-asp (Git submodules)
    + Liberty.Reservation.Application
    + Liberty.Reservation.Application.Test
    + Liberty.Reservation.Employee.WebAPI
    + Liberty.Reservation.Employee.WebAPI.Application
    + Liberty.Reservation.User.WebAPI
    + Liberty.Reservation.User.WebAPI.Application
    + Liberty.Reservation.Manager.WebAPI
    + Liberty.Reservation.Manager.WebAPI.Application
    + Liberty.Reservation.Site.WebAPI
    + Liberty.Reservation.Site.WebAPI.Application
    - docker-compose.yml
    - Liberty.Reservation.sln

  </div>
  <div style="width: 48%;">
    <h5>Proposed structure</h5>

    - .vscode
    - docker
    - src
      - commons/liberty-asp (Git submodules)
        + Liberty.ApplicationShared (formerly Liberty.Application)
        + Liberty.ApplicationShared.Test (formerly Liberty.Application)
        + Liberty.Entity
        + Liberty.Exception
        + Liberty.OpenTelemetry
        + Liberty.Pagination
        + Liberty.Specification
        + Liberty.UnitOfWork
        + and so more ...
      - modules
        + Liberty.Reservation.Application
      - services
        - employee
          + Liberty.Reservation.Employee.WebApi
          + Liberty.Reservation.Employee.WebApi.Application
          + Liberty.Reservation.Employee.Application
          + Liberty.Reservation.Employee.WebApi.Test
        - site
          + ...
        - manager
          + ...
        - user
          + ...
        + and so more ...
      + background-jobs
      + integration-event
      + Liberty.AppHost
    - .dockerignore
    - .editorconfig
    - .gitignore
    - docker-compose.yml
    - nuget.config
    - sonar-analysis.sh
    - SonarAnalysis.ps1
    - SonarQube.Analysis.xml
    - Liberty.Reservation.sln

  </div>
</div>

1. **.vscode**: Directory used by Visual Studio Code (VS Code), a popular code editor developed by Microsoft. This
   folder typically contains configuration files and settings specific to the project or workspace it resides in. Here
   are some common files and their purposes within the .vscode folder: settings.json, launch.json, tasks.json, v.v...

2. **docker**: Contains the configuration files and resources required to build and deploy Docker containers to the
   project via docker-compose.

3. **src**: Contains the main source code of the application. This is where developers place source code files,
   configurations, and other components necessary to build and run the application.

4. **.editorconfig**: Used to maintain consistent source code formatting rules within a project. It helps developers
   working on the same project adhere to the same source code formatting rules when using IDEs like Visual Studio 2022
   or Visual Studio Code. (https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/code-style-rule-options).

5. **.dockerignore**: Used to specify files and directories that should be ignored by Docker when building an image.
   This is similar to a .gitignore file used by Git to ignore files and directories in version control.

6. **.gitignore**: Specifies which files and directories should be ignored by Git. This means that any files or
   directories listed in the .gitignore file will not be tracked by Git, and changes to these files will not be included
   in commits.

7. **docker-compose.yml**: A configuration file used by Docker Compose to define and manage multi-container Docker
   applications. It allows you to specify the services, networks, and volumes that your application requires.

8. **nuget.config**: Used to configure NuGet, the package manager for .NET. This file typically contains settings that
   control how NuGet behaves when restoring and managing packages from https://api.nuget.org/v3/index.json

9. **sonar-analysis.sh**: A shell script used to perform static code analysis using SonarQube, a popular tool for
   continuous inspection of code quality.

10. **SonarAnalysis.ps1**: A PowerShell script designed to perform a SonarQube analysis on the project.

11. **SonarQube.Analysis.xml**: A configuration file used for integrating the SonarQube static code analysis tool with
    the project.

12. **Liberty.Reservation.sln**: A solution file used by Microsoft Visual Studio and other compatible Integrated
    Development Environments (IDEs) to manage a collection of projects.

#### Solution explorer

<div style="display: flex; justify-content: space-between;">
  <div style="width: 48%;">
    <h5>Current structure</h5>

    + Liberty.Application
    + Liberty.Application.Test
    - Liberty.Reservation.Application
      + Consts
      + Contexts
        + DataContexts
          - Models
            + Contents
            + Datas
            + Metas
            + Relations
          - DataContext.cs
      - Domains
        + Repositories
        + Services
      + Exceptions
      + Extensions
      + Templates
    + Liberty.Reservation.Application.Test
    + Liberty.Reservation.Employee.WebAPI
    + Liberty.Reservation.Employee.WebAPI.Application
    + Liberty.Reservation.Manager.WebAPI
    + Liberty.Reservation.Manager.WebAPI.Application
    + Liberty.Reservation.Site.WebAPI
    + Liberty.Reservation.Site.WebAPI.Application
    + Liberty.Reservation.User.WebAPI
    + Liberty.Reservation.User.WebAPI.Application

  </div>
  <div style="width: 48%;">
    <h5>Proposed structure</h5>

    - Liberty.Reservation
    - Commons
      + Liberty.ApplicationShared (formerly Liberty.Application)
      + Liberty.Entity
      + Liberty.Exception
      + Liberty.OpenTelemetry
      + Liberty.Pagination
      + Liberty.Specification
      + Liberty.UnitOfWork
      + and so more ...
    - Modules
      - Liberty.Reservation.Application
        + Behaviors
        + Constants
        + Contexts
        + Cqrs
        + Domains
        + Exceptions
        + Extensions
        + Templates
      + Liberty.Reservation.Application.Test
    - Services
      - Employee
        + Liberty.Reservation.Employee.WebApi
        + Liberty.Reservation.Employee.WebApi.Application
        + Liberty.Reservation.Employee.Application
        + Liberty.Reservation.Employee.WebApi.Test
      - Manager
        + ...
      - Site
        + ...
      - User
        + ..
      + and so more ...
    + Liberty.AppHost

  </div>
</div>

1. **Liberty.Reservation/Commons**: contains common or shared code that is used across multiple modules or services
   within the liberty-reservation-asp application. This can include utility classes, helper functions, base classes, and
   other reusable components that are not specific to a single module but are needed by various parts of the
   application. **Plugin mechanism for adding shared parts**.

- **Liberty.ApplicationShared**: Contains shared application logic and utilities that are used across different parts
  of the Liberty Reservation application. This can include common services, utilities, base classes, and other shared
  components that are not specific to a single module but are used throughout the application. Here are some possible
  contents and purposes of files within this folder:

  - **Domains/Repositories**: Interfaces and base classes for repositories that handle data access logic.
  - **Logging**: Custom logging behaviors and utilities.
  - **Utils**: Utility classes and methods for common tasks such as email building, date manipulation, etc.
  - **Extensions**: Extension methods that add functionality to existing types.
  - **Cqrs/BaseCommands**: Base classes for handling commands in a CQRS (Command Query Responsibility Segregation)
    pattern.

- **Liberty.Cache**: Provides distributed caching functionality using Redis. Includes cache service with operations
  for get/set/remove data, connection pooling, bulk operations, pattern-based key management, distributed locking
  mechanism, and Lua script execution for optimized Redis operations. Supports both in-memory and Redis caching with
  configurable TTL, sliding expiration, and connection retry policies.

- **Liberty.Entity**: Contains entity definitions and interfaces related to the entities. Such as ILoginHistory,
  IHasCreator, ...

- **Liberty.Fax**: Provides fax sending functionality via FaxImo REST API. Includes HTTP-based fax service with
  authentication, PDF conversion utilities, configurable retry mechanism, and support for Japanese characters.
  Handles fax requests with recipient numbers, subject, and body content.

- **Liberty.GmoPaymentGateway**: Provides payment gateway integration with GMO Payment service. Includes payment
  transaction operations (EntryTran, ExecTran, SearchTrade, Cancel, ChangeOrder), error code management, 3D Secure
  support, multi-currency payment handling, and comprehensive GMO error code mapping with user-friendly messages for
  payment failures.

- **Liberty.Hangfire**: Provides background job processing using Hangfire with PostgreSQL storage. Includes recurring
  job scheduling with Cron expressions, scheduled jobs execution at specific times or with delays, custom retry
  mechanisms, dashboard with basic authentication, and job management utilities with configurable time zones.

- **Liberty.MassTransit**: Provides message queue integration using MassTransit with RabbitMQ support. Includes
  message broker configuration, publish-subscribe patterns, batch publishing with publisher confirmation, queue
  settings with durable and lazy mode, and customizable bus registration with factory configurators.

- **Liberty.Media**: Provides file storage and management with AWS S3 integration. Includes file upload/download/delete
  operations, image resizing with multiple size types, multipart upload for large files, pre-signed URL generation,
  file validation with extension and size limits, and RESTful endpoints for file operations with ImageMagick processing.

- **Liberty.Pagination**: Contains code related to pagination functionality within the Liberty Reservation
  application. Such as Order, Sort, PageableBinderConfig, ...

- **Liberty.SentrySelfHosted**: A library for integrating Sentry into ASP.NET Core applications within the Liberty system.
  It provides extension methods to easily configure and use Sentry (log, trace, error monitoring) via appsettings, supports
  Entity Framework and OpenTelemetry, allows custom log filtering and event handling, and helps standardize centralized
  error monitoring across all microservices.

- **Liberty.Serilog**: A logging library for .NET applications in the Liberty system, providing centralized and extensible
  logging using Serilog. It supports various sinks such as Console, Seq, AWS CloudWatch, OpenTelemetry, and Sentry, as well
  as enrichers for environment and exception details. The library enables structured logging, asynchronous log processing,
  and seamless integration with ASP.NET Core and Entity Framework, helping standardize and enhance observability across
  all microservices.

- **Liberty.ServiceDefaults**: A shared library providing default configurations and integrations for .NET microservices in
  the Liberty system. It includes built-in support for OpenTelemetry tracing, resilient HTTP client policies, and service
  discovery, helping standardize observability, reliability, and distributed tracing across all services.

- **Liberty.Specification**: Contains code related to the Specification pattern. The Specification pattern is a
  software design pattern that allows for the creation of business rules that can be combined and reused. It is often
  used to encapsulate the logic for querying and filtering data in a way that is both flexible and maintainable. Such
  as SpecificationBase, GridSpecificationBase, IGridSpecification, ...

  - **Specification Interface**: Defines the contract for specifications, typically including methods like
    IsSatisfiedBy which checks if a given object meets the criteria of the specification.
  - **Composite Specifications**: These are specifications that combine other specifications using logical operations
    like AND, OR, and NOT.
  - **Concrete Specifications**: These are specific implementations of the specification interface that encapsulate
    particular business rules.
  - **Specification Builder**: A utility to help construct complex specifications from simpler ones.
  - **Extensions and Helpers**: Additional methods and utilities to work with specifications, such as converting them
    to expressions that can be used with LINQ queries.

- **Liberty.SysException**: Contains classes and files related to exception handling within the Liberty Reservation
  application. This could include custom exception classes, exception handling utilities, and possibly configurations
  for how exceptions are managed and logged within the application. Custom Exception Classes: These are classes that
  extend the base exception class to provide more specific error information. For example, AppNotFoundException,
  AppInvalidException, and AppAuthException.

- **Liberty.SysIntegrationEvent**: A shared library for defining and handling integration events in the Liberty system.
  It provides base event contracts and utilities for event-driven communication between microservices, leveraging
  MassTransit for message transport and supporting reliable, decoupled integration patterns.

- **Liberty.UnitOfWork**: Contains the implementation of the Unit of Work pattern for the Liberty Reservation
  application. The Unit of Work pattern is a design pattern used to manage changes to a set of objects by coordinating
  the writing out of changes and the resolution of concurrency problems. Such as AppDbContextBase và
  CustomDbConnectionInterceptor.

  - **Interfaces**: Definitions of the Unit of Work and repository interfaces. These interfaces define the contract
    for the operations that can be performed, such as committing transactions, managing repositories, and handling
    database operations.

  - **Implementations**: Concrete classes that implement the Unit of Work and repository interfaces. These classes
    handle the actual database operations, transaction management, and coordination of changes.

  - **Repositories**: Generic and specific repository implementations that provide data access methods for different
    entities. These repositories interact with the database context to perform CRUD (Create, Read, Update, Delete)
    operations base on IGenericRepository from the Liberty.ApplicationShared common.

  - **Helpers and Utilities**: Additional helper classes and utilities that support the Unit of Work and repository
    implementations, such as transaction management.

**Summery**: Provides plugin-style shared libraries for cross-cutting concerns such as caching(Liberty.Cache)
,payment gateway integration (Liberty.GmoPaymentGateway), message brokering (Liberty. MassTransit),
background jobs (Liberty.Hangfire), file storage (Liberty.Media), centralized logging and monitoring
(Liberty.Serilog, Liberty.SentrySelfHosted), distributed tracing and service defaults (Liberty.ServiceDefaults),
exception handling (Liberty.SysException), integration events (Liberty.SysIntegrationEvent), and infrastructure
patterns like Unit of Work and Specification (Liberty.UnitOfWork, Liberty.Specification).

2. **Modules/Liberty.Reservation.Application**: Contains the application layer of the Liberty Reservation module of the
   Liberty system. The class contains basic classes to share with the services of the Liberty Reservation module.

- **Behavior**: Contains behavior declarations of the MediaR library. Behaviors are pipeline behaviors that allow you
  to add additional processing steps before and after a request is handled by the main handler. This can include
  functions like logging, validation, caching, or any logic you want to apply around request handling.

- **Constants**: Contain constant values used throughout the application, such as route types, status codes, or other
  fixed values.

- **Contexts/DataContexts**: Contains all entities and entity configuration of the entire module and shares it with
  services in the Liberty Reservation module. When the service is used, it will specify each DbSet of each entity in
  the service's DbContext.

   ```cs
   public class BedType : EntityData
   {
     /// <summary>
     /// 名称
     /// </summary>
     public string? Name { get; set; }

     /// <summary>
     ///  単位:ex 組み・台
     /// </summary>
     public BedTypeUnitTypes BedTypeUnitType { get; set; }

     /// <summary>
     /// 部屋-ベッドタイプリリレーション
     /// </summary>
     public ICollection<RoomGroupBedType>? RoomGroupBedType { get; set; }
   }
   ```

   ```cs
   public class BedTypeConfiguration : BaseDataEntityTypeConfiguration<BedType>
   {
     protected override void EntityConfigure(
         EntityTypeBuilder<BedType> builder
     )
     {
       builder.ToTable("BedType", DbConfiguration.DefaultSchema);

       builder
           .HasMany(c => c.RoomGroupBedType)
           .WithOne(c => c.BedType)
           .HasForeignKey(c => c.BedTypeId);
     }
   }
   ```

- **Cqrs**: Contains wrap base classes implementing the Command Query Responsibility Segregation (CQRS) pattern,
  including command and query handlers. Such as:

  - **Command**: CommandBase, CreateCommandBase, UpdateCommandBase, DeleteCommandBase implementation from interfaces
    ICommandBase of the Liberty.ApplicationShared common.

  ```cs
  public abstract record CommandBase<TResponse> : RequestBase<TResponse>, ICommandBase<TResponse>;

   public abstract record CreateCommandBase<TModel, TResponse>
       : RequestBase<TResponse>, ICreateCommandBase<TModel, TResponse>
       where TModel : class
   {
       public required TModel Payload { get; set; }
   }

   public abstract record UpdateCommandBase<TModel, TResponse>
       : RequestBase<TResponse>, IUpdateCommandBase<TModel, TResponse>
       where TModel : class
   {
       public required TModel Payload { get; set; }
   }

   public abstract record DeleteCommandBase<TModel, TResponse>
       : RequestBase<TResponse>, IDeleteCommandBase<TModel, TResponse>
       where TModel : class
   {
       public required TModel Payload { get; set; }
   }
  ```

  ```cs
   public abstract class CommandBaseHandler<TCommand, TResponse>(
       IUnitOfWork unitOfWork,
       IMapper mapper
   ) : ICommandHandlerBase<TCommand, TResponse>
       where TCommand : ICommandBase<TResponse>
   {
       protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
       protected IMapper Mapper { get; } = mapper;

       public virtual Task<TResponse> Handle(
           TCommand request,
           CancellationToken cancellationToken
       )
       {
           return HandleAsync(request, cancellationToken);
       }

       protected abstract Task<TResponse> HandleAsync(
           TCommand request,
           CancellationToken cancellationToken
       );
   }

   public abstract class CreateCommandHandlerBase<TCommand, TResponse>(
       IUnitOfWork unitOfWork,
       IMapper mapper
   ) : CommandBaseHandler<TCommand, TResponse>(
           unitOfWork,
           mapper
       ),
       ICreateCommandHandlerBase<TCommand, TResponse>
       where TCommand : ICommandBase<TResponse>;

   public abstract class UpdateCommandHandlerBase<TCommand, TResponse>(
       IUnitOfWork unitOfWork,
       IMapper mapper
   ) : CommandBaseHandler<TCommand, TResponse>(
           unitOfWork,
           mapper
       ),
       IUpdateCommandHandlerBase<TCommand, TResponse>
       where TCommand : ICommandBase<TResponse>;

   public abstract class DeleteCommandHandlerBase<TCommand, TResponse>(
       IUnitOfWork unitOfWork,
       IMapper mapper
   ) : CommandBaseHandler<TCommand, TResponse>(
           unitOfWork,
           mapper
       ),
       IDeleteCommandHandlerBase<TCommand, TResponse>
       where TCommand : ICommandBase<TResponse>;
  ```

  - **Query**: IQueryPagedBase and query interfaces of the Liberty.ApplicationShared common.

  ```cs
     public interface IQueryBase<TResponse> : IRequestBase<(IHeaderDictionary, TResponse)>;

     public interface IQuerySingleBase<TResponse> : IQueryBase<TResponse>;

     public interface IQueryListBase<TResponse> : IQueryBase<IEnumerable<TResponse>>;
  ```

  ```cs
     public interface IQueryBaseHandler<in TQuery, TResponse>
         : IRequestHandler<TQuery, (IHeaderDictionary, TResponse)>
         where TQuery : IQueryBase<TResponse>;

     public interface IQuerySingBaseHandler<in TQuery, TResponse>
         : IQueryBaseHandler<TQuery, TResponse>
         where TQuery : IQuerySingleBase<TResponse>, IQueryBase<TResponse>;

     public interface IQueryListBaseHandler<in TQuery, TResponse>
         : IQueryBaseHandler<TQuery, IEnumerable<TResponse>>
         where TQuery : IQueryListBase<TResponse>;
  ```

- **Domain**: Contains repositories and services commons and shared for services belonging to the module.

- **Exceptions**: Contains exceptions common and shared for services belonging to the module.

- **Extensions**: Contains extensions common and shared for services belonging to the module.

- **Template**: Contains template files used for generating various types of documents, emails, or other formatted
  outputs related to reservations. These templates might be used to create standardized content such as reservation
  confirmations, cancellation notices, or other communications that need to be dynamically populated with data.

3. **Liberty.AppHost**: .NET Aspire project templates offer a sophisticated dashboard for comprehensive app monitoring
   and inspection. This dashboard allows you to closely track various aspects of your app, including logs, traces, and
   environment configurations, in real-time. It's purpose-built to enhance the local development experience, providing
   an insightful overview of your app's state and
   structure. (https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview)

#### Components of a service

```
- Liberty.Reservation.[Service name].WebApi
- Liberty.Reservation.[Service name].WebApi.Application
- Liberty.Reservation.[Service name].Application
```

<img src="./docs/images/LibertySystemArchitect-ComponentArchitectureOfService.drawio.svg" alt="">

#### Detail components of a service

<div style="display: flex; justify-content: space-between;">
  <div style="width: 48%;">
    <h5>Current structure</h5>

    - Liberty.Reservation.Employee.WebAPI
      + Controllers
      + Filters
      + Handlers
      + Initializations
      + Localizes\Resources
      + Migrations
      + Settings
      - I18n.cs
      - Program.cs
    - Liberty.Reservation.Employee.WebAPI.Application
      + Controller

  </div>
  <div style="width: 48%;">
    <h5>Proposed structure</h5>

    - Liberty.Reservation.[Service name].WebApi
      - Configurations
        . AutoMapperStartup.cs
        . DbContextStartup.cs
        . HttpClientStartup.cs
        . MediatRStartup.cs
        . MvcStartup.cs
        . OptionStartup.cs
        . ProblemDetailsStartup.cs
        . RateLimitStartup.cs
        . SecurityStartup.cs
        . ServiceStartup.cs
        . SwaggerStartup.cs
        [...]Startup.cs
      - Filters
        . ActionFilter.cs
      - Handlers
        . ErrorHandler.cs
      - Initializations
        . InitializationService.cs
      - Migrations
      - Program.cs
      - .dockerignore
      - Dockerfile
      - appsettings.json
      - appsettings.Development.json

    - Liberty.Reservation.[Service name].WebApi.Application
      - Boundaries/Restful
        . BaseEndpoint.cs
        . [...]Endpoint.cs
      - Localized
        + Resources
        . I18n.cs
        . I18nEData.cs
      - Mappings
        . AutoMapperProfile.cs
        . [Mapping name]MapperProfile.cs
      - Models
        - Requests
          . [...]Request.cs
          . ...
        - Responses
          . [...]Response.cs
          . ...
        . and so more other model (dto) if need
      - Settings
        . AppSetting.cs
        . DbConnectionStringSetting.cs
        . HttpClientPolicySetting.cs
        . WebCorsSetting.cs
        . ...
      - UserCases
        - Commands
        - Queries
      - Validations
        . [...]Validator.cs
      - Web
      - AssemblyDefinition.cs

    - Liberty.Reservation.[Service name].Application
      + Constants
      - Context
        + Entities
        + Configurations
        + [Service name]DataContext.cs
      - Domains
        - Repositories
          - Interfaces
            I[Repository name]Repository.cs
          [Repository name]Repository.cs
        - Services
          - Interfaces
            I[...]Service.cs
          [...]Service.cs
      + Exceptions
      I[Service name]UnitOfWork
      [Service name]UnitOfWork

  </div>
</div>
1. **Liberty.Reservation.[Service name].WebApi**: Specifically designed to handle web-based API (Application Programming Interface) interactions related.

- **Configurations**: Contains web app configurations and is configured through the c# extension mechanism in Program.cs
  for easy management and clear code. Such as:

  - **AutoMapperStartup**: Specifies the mapper configuration profiles used
  - **DbContextStartup.cs**: Configure DbContext from field variable
   ```cs
     public static IServiceCollection AddDbContext(
         this IServiceCollection services,
         IConfiguration config
     )
     {
         services.Configure<ConnectionPoolOptions>(config.GetSection("ConnectionPool"));
         var connectionPoolOptions = config.GetOptionsExt<ConnectionPoolOptions>("ConnectionPool");
         var dbConnectionManager = new DbConnectionManager(connectionPoolOptions);
         services.AddSingleton<IDbConnectionManager>(_ => dbConnectionManager);

         var inMemoryDb = config.GetConnectionString("InMemoryDatabase");
         var migrationsAssembly = config.GetConnectionString("MigrationsAssembly");
         var connectionString = config.GetConnectionString("DataContextConnection");

         services.AddDbContext<EmployeeDataContext>(
             options =>
             {
                 if (!string.IsNullOrEmpty(inMemoryDb))
                 {
                     // options.UseInMemoryDatabase(inMemoryDb);
                 }

                 options.UseMySql(
                         connectionString,
                         new MySqlServerVersion(new Version(8, 0, 29)),
                         sqlOptions =>
                         {
                             sqlOptions.MigrationsAssembly(migrationsAssembly);
                             sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                         }
                     )
                     .LogTo(Console.WriteLine, LogLevel.Information)
                     .EnableDetailedErrors(false)
                     .EnableSensitiveDataLogging(false)
                     // .AddInterceptors(new CustomDbConnectionInterceptor(dbConnectionManager));

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                }
             }
         );

         services.AddScoped<DbContext>(
             provider => provider.GetRequiredService<EmployeeDataContext>()
         );

         services.AddTransient<IUnitOfWork, EmployeeUnitOfWork>();
         return services;
     }
   ```

  - **MediatRStartup.cs**: Configure command and query requests with handlers, behaviors.

  ```cs
  public static IServiceCollection AddMediatRModule(
    this IServiceCollection services
  )
  {
    services.AddValidatorsFromAssemblyContaining(typeof(AssemblyDefinition));

    services.AddMediatR(typeof(AssemblyDefinition));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExecutionStrategyBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

    return services;
  }
  ```

  - **MvcStartup.cs**: Configure router, api versioning, ...
  - **OptionStartup.cs**: Configure models binding from parameter in appsettings.json via DI
    ```cs
    Init:
      services.Configure<JobSettings>(config.GetSection("JobSettings"));
    Use:
      IOptions<JobSettings> jobSettings
    ```
  - **ProblemDetailsStartup.cs**: Configure about the problem details
  - **RateLimitStartup.cs**: Configure rate limiter according to each field variable parameter
  - **SecurityStartup.cs**: Configure authentication via jwt, Cors,...
  - **ServiceStartup.cs**: Declare dependency injection for used services
  - **SwaggerStartup.cs**: Swagger configuration, such as

  ```cs
    services.AddSwaggerGen(
        options =>
        {
            options.OperationFilter<PageableModelFilter>();
        }
    );
  ```

2. **Liberty.Reservation.[Service name].WebApi.Application**: Focuses on the application logic and exposes these
   functionalities through web-based APIs.

- **Boundaries/Restful**: Contains Restful APIs according to Controller-based Apis

  - **Use Endpoint prefix instead of Controller because:**

    - **More flexibility**: Endpoints allow you to define actions more clearly, not limited by the traditional
      class-based controller structure
    - **Separating logic and routing**: Endpoints allow you to separate business logic and routing, helping to reduce
      complexity and increase source code reuse.

  - **HTTP‑based APIs are RESTful that consists of the following levels:**
    - Level 0 – Clients of a level 0 API invoke the service by making HTTP POST requests to its sole URL endpoint
    - Level 1 – A level 1 API supports the idea of resources
    - Level 2 – A level 2 API uses HTTP verbs to perform actions:GET to retrieve, POST to create, and PUT to update.
      The request query parameters and body, if any, specify the action’s parameters.

  ```cs
   [Route("api/employees")]
   [ApiVersion("1.0")]
   public class EmployeeEndpoint(
       ILogger<EmployeeEndpoint> logger,
       IMediator mediator
   ) : BaseEndpoint(mediator)
   {
       [HttpPost]
       public async Task<IActionResult> CreateEmployee(
           [FromBody] CreateEmployeeRequest request
       )
       {
           logger.LogDebug("REST request to save Employee : {Request}", request);

           var newEmployee = await Mediator.Send(
               new EmployeeCreateCommand { Payload = request }
           );

           return Ok(newEmployee)
               .WithHeaders(
                   HeaderUtil.CreateEntityCreationAlert(
                       "employee",
                       newEmployee
                   )
               );
       }

       [HttpGet]
       public async Task<IActionResult> GetAllEmployees(
           IPageable pageable
       )
       {
           logger.LogDebug("REST request to get a page of Employees");

           var (headers, response) = await Mediator.Send(
               new EmployeeGetAllQuery(
                   pageable
               )
           );
           return Ok(response).WithHeaders(headers);
       }

       [HttpGet("public")]
       public async Task<IActionResult> GetAllPublicEmployees(
           IPageable pageable
       )
       {
           logger.LogDebug("REST request to get a page of Public Employees");

           var (headers, response) = await Mediator.Send(
               new EmployeeGetAllPublicEmployeesQuery(
                   pageable
               )
           );
           return Ok(response).WithHeaders(headers);
       }
   }
  ```

- **Mappings**: Declare the mappings of the AutoMapper library between two objects or configure Select in Linq when
  querying the database.

  ```cs
    var page = await employeeRepository
          .GetQueryableWithAsNoTracking()
          .Include(x => x.Address)
          .Include(x => x.EmployeeMeta)
          .ProjectTo<DataGetEmployeeManyResponse>(Mapper.ConfigurationProvider)
          .UsePageableAsync(
              request.Pageable,
              isApplySort: true,
              cancellationToken: cancellationToken
          );
  ```

- **Models**: Contains dto (Data transfer objects) used in the system, Use the record keyword instead of the class
  keyword for dto because record is immutable and faster to declare than class with c# 12

  ```cs
  public record CreateEmployeeRequest(
    string Name,
    string? Kana,
    string Email,
    string? Tel,
    string? Mobile,
    string? Gender,
    DateOnly? BirthDay
  )
  ```

  ```cs
  public record DataGetEmployeeManyResponse
  {
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? EMail { get; init; }
    public DataEmployeeMeta? Meta { get; init; }
  }

  public record DataEmployeeMeta
  {
    public string? Kana { get; init; }
    public DateTime? BirthDay { get; init; }
  }
  ```

- **Settings**: Contains dto mappings with environment variable objects in appsettings.json
  ```json
    "Cors": {
      "PolicyName": "MyOrigins",
      "AllowedOrigins": "*",
      "AllowedMethods": "GET,POST,PUT,PATCH,DELETE",
      "AllowedHeaders": "*",
      "ExposedHeaders": "*",
      "AllowCredentials": true,
      "MaxAge": 1800
    }
  ```
  ```cs
  public class WebCorsSetting
  {
    public string? PolicyName { get; set; }
    public string? AllowedOrigins { get; set; } = "*";
    public string AllowedMethods { get; set; } = "*";
    public string? AllowedHeaders { get; set; } = "*";
    public string? ExposedHeaders { get; set; } = "*";
    public bool AllowCredentials { get; set; }
    public int MaxAge { get; set; }
    public bool EnforceHttps { get; set; }
  }
  ```
- **UseCases**: Handle system domain business logic with commands and queries.
  <img src="./docs/images/UseCase.png">

  ```cs
    public record EmployeeCreateCommand : CreateCommandBase<CreateEmployeeRequest, string>;
  ```

  ```cs
    public class EmployeeCreateCommandHandler(
        ILogger<EmployeeCreateCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<Reservation.Employee.Application.Contexts.Entities.Employee> userManager,
        IEmployeeService employeeService,
        IAddressService addressService,
        IEmployeeMetaService employeeMetaService
    ) : CreateCommandHandlerBase<EmployeeCreateCommand, string>(
        unitOfWork,
        mapper
    )
    {
        protected override async Task<string> HandleAsync(
            EmployeeCreateCommand request,
            CancellationToken cancellationToken
        )
        {
            var payload = request.Payload;

            if (await userManager.FindByNameAsync(payload.Name) is not null)
            {
                throw new LoginAlreadyUsedException();
            }

            if (await userManager.FindByEmailAsync(payload.Email) is not null)
            {
                throw new EmailAlreadyUsedException();
            }

            var newEmployeeId = string.Empty;
            try
            {
                await UnitOfWork.BeginTransactionAsync(cancellationToken);

                var newAddress = await addressService.CreateAddress(
                    Mapper.Map<Address>(payload)
                );

                var newEmployeeMeta = await employeeMetaService.CreateEmployeeMeta(
                    Mapper.Map<EmployeeMeta>(payload)
                );

                var (newEmployee, _) = await employeeService.Create(
                    payload.Name,
                    payload.Email
                );

                newEmployee.Address = newAddress;
                newEmployee.EmployeeMeta = newEmployeeMeta;

                await UnitOfWork.Set<Reservation.Employee.Application.Contexts.Entities.Employee>()
                    .AddAsync(
                        newEmployee,
                        cancellationToken
                    );

                await UnitOfWork.CommitAsync(cancellationToken);

                newEmployeeId = newEmployee.Id;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create employee failed: {Message}", ex.Message);
                await UnitOfWork.RollbackAsync(cancellationToken);
            }

            return newEmployeeId;
        }
    }
  ```

  ```cs
    public record EmployeeGetAllPublicEmployeesQuery(
        IPageable Pageable
    )
        : IQueryPagedBase<DataGetEmployeeManyResponse>
    {
        public IPageable Pageable { get; set; } = Pageable;
    }
  ```

  ```cs
    public class EmployeeGetAllPublicEmployeesQueryHandler(
        IMapper mapper,
        IEmployeeRepository employeeRepository
    ) : QueryPageBaseHandler<EmployeeGetAllPublicEmployeesQuery, DataGetEmployeeManyResponse>(
        mapper
    )
    {
        protected override async Task<(IHeaderDictionary, IEnumerable<DataGetEmployeeManyResponse>)> HandleAsync(
            EmployeeGetAllPublicEmployeesQuery request,
            CancellationToken cancellationToken
        )
        {
            var queryable = employeeRepository
                .GetQueryableWithAsNoTracking(
                    new EmployeeGetAllPublicEmployeesQuerySpec(
                        request.Pageable,
                        x => x.IsEnabled
                    )
                )
                .ProjectTo<DataGetEmployeeManyResponse>(Mapper.ConfigurationProvider);
            var page = await queryable.UsePageableAsync(
                request.Pageable,
                cancellationToken: cancellationToken
            );

            var data = page.Content;
            var headers = page.GeneratePaginationHttpHeaders();

            return (headers, data);
        }
    }

    public class EmployeeGetAllPublicEmployeesQuerySpec
        : SpecificationBase<Reservation.Employee.Application.Contexts.Entities.Employee>
    {
        public EmployeeGetAllPublicEmployeesQuerySpec(
            IPageable pageable,
            Expression<Func<Reservation.Employee.Application.Contexts.Entities.Employee, bool>>? criteria = null
        )
        {
            Criteria = criteria;

            AddInclude(x => x.Address!);
            AddInclude(x => x.EmployeeMeta!);


            if (!pageable.Sort.Orders.Any())
            {
                return;
            }

            foreach (var sortOrder in pageable.Sort.Orders)
            {
                switch (sortOrder.Property)
                {
                    case "kana" when sortOrder.Direction is Direction.Asc:
                        ApplyOrderBy(x => x.EmployeeMeta!.Kana!);
                        break;
                    case "kana" when sortOrder.Direction is Direction.Desc:
                        ApplyOrderByDescending(x => x.EmployeeMeta!.Kana!);
                        break;
                    default:
                        ApplyOrderBy(x => x.Id);
                        break;
                }
            }
        }

        public override Expression<Func<Reservation.Employee.Application.Contexts.Entities.Employee, bool>>? Criteria
        {
            get;
        }
    }
  ```

  ```cs
    public record EmployeeGetAllQuery(
        IPageable Pageable
    ) : IQueryPagedBase<DataGetEmployeeManyResponse>
    {
        public IPageable Pageable { get; set; } = Pageable;
    }
  ```

  ```cs
    public class EmployeeGetAllQueryHandler(
        IMapper mapper,
        IEmployeeRepository employeeRepository
    ) : QueryPageBaseHandler<EmployeeGetAllQuery, DataGetEmployeeManyResponse>(
        mapper
    )
    {
        protected override async Task<(IHeaderDictionary, IEnumerable<DataGetEmployeeManyResponse>)> HandleAsync(
            EmployeeGetAllQuery request,
            CancellationToken cancellationToken
        )
        {
            var page = await employeeRepository
                .GetQueryableWithAsNoTracking()
                .Include(x => x.Address)
                .Include(x => x.EmployeeMeta)
                .ProjectTo<DataGetEmployeeManyResponse>(Mapper.ConfigurationProvider)
                .UsePageableAsync(
                    request.Pageable,
                    isApplySort: true,
                    cancellationToken: cancellationToken
                );

            var data = page.Content;
            var headers = page.GeneratePaginationHttpHeaders();

            return (headers, data);
        }
    }
  ```

- **Validations**: Contains validation for commands, queries or api requests (processing flow does not go through
  CQRS)

   ```cs
     public class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
     {
       public EmployeeCreateCommandValidator()
       {
         RuleFor(x => x.Payload.Name)
             .NotNull()
             .NotEmpty();

         RuleFor(x => x.Payload.Email)
             .NotNull()
             .NotEmpty()
             .EmailAddress();
       }
     }
   ```

3. **Liberty.Reservation.[Service name].Application**: Infrastructure layer of the service.

- **Constants**: Declare service constants

- **Contexts**: Declare the service's own DbContext and specify the DbSets within the scope of the service

- **Domains**: Contains separate Repositories and Services of the service

- **Exceptions**: Contains service-specific exceptions

- **UnitOfWork**: Declare a separate unit of work for the service

   ```cs
    public interface IEmployeeUnitOfWork : IUnitOfWork;
   ```

   ```cs
    public class EmployeeUnitOfWork(
        DbContext context
    ) : BaseUnitOfWork(context), IEmployeeUnitOfWork;
   ```

#### Layer Dependencies of a service

<img src="./docs/images/LibertySystemArchitect-LayerDependencies.drawio.svg" alt="">

#### Implement the business domain flow of a service

CQRS is a good choice for a PMS system, especially if the system has high requirements for performance, scalability and
flexibility.
For complex operations in the business domain, the execution flow of the system will need to use CQRS.

<img src="./docs/images/LibertySystemArchitect-BusinessFlow.drawio.svg" alt="">

#### Implement the normarl flow of a service

Implemented with simple operations such as CRUD for catalog data types, to reduce system complexity.

<img src="./docs/images/LibertySystemArchitect-NormalFlow.drawio.svg" alt="">

---

## Git Convention

<img src="./docs/images/Liberty-GitFlow.png" alt="">

### General Rules

| No | Checked Items                                                                                 | Assessment | Notes | Priority | Severity |
|----|-----------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | The subject of the commit message is limited to 50 characters                                 | Mandatory  |       | 1        |          |
| 2  | Capitalize the first letter of the subject                                                    |            |       | 2        |          |
| 3  | Do not end the subject with a period                                                          |            |       | 2        |          |
| 4  | Use an imperative style in the subject (Add password validation vs Added password validation) |            |       | 2        |          |
| 5  | Add a commit body when additional background for the commit is necessary                      |            |       | 2        |          |
| 6  | Body is separated from subject by one blank line                                              | Mandatory  |       | 1        |          |

### Semantic Subjects

| No | Checked Items                                                                                                                                                                                                                                                                                    | Assessment | Notes | Priority | Severity |
|----|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | Commit messages' subjects are preceded by a tag to make it easier to read through them and filter them out: <br> `<type>: <commit subject>` <br> For example: <br> `feat: #tasknumber short_Description` <br> `fix: #bugNumber short_Description` <br> `hotfix: #hotfixNumber short_Description` | Mandatory  |       | 1        |          |

### Tags

| No | Checked Items                                                                                                | Assessment | Notes | Priority | Severity |
|----|--------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | `feat`: New feature or functionality for the user, not a new feature for the build script                    | Mandatory  |       | 1        |          |
| 2  | `fix`: Bug fix for the user, not a fix to a build script                                                     | Mandatory  |       | 1        |          |
| 3  | `docs`: Changes to the documentation                                                                         |            |       | 2        |          |
| 4  | `style`: Formatting, missing semi-colons, etc; no production code change                                     |            |       | 2        |          |
| 5  | `refactor`: Code refactoring (variable renaming or code restructuring) that doesn't affect the functionality |            |       | 2        |          |
| 6  | `test`: Adding, fixing, or refactoring tests; no production code change                                      |            |       | 3        |          |
| 7  | `chore`: Updating build scripts or upgrading dependencies; no production code change                         |            |       | 3        |          |
| 8  | `misc`: Use for anything that doesn't clearly fall into any of the previous categories                       |            |       | 3        |          |

### Branch Name

| No | Checked Items                                                                                                                                                                                                            | Assessment | Notes | Priority | Severity |
|----|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | Branch name with new features <br> `features/<ticket_number>_<summary_feature>` <br> `bugs/<ticket_number>_<summary_feature>` <br> `hotfixs/<ticket_number>_<summary_feature>` <br> Example: `features/001_login_logout` | Mandatory  |       | 2        |          |

### Pull Request Name

| No | Checked Items                                                                                                                                                    | Assessment | Notes | Priority | Severity |
|----|------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | Pull Request title <br> `[<type>] <PR title>` <br> For example: <br> `[Feat] short_Description` <br> `[Fix] short_Description` <br> `[Hotfix] short_Description` | Mandatory  |       | 1        |          |

---

## Code Review Checklist

### General

| No | Checked Items                                                     | Assessment | Notes | Priority  | Severity |
|----|-------------------------------------------------------------------|------------|-------|-----------|----------|
| 1  | Does the code comply with the language and framework conventions? |            |       | Mandatory | 1        |

### Commenting

| No | Checked Items                                              | Assessment | Notes | Priority  | Severity |
|----|------------------------------------------------------------|------------|-------|-----------|----------|
| 1  | Has the comment been updated with the latest code?         |            |       | Mandatory | 1        |
| 2  | Is the comment clear and correct with the code?            |            |       |           |          |
| 3  | Did the comment describe why and how the code works?       |            |       |           |          |
| 4  | Exceptions, errors around have been commented yet?         |            |       |           |          |
| 5  | Has each operation and feature cluster been commented yet? |            |       |           |          |
| 6  | Have related events and features been commented yet?       |            |       |           |          |
| 7  | Is there a comment on each class title?                    |            |       |           |          |

### Source Code

| No | Checked Items                                                                                                                      | Assessment | Notes | Priority  | Severity |
|----|------------------------------------------------------------------------------------------------------------------------------------|------------|-------|-----------|----------|
| 1  | Does the name of the method/function make sense and describe what it will do?                                                      |            |       | Mandatory | 2        |
| 2  | Are the params used already described?                                                                                             |            |       | Mandatory | 1        |
| 3  | Are normal and exception streams clearly separated?                                                                                |            |       | Mandatory | 1        |
| 4  | When a logic thread is too long, has the action been split into smaller methods?                                                   |            |       |           | 2        |
| 5  | When the logic flow is too long, is it possible to reduce the conditional syntaxes like if-else, while, etc?                       |            |       |           | 2        |
| 6  | Have you minimized nested loops?                                                                                                   |            |       |           | 3        |
| 7  | Is the given variable name meaningful and easy to understand?                                                                      |            |       | Mandatory | 3        |
| 8  | Is the code easy to understand and straight to the point?                                                                          |            |       |           | 3        |
| 9  | With the complex code, are there explanations and comments to avoid confusion when maintaining?                                    |            |       | Mandatory | 2        |
| 10 | Have you used Tab with Space logically according to the structure?                                                                 |            |       |           | 2        |
| 11 | Is there only 1 command per line? Do not put multiple commands in 1 line, because it will be difficult to read and maintain later. |            |       |           | 1        |
| 14 | Is the variable name different from the class name?                                                                                |            |       | Mandatory | 3        |
| 15 | Is the function/method name set according to the general rules? For example, camelCase, is a verb.                                 |            |       |           | 2        |
| 16 | Is the global function's name different from the local function?                                                                   |            |       | Mandatory | 3        |
| 19 | Are names for folders and libraries defined in the document?                                                                       |            |       | Mandatory | 2        |
| 20 | Did the name and type of the folder match the required framework? For example, folder ./src to contain source code.                |            |       | Mandatory | 2        |
| 21 | Are 3rd party libraries used? If yes, then:                                                                                        |            |       |           |          |
| 22 | Has the customer approved this listing?                                                                                            |            |       |           |          |
| 23 | Is the license for the libraries to use appropriate and approved?                                                                  |            |       |           |          |
| 24 | Is there a line of code that is not being used?                                                                                    |            |       |           | 3        |

---

## Coding Guideline

### Get liberty-asp submodules

- init
  ```bash
  git submodule update --init --recursive
  ```
- update

  ```bash
  git submodule update --remote --recursive
  ```

- [liberty-asp](https://github.com/liberty-membership/liberty-asp)
  ```bash
  git submodule add https://github.com/liberty-membership/liberty-asp
  ```

### Migration

#### Step 1: Navigate to the project containing the DbContext

```bash
cd Liberty.Reservation.DbMigration
```

#### Step 2: Create a new migration

```bash
dotnet ef migrations add a001 --project Liberty.Reservation.DbMigration
```

#### Step 3: Generate the SQL script

```bash
./migration-script.sh
```

#### Step 4: Run the migration to update the database using DbUp

```bash
dotnet run --project Liberty.Reservation.DbMigration
```

### Code quality

**By Script :**

1. Run Sonar in container : `docker compose -f ./docker/sonar.yml up -d`

2. Wait container was up Run `SonarAnalysis.ps1` or `sonar-analysis.sh` and go to http://localhost:9001

**Manually :**

1. Run Sonar in container : `docker compose -f ./docker/sonar.yml up -d`

2. Install sonar scanner for .net :

`dotnet tool install --global dotnet-sonarscanner`

3. Run sonar begin

```bash
dotnet sonarscanner begin /d:sonar.login=admin /d:sonar.password=admin /k:"Liberty" /d:sonar.host.url="http://localhost:9001" /s:"`pwd`/SonarQube.Analysis.xml" ``
```

4. Build your application : `dotnet build`

5. Publish sonar results : `dotnet sonarscanner end /d:sonar.login=admin /d:sonar.password=admin`

6. Go to http://localhost:9001

#### Monitoring and Tracking the development local

1. Run `dotnet restore ; dotnet build ; dotnet run` with `Liberty.AppHost/Liberty.AppHost.csproj`

2. Go to Aspire Dashboard
   <img src="./docs/images/Aspire-1.png">
   <img src="./docs/images/Aspire-2.png">
   <img src="./docs/images/Aspire-3.png">
   <img src="./docs/images/Aspire-4.png">
   <img src="./docs/images/Aspire-5.png">

---

## Version Control and CI/CD

Will update soon

---

## References

Will update soon
