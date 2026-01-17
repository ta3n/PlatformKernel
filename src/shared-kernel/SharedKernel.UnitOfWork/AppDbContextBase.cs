using Microsoft.EntityFrameworkCore;
using SharedKernel.UnitOfWork.DbFunctions;
using SharedKernel.UnitOfWork.DbFunctions.Base;

namespace SharedKernel.UnitOfWork;

/// <summary>
/// Represents a foundational class for constructing application-specific database contexts using Entity Framework Core.
/// Offers core functionalities such as applying entity configurations, managing database functions, and handling
/// changes or asynchronous operations within a database context.
/// </summary>
/// <typeparam name="TContext">The derived type of the specific database context inheriting from this base class.</typeparam>
public class AppDbContextBase<TContext>(
    DbContextOptions options
) : DbContext(options)
    where TContext : DbContext
{
    /// <summary>
    /// A private, read-only collection of database function register instances implementing the
    /// <see cref="IDbFunctionRegister"/> interface. It is primarily used to define and register
    /// custom database functions within the <see cref="ModelBuilder"/> during the Entity Framework Core
    /// model configuration process.
    /// </summary>
    /// <remarks>
    /// This collection is pre-populated with a default set of database function registers upon
    /// initialization, such as the <see cref="JsonbExtensionsRegister"/>, and can be iterated
    /// to apply all the defined function registrations to the model.
    /// </remarks>
    private readonly List<IDbFunctionRegister> _dbFunctionRegisters =
    [
        new JsonbExtensionsRegister(),
        new AppDateExtensionsRegister(),
        new ReservationExtensionsRegister(),
        new TimeZoneExtensionsRegister(),
        new TimeExtensionsRegister()
    ];

    /// <summary>
    /// Configures the entity framework model for the context using the provided <see cref="ModelBuilder"/> instance.
    /// This method is overridden in derived classes to define additional configurations such as applying entity configurations
    /// from assemblies or establishing relationships between entities.
    /// </summary>
    /// <param name="modelBuilder">
    /// An instance of <see cref="ModelBuilder"/> that is used to configure the entity framework model for the context.
    /// </param>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);

        RegisterAllDbFunctions(modelBuilder);
    }

    /// <summary>
    /// Registers all custom database functions with the provided <see cref="ModelBuilder"/> instance.
    /// This method iterates through a collection of database function registers and applies
    /// their respective function registrations to the model builder.
    /// </summary>
    /// <param name="modelBuilder">
    /// An instance of <see cref="ModelBuilder"/> used to register the database functions.
    /// </param>
    private void RegisterAllDbFunctions(
        ModelBuilder modelBuilder
    )
    {
        foreach (var register in _dbFunctionRegisters)
        {
            register.RegisterFunctions(modelBuilder);
        }
    }
}
