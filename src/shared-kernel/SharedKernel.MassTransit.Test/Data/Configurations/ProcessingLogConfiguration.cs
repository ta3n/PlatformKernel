using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.MassTransit.Test.Domain;

namespace SharedKernel.MassTransit.Test.Data.Configurations;

public sealed class ProcessingLogConfiguration : IEntityTypeConfiguration<ProcessingLog>
{
    public void Configure(
        EntityTypeBuilder<ProcessingLog> builder
    )
    {
        builder.ToTable("test_processing_logs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Step)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(log => log.Source)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(log => log.OrderId);
    }
}
