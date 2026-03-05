using Liberty.Reservation.Application.Contexts.DataContexts.Configurations;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Employee.Application.Contexts.Configurations;

public class EmployeeMetaConfiguration : BaseDataEntityTypeConfiguration<EmployeeMeta>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<EmployeeMeta> builder
    )
    {
        builder.ToTable("EmployeeMeta");

        builder.Ignore(x => x.Temporarily);
    }
}
