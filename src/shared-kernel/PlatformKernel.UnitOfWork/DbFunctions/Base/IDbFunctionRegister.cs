using Microsoft.EntityFrameworkCore;

namespace PlatformKernel.UnitOfWork.DbFunctions.Base;

/// <summary>
/// Provides a contract for registering custom database functions in an Entity Framework Core
/// model. Implementations of this interface are responsible for defining and registering
/// database-specific or custom SQL functions with the <see cref="ModelBuilder"/> during the
/// model creation phase.
/// </summary>
/// <remarks>
/// This interface is typically used in scenarios where static database functions or
/// user-defined functions (UDFs) need to be mapped and accessed via LINQ queries in Entity
/// Framework Core. Registering a function enables its use in the application's LINQ
/// expressions and query translations.
/// </remarks>
public interface IDbFunctionRegister
{
    /// <summary>
    /// Registers database functions within the specified <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/> used to configure and map entity types to the database schema.</param>
    void RegisterFunctions(
        ModelBuilder modelBuilder
    );
}
