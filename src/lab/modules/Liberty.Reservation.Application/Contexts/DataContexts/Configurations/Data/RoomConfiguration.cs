using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class RoomConfiguration : BaseDataEntityTypeConfiguration<Room>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Room> builder
    )
    {
        builder.ToTable("Room", DbConfiguration.DefaultSchema); // FIXME Room

        builder.Ignore(t => t.Meta);

        builder
            .HasMany(c => c.FileRooms)
            .WithOne(c => c.Room)
            .HasForeignKey(c => c.RoomId);

        builder
            .HasMany(c => c.RoomRoomGroups)
            .WithOne(c => c.Room)
            .HasForeignKey(c => c.RoomId);
    }
}
