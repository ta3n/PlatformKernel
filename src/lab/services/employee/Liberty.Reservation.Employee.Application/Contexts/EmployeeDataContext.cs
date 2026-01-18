using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Liberty.Reservation.Employee.Application.Contexts.Entities.File;

namespace Liberty.Reservation.Employee.Application.Contexts;

public class EmployeeDataContext(
    DbContextOptions<EmployeeDataContext> options
) : IdentityDbContext<Entities.Employee>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Entities.Employee> Employees { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<FileEmployee> FileEmployees { get; set; }

    protected override void OnModelCreating(
        ModelBuilder builder
    )
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(EmployeeDataContext).Assembly);
    }
}
