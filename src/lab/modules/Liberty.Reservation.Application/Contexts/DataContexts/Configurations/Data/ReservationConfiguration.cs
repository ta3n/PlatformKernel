using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class ReservationConfiguration : BaseDataEntityTypeConfiguration<Entities.Data.Reservation>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Entities.Data.Reservation> builder
    )
    {
        builder.ToTable("Reservation", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.ReservationData);

        builder
            .HasMany(c => c.ReservationQuestions)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);

        builder
            .HasMany(c => c.ReservationPoints)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDateOptionItems)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDatePersonAgeTypes)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);

        builder
            .HasMany(c => c.ReservationPlanRoomGroupAppDates)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);

        builder
            .HasMany(c => c.ReservationPoints)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);

        builder
            .HasMany(c => c.OrderReservations)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);
    }
}
