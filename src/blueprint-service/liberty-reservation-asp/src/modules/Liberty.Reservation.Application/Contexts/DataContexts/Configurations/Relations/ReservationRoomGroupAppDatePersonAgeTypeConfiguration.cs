using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ReservationRoomGroupAppDatePersonAgeTypeConfiguration
    : BaseRelationEntityTypeConfiguration<ReservationRoomGroupAppDatePersonAgeType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ReservationRoomGroupAppDatePersonAgeType> builder
    )
    {
        builder.ToTable("reservation_room_group_app_date_person_age_type", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.ReservationId,
                    c.RoomGroupId,
                    c.BookingDateId,
                    c.PersonAgeTypeId,
                    c.RoomGroupIndex
                }
            );

        builder
            .HasOne(sc => sc.Reservation)
            .WithMany(s => s.ReservationRoomGroupAppDatePersonAgeTypes)
            .HasForeignKey(sc => sc.ReservationId);

        builder
            .HasOne(sc => sc.PersonAgeType)
            .WithMany(s => s.ReservationRoomGroupAppDatePersonAgeTypes)
            .HasForeignKey(sc => sc.PersonAgeTypeId);

        builder
            .HasOne(sc => sc.PersonAgeType)
            .WithMany(s => s.ReservationRoomGroupAppDatePersonAgeTypes)
            .HasForeignKey(sc => sc.PersonAgeTypeId);

        builder
            .HasOne(sc => sc.PersonAgeType)
            .WithMany(s => s.ReservationRoomGroupAppDatePersonAgeTypes)
            .HasForeignKey(sc => sc.PersonAgeTypeId);
    }
}
