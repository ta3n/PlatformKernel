using Liberty.Reservation.Application.Contexts.DataContexts.Configurations;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Employee.Application.Contexts.Configurations;

public class FileEmployeeConfiguration : BaseRelationEntityTypeConfiguration<FileEmployee>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FileEmployee> builder
    )
    {
        builder.ToTable("FileEmployee");

        builder.HasKey(
            x => new
            {
                x.FileId,
                x.EmployeeId
            }
        );
    }
}
