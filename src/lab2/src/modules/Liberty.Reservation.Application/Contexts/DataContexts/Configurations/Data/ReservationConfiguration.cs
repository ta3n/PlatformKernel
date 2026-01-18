using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class ReservationConfiguration : BaseDataEntityTypeConfiguration<Entities.Data.Reservation>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Entities.Data.Reservation> builder
    )
    {
        builder.ToTable("reservation", DbConfiguration.DefaultSchema);

        builder.Property(e => e.BookingData)
            .HasColumnType("jsonb")
            .HasDefaultValue(
                new BookingData
                {
                    Facility = new(),
                    Plan = new(),
                    RoomGroup = new(),
                    Site = new()
                }
            )
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<BookingData>(v)
            );

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
            .HasMany(c => c.OrderReservations)
            .WithOne(c => c.Reservation)
            .HasForeignKey(c => c.ReservationId);
    }
}
