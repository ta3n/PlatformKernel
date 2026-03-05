using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Employee.Application.Contexts.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Entities.Employee>
{
    public void Configure(
        EntityTypeBuilder<Entities.Employee> builder
    )
    {
        builder.ToTable("Employee");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnOrder(1);

        builder
            .HasMany(c => c.FileEmployees)
            .WithOne(c => c.Employee)
            .HasForeignKey(c => c.EmployeeId);
    }
}
